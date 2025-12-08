using Givehub.Helper;
using Givehub.Helpers;
using Givehub.Models;
using Givehub.Models.ViewModels;
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

        public IActionResult CreateDonee()
        {
            return View(new DoneeVM());
        }

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
                Requirements = vm.Requirements,
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


        public IActionResult DoneeHomePage()
        {
            var donees = _context.Donees.Include(d => d.Donations).ToList();
            return View(donees);
        }

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
                Requirements = donee.Requirements,
                Description = donee.Description,
                Image = donee.Image
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> EditDonee(DoneeVM vm)
        {
            var existing = await _context.Donees.FindAsync(vm.Id);

            if (existing == null)
                return NotFound();

            existing.Name = vm.Name;
            existing.Category = vm.Category;
            existing.Address = vm.Address;
            existing.Requirements = vm.Requirements;
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


