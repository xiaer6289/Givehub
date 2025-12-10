using Givehub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Cryptography;
using System.Text;

namespace Givehub.Controllers
{
    public class AccountController : Controller
    {
        private readonly DB _context;
        private readonly IEmailService _emailService;

        public AccountController(DB context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Email
                var existingEmail = await _context.Donors
                    .FirstOrDefaultAsync(d => d.Email == model.Email);

                if (existingEmail != null)
                {
                    ViewBag.EmailError = "Email already registered";
                    return View(model);
                }

                // PhoneNo
                var existingPhone = await _context.Donors
                    .FirstOrDefaultAsync(d => d.PhoneNo == model.PhoneNo);

                if (existingPhone != null)
                {
                    ViewBag.PhoneError = "Phone number already registered";
                    return View(model);
                }

                // Hashing Password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var donor = new Donor
                {
                    Name = model.Name,
                    Email = model.Email,
                    PhoneNo = model.PhoneNo,
                    Password = hashedPassword,
                    AdminId = 1
                };

                _context.Donors.Add(donor);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToAction("Register");
            }
            catch
            {
                ViewBag.ErrorMessage = "An error occurred during registration. Please try again.";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            ViewBag.EmailError = null;
            ViewBag.PasswordError = null;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == model.Email);

            if (admin != null)
            {
                if (model.Password != admin.Password)
                {
                    ViewBag.PasswordError = "Incorrect password";
                    return View(model);
                }

                HttpContext.Session.SetInt32("AdminId", admin.Id);
                HttpContext.Session.SetString("Role", "Admin");
                return RedirectToAction("AdminHomePage", "Admin");
            }

            var donor = await _context.Donors
                .FirstOrDefaultAsync(d => d.Email == model.Email);

            if (donor != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(model.Password, donor.Password))
                {
                    ViewBag.PasswordError = "Incorrect password";
                    return View(model);
                }


                HttpContext.Session.SetInt32("DonorId", donor.Id);
                HttpContext.Session.SetString("Role", "Donor");
                return RedirectToAction("Index", "Home");
            }

            ViewBag.EmailError = "Email not found";
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var donor = await _context.Donors
                .FirstOrDefaultAsync(d => d.Email == email);

            if (donor == null)
            {
                TempData["InvalidMessage"] = "Email not found. Please enter a valid email.";
                return View();
            }

            var existingTokens = _context.PasswordResetTokens
                .Where(t => t.DonorId == donor.Id);
            _context.PasswordResetTokens.RemoveRange(existingTokens);

            // Generate secure token
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var resetToken = new PasswordResetToken
            {
                DonorId = donor.Id,
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(5) 
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            // Create reset link
            var resetLink = Url.Action("ResetPassword", "Account",
                new { token, email = donor.Email }, Request.Scheme);

            var emailBody = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ 
                        font-family: Arial, sans-serif; 
                        line-height: 1.6; 
                        color: #333;
                    }}
                    .container {{
                        max-width: 600px; 
                        margin: 0 auto; 
                        padding: 20px; 
                    }}
                    .button {{ 
                        display: inline-block; 
                        padding: 12px 24px; 
                        background-color: #007bff; 
                        color: white; 
                        text-decoration: none; 
                        border-radius: 5px; 
                        margin: 20px 0;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h2>Password Reset Request</h2>
                    <p>Hello {donor.Name},</p>
                    <p>We received a request to reset your password. Click the button below to reset it:</p>
                    <a href='{resetLink}' class='button'>Reset Password</a>
                    <p><strong>Please change your password within 5 minutes, or the request will expire.</strong></p>
                    <p>If you didn't request this password reset, please ignore this email. Your password will remain unchanged.</p>
                </div>
            </body>
            </html>
            ";

            await _emailService.SendEmailAsync(donor.Email, "Reset Your Password - GiveHub", emailBody);

            TempData["Message"] = "Password reset link has been sent to your email!";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var donor = await _context.Donors
                .FirstOrDefaultAsync(d => d.Email == email);

            if (donor == null)
            {
                TempData["Error"] = "Invalid reset link.";
                return RedirectToAction("Message");
            }

            var resetToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t =>
                    t.DonorId == donor.Id &&
                    t.Token == token &&
                    t.Expiration > DateTime.UtcNow);

            if (resetToken == null)
            {
                TempData["Error"] = "Invalid or expired reset link.";
                return RedirectToAction("Message");
            }

            var model = new ResetPasswordVM
            {
                Token = token,
                Email = email
            };

            // Pass Token and Email to view
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var donor = await _context.Donors
                .FirstOrDefaultAsync(d => d.Email == model.Email);

            if (donor == null)
            {
                TempData["Error"] = "Invalid reset link.";
                return RedirectToAction("Message");
            }

            var resetToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t =>
                    t.DonorId == donor.Id &&
                    t.Token == model.Token &&
                    t.Expiration > DateTime.UtcNow);

            //if (resetToken == null)
            //{
            //    TempData["Error"] = "Invalid or expired reset l   ink.";
            //    return RedirectToAction("Message");
            //}

            donor.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            _context.PasswordResetTokens.Remove(resetToken);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Password reset successful! Go back to your previous tab and login.";
            return RedirectToAction("Message");
        }

        public IActionResult Message()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
