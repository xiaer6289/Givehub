using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;
using Givehub.Models;

namespace Givehub.Controllers
{
    public class AccountController : Controller
    {
        private readonly DB _context;

        public AccountController(DB context)
        {
            _context = context;
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
                string hashedPassword = HashPassword(model.Password);

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
            ViewBag.PasswordError = null;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Donors.FirstOrDefaultAsync(d => d.Email == model.Email);
            if (user == null)
            {
                ViewBag.EmailError = "Email not found";
                return View(model);
            }

            string hashedPassword = HashPassword(model.Password);
            if (user.Password != hashedPassword)
            {
                ViewBag.PasswordError = "Incorrect password";
                return View(model);
            }

            HttpContext.Session.SetInt32("DonorId", user.Id);

            return RedirectToAction("Index", "Home");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }
        public IActionResult ResetPassword()
        {
            return View();
        }
    }
}
