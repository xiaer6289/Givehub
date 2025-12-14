using Givehub.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Claims;
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

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var donor = new Donor
                {
                    Name = model.Name,
                    Email = model.Email,
                    PhoneNo = model.PhoneNo,
                    Password = hashedPassword,
                    AdminId = 2
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

                var existingTokens = _context.AdminLoginTokens
                .Where(t => t.AdminId == admin.Id);
                _context.AdminLoginTokens.RemoveRange(existingTokens);

                // Generate secure token
                var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

                var loginToken = new AdminLoginToken
                {
                    AdminId = admin.Id,
                    Token = token,
                    Expiration = DateTime.UtcNow.AddMinutes(5)
                };

                _context.AdminLoginTokens.Add(loginToken);
                await _context.SaveChangesAsync();

                // Create verification link
                var verificationLink = Url.Action("VerifyAdminLogin", "Account",
                     new { token, email = admin.Email }, Request.Scheme);

                var emailBody = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .button {{ 
                            display: inline-block; 
                            padding: 12px 24px; 
                            background-color: #28a745; 
                            color: white; 
                            text-decoration: none; 
                            border-radius: 5px; 
                            margin: 20px 0;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2>🔐 Admin Login Verification</h2>
                        <p>Hello Admin,</p>
                        <p>Click the button below to complete your login:</p>
                        <a href='{verificationLink}' class='button'>✅ Verify and Login</a>
                        <p><strong>This link will expire in 5 minutes.</strong></p>
                        <p>If you didn't attempt to login, please ignore this email.</p>
                    </div>
                </body>
                </html>
                ";

                await _emailService.SendEmailAsync(admin.Email, "Admin Login Verification - GiveHub", emailBody);

                ViewBag.Message = "Verification email sent! Please check your email and click the button to login.";
                return View(model);
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

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, donor.Id.ToString()),
                    new Claim(ClaimTypes.Role, "Donor"),
                    new Claim(ClaimTypes.Email, donor.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.EmailError = "Email not found";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> VerifyAdminLogin(string token, string email)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == email);

            if (admin == null)
            {
                TempData["Error"] = "Invalid verification link.";
                return RedirectToAction("Message");
            }

            var loginToken = await _context.AdminLoginTokens
                .FirstOrDefaultAsync(t =>
                    t.AdminId == admin.Id &&
                    t.Token == token &&
                    t.Expiration > DateTime.UtcNow);

            if (loginToken == null)
            {
                TempData["Error"] = "Invalid or expired verification link. Please try logging in again.";
                return RedirectToAction("Message");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Email, admin.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


            _context.AdminLoginTokens.Remove(loginToken);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Login successful! Please return to your previous tab to continue working.";
            return RedirectToAction("Message", "Account");
        }

        [HttpGet]
        public IActionResult CheckAdminLoginStatus()
        {
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);
            var loggedIn = roleClaim == "Admin";
            return Json(new { loggedIn });
        }

        [HttpGet]
        public IActionResult CheckDonorResetStatus()
        {
            var resetDoneClaim = User.FindFirst("PasswordResetDone")?.Value;
            var resetDone = resetDoneClaim == "true";
            return Json(new { resetDone });
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

            donor.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            _context.PasswordResetTokens.Remove(resetToken);

            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim("PasswordResetDone", "true")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


            TempData["Success"] = "Password reset successful! Go back to your previous tab and login.";
            return RedirectToAction("Message");
        }

        public IActionResult Message()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
