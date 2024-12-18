using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCTemplate.Data;
using MVCTemplate.Models;
using System.Text.RegularExpressions;

namespace MVCTemplate.Controllers
{

    public class FileUploadController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly Context _context;
        private readonly string _uploadDirectory;

        public FileUploadController(UserManager<IdentityUser> userManager, Context context)
        {
            _userManager = userManager;
            _context = context;
            _uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        }

        public async Task<IActionResult> Index(string? searchTerm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            var allUploadedFiles = await _context.UploadedFile.Where(e => e.UserId == user.Id).ToListAsync();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                allUploadedFiles = allUploadedFiles
                    .Where(e => (e.OriginalFileName != null && e.OriginalFileName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                                 e.Description.Contains(searchTerm))
                    .ToList();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_UploadedFilesTable", allUploadedFiles);
            }

            return View(allUploadedFiles);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, UploadedFile model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please choose a file first.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(model.Description))
            {
                TempData["Error"] = "Please provide a description for the file.";
                return RedirectToAction("Index");
            }

            try
            {
                var allowedExtensions = new[] { ".pdf", ".png", ".jpg", ".docx" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["Error"] = "Invalid file type.";
                    return RedirectToAction("Index");
                }

                var fileMimeType = file.ContentType;
                var allowedMimeTypes = new[] { "application/pdf", "image/png", "image/jpeg", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };
                if (!allowedMimeTypes.Contains(fileMimeType))
                {
                    TempData["Error"] = "Invalid file type.";
                    return RedirectToAction("Index");
                }

                if (file.Length > 5 * 1024 * 1024)
                {
                    TempData["Error"] = "File size exceeds the limit of 5MB.";
                    return RedirectToAction("Index");
                }

                if (!Directory.Exists(_uploadDirectory))
                {
                    Directory.CreateDirectory(_uploadDirectory);
                }

                var sanitizedFileName = Regex.Replace(Path.GetFileName(file.FileName), @"[^\w\.]", "_");
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(sanitizedFileName);
                var filePath = Path.Combine(_uploadDirectory, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileUpload = new UploadedFile
                {
                    UserId = user.Id,
                    OriginalFileName = file.FileName,
                    UniqueFileName = uniqueFileName,
                    Description = model.Description,
                };

                _context.UploadedFile.Add(fileUpload);
                await _context.SaveChangesAsync();

                TempData["Success"] = "File uploaded successfully.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while uploading the file. Please try again.";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DownloadFile(string UniqueFileName)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var fileRecord = await _context.UploadedFile
                .FirstOrDefaultAsync(f => f.UserId == user.Id && f.UniqueFileName == UniqueFileName);

            if (fileRecord == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(_uploadDirectory, fileRecord.UniqueFileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", fileRecord.OriginalFileName);
        }
    }
}
