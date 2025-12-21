using Givehub.Helpers;
using Givehub.Models;
using Givehub.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Givehub.Controllers
{
    public class DoneeController : Controller
    {
        private readonly DB _context;
        private readonly DoneeHelper _helper;

        public DoneeController(DB context, DoneeHelper helper)
        {
            _context = context;
            _helper = helper;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateDonee()
        {
            return View(new DoneeVM());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDonee(DoneeVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var donee = new Donee
            {
                Name = vm.Name,
                Category = vm.Category,
                Address = vm.Address,
                Requirements = vm.Requirements != null ? string.Join(",", vm.Requirements) : null,
                Description = vm.Description,
                Date = DateTime.Now,
                AdminId = 1
            };

            if (vm.ImageFile != null && vm.ImageFile.Length > 0)
            {
                string error = _helper.ValidatePhoto(vm.ImageFile);
                if (!string.IsNullOrEmpty(error))
                {
                    ModelState.AddModelError("ImageFile", error);
                    return View(vm);
                }

                donee.Image = _helper.SavePhoto(vm.ImageFile, "images/donee");
            }

            _context.Donees.Add(donee);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Donee created successfully!";
            return RedirectToAction("DoneeHomePage");
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckNameExists(string name, int id)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Json(true);

            var exists = _context.Donees
                .Any(d => d.Name.ToLower() == name.ToLower() && d.Id != id);

            if (exists)
                return Json($"The name '{name}' already exists in our system.");

            return Json(true);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DoneeHomePage(string search, string category, int page = 1)
        {
            int pageSize = 8;
            var query = _context.Donees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(d =>
                    d.Name.ToLower().Contains(search) ||
                    d.Address.ToLower().Contains(search)
                );
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(d => d.Category == category);
            }

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var donees = query
                .OrderBy(d => d.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;
            ViewBag.Category = category;

            return View(donees);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditDonee(int id)
        {
            var donee = _context.Donees.Find(id);
            if (donee == null) return NotFound();

            var vm = new DoneeVM
            {
                Id = donee.Id,
                Name = donee.Name,
                Category = donee.Category,
                Address = donee.Address,
                Requirements = string.IsNullOrEmpty(donee.Requirements)
                       ? new List<string>()
                       : donee.Requirements.Split(',').Select(r => r.Trim()).ToList(),
                Description = donee.Description,
                Image = donee.Image
            };

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditDonee(DoneeVM vm)
        {
            var existing = await _context.Donees.FindAsync(vm.Id);

            if (existing == null)
                return NotFound();

            existing.Name = vm.Name;
            existing.Category = vm.Category;
            existing.Address = vm.Address;
            existing.Requirements = string.Join(", ", vm.Requirements ?? new List<string>());
            existing.Description = vm.Description;


            if (vm.ImageFile != null && vm.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(vm.ImageFile.FileName);
                var filePath = Path.Combine("wwwroot/images/donee", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await vm.ImageFile.CopyToAsync(stream);
                }

                existing.Image = "/images/donee/" + fileName;
            }

            _context.Update(existing);
            await _context.SaveChangesAsync();

            return RedirectToAction("DoneeHomePage");
        }



        [Authorize]
        public IActionResult ViewDonee(int id, string mode)
        {
            var donee = _context.Donees.Find(id);
            if (donee == null)
                return NotFound();

            var vm = new DoneeVM
            {
                Id = donee.Id,
                Name = donee.Name,
                Category = donee.Category,
                Address = donee.Address,
                Requirements = !string.IsNullOrEmpty(donee.Requirements)
                    ? donee.Requirements.Split(',').Select(r => r.Trim()).ToList()
                    : new List<string>(),
                Description = donee.Description,
                Image = donee.Image,
                Date = donee.Date
            };

            // 🔥 FORCE user view when coming from Home
            if (mode == "user")
                return View("ViewDonee.User", vm);

            // Admin pages
            if (User.IsInRole("Admin"))
                return View("ViewDonee.Admin", vm);

            // Default
            return View("ViewDonee.User", vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDonee(int id)
        {
            var donee = await _context.Donees.FindAsync(id);
            if (donee == null) return NotFound();

            if (!string.IsNullOrEmpty(donee.Image))
                _helper.DeletePhoto(donee.Image, "images/donee");

            _context.Donees.Remove(donee);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Donee deleted successfully!";
            return RedirectToAction("DoneeHomePage");
        }
    }
}


