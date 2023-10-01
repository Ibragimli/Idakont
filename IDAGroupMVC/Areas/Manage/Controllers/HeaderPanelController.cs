using IDAGroupMVC.Helper;
using IDAGroupMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDAGroupMVC.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin , Admin")]
    public class HeaderPanelController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public HeaderPanelController(DataContext context, IWebHostEnvironment env) { _context = context; _env = env; }

        public IActionResult Index()
        {
            var header = _context.Settings.Where(x => x.IsDelete == false).AsQueryable();
            header = header.Where(x => x.Key.StartsWith("Head"));
            return View(header.ToList());
        }
        public async Task<IActionResult> Edit(int id)
        {
            if (!SettingManager.SettingExists(id, _context)) return RedirectToAction("notfound", "error");
            var setting = await _context.Settings.Where(x => x.IsDelete == false).FirstOrDefaultAsync(x => x.Id == id);
            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Setting setting)
        {
            if (!SettingExists(setting.Id)) return RedirectToAction("notfound", "error");

            Setting settingExist = await _context.Settings.Where(x => x.IsDelete == false).FirstOrDefaultAsync(x => x.Id == setting.Id);

            //Required
            IsRequired(setting);
            if (!ModelState.IsValid) return View(settingExist);


            EditChange(setting, settingExist);
            SaveChange();


            return RedirectToAction(nameof(Index));

        }


        private bool SettingExists(int id)
        {
            return _context.Settings.Where(x => x.IsDelete == false).Any(e => e.Id == id);
        }
        private void IsRequired(Setting setting)
        {
            if (setting.Value == null)
            {
                ModelState.AddModelError("Value", "Value is required");
            }
        }
        private void EditChange(Setting setting, Setting settingExist)
        {
            if (settingExist.Value != null)
                settingExist.Value = setting.Value;
            if (setting.KeyImageFile != null)
            {
                settingExist.KeyImageFile = settingExist.KeyImageFile;
                PosterCheck(setting.KeyImageFile);
                DeleteFile(settingExist.Value, "settings");
                settingExist.Value = FileSave(setting.KeyImageFile, "settings");
            }
            settingExist.ModifiedDate = DateTime.UtcNow.AddHours(4);
            SaveChange();
        }
        private void SaveChange()
        {
            _context.SaveChanges();
        }
        private void PosterCheck(IFormFile PosterImageFile)
        {
            if (PosterImageFile.ContentType != "image/png" && PosterImageFile.ContentType != "image/jpeg" && PosterImageFile.ContentType != "image/webp")
                ModelState.AddModelError("Value", "Resmin yalnız (png, jpg və ya webp) type-ında ola bilər");


            if (PosterImageFile.Length > 1 * 2097152)
                ModelState.AddModelError("Value", "Resmin maks yaddaşı 2 MB ola bilər!");

        }
        private void ImagesCheck(List<IFormFile> Images)
        {
            if (Images != null)
            {
                if (Images.Count > 8)
                    ModelState.AddModelError("Value", "Maksimum 8 şəkil əlavə edə bilərsiniz");

                foreach (var image in Images)
                {
                    if (image.ContentType != "image/png" && image.ContentType != "image/jpeg" && image.ContentType != "image/webp")
                        ModelState.AddModelError("Value", "Resmin yalnız (png, jpg və ya webp) type-ında ola bilər");

                    if (image.Length > 1 * 2097152)
                        ModelState.AddModelError("Value", "Resmin maks yaddaşı 2 MB ola bilər!");

                }
            }

        }
        private string FileSave(IFormFile Image, string folderName)
        {
            string image = FileManager.Save(_env.WebRootPath, "uploads/" + folderName, Image);
            return image;
        }
        private void DeleteFile(string image, string folderName)
        {
            FileManager.Delete(_env.WebRootPath, "uploads/" + folderName, image);
        }

    }
}
