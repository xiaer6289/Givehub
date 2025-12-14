using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;

namespace Givehub.Helpers
{
    public class DoneeHelper
    {
        private readonly IWebHostEnvironment _env;

        public DoneeHelper(IWebHostEnvironment env)
        {
            _env = env;
        }

        // ------------------------------------------------------------------------
        // Photo Validation
        // ------------------------------------------------------------------------
        public string ValidatePhoto(IFormFile file)
        {
            var reType = new Regex(@"^image\/(jpeg|png)$", RegexOptions.IgnoreCase);
            var reName = new Regex(@"^.+\.(jpeg|jpg|png)$", RegexOptions.IgnoreCase);

            if (!reType.IsMatch(file.ContentType) || !reName.IsMatch(file.FileName))
            {
                return "Only JPG and PNG images are allowed.";
            }

            if (file.Length > 2 * 1024 * 1024) // 2MB max
            {
                return "Image size cannot exceed 2MB.";
            }

            return string.Empty;
        }

        // ------------------------------------------------------------------------
        // Save photo with resizing
        // ------------------------------------------------------------------------
        public string SavePhoto(IFormFile file, string folder)
        {
            // generate unique filename
            var fileName = Guid.NewGuid().ToString("n") + ".jpg";

            // ensure folder exists
            var folderPath = Path.Combine(_env.WebRootPath, folder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, fileName);

            // Resize image to 200x200 crop
            var options = new ResizeOptions
            {
                Size = new(200, 200),
                Mode = ResizeMode.Crop
            };

            using var stream = file.OpenReadStream();
            using var img = Image.Load(stream);
            img.Mutate(x => x.Resize(options));
            img.Save(filePath);

            // return relative path for Razor view
            return "/" + folder.Replace("\\", "/") + "/" + fileName;
        }

        // ------------------------------------------------------------------------
        // Delete photo
        // ------------------------------------------------------------------------
        public void DeletePhoto(string file, string folder)
        {
            if (string.IsNullOrEmpty(file)) return;

            file = Path.GetFileName(file); // remove path if exists
            var path = Path.Combine(_env.WebRootPath, folder, file);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
