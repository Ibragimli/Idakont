using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IDAGroupMVC.Models;
using IDAGroupMVC.Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace IDAGroupMVC.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin , Admin")]
    public class CompaniesController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public CompaniesController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Manage/Companies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Companies.Include(x => x.CompanyImages).Where(x => x.IsDelete == false).ToListAsync());
        }


        // GET: Manage/Companies/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company)
        {
            ///Required
            IsRequired(company);
            if (_context.Companies.Any(x => x.Name == company.Name))
            {
                ModelState.AddModelError("Name", "Name is already!");
                return View();
            }
            if (!ModelState.IsValid) return View();

            //Check
            PosterImageCheck(company);
            ImagesCheck(company);
            if (!ModelState.IsValid) return View();

            //Create
            CreatePosterImage(company);
            if (company.ImageFiles != null)
                CreateImages(company);


            //SaveChange
            SaveChange(company);
            SaveContext();
            return RedirectToAction(nameof(Index));
        }

        // GET: Manage/Companies/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!CompanyExists(id)) return RedirectToAction("notfound", "error");
            var company = await _context.Companies.Include(x => x.CompanyImages).Where(x => x.IsDelete == false).FirstOrDefaultAsync(x => x.Id == id);
            return View(company);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Company company)
        {
            if (!CompanyExists(company.Id)) return RedirectToAction("notfound", "error");

            Company companyExist = await _context.Companies.Include(x => x.CompanyImages).Where(x => x.IsDelete == false).FirstOrDefaultAsync(x => x.Id == company.Id);
            if (companyExist.Name != company.Name && _context.Companies.Any(x => x.Name == company.Name))
            {
                ModelState.AddModelError("Name", "Name is already!");
                return View(companyExist);
            }
            //Required
            EditIsRequired(company);
            if (!ModelState.IsValid) return View(companyExist);

            if (company.PosterImageFile != null)
            {
                PosterImageCheck(company);
                if (!ModelState.IsValid) return View(companyExist);
                EditPosterImageSave(company, companyExist);
            }
            ImagesDelete(company, companyExist);

            if (company.ImageFiles !=null)
            {
                //ImagesDelete
                EditImageSave(company, companyExist);
            }

            EditChange(company, companyExist);
            SaveContext();
            return RedirectToAction(nameof(Index));

        }

        // GET: Manage/Companies/Delete/5
        public IActionResult Delete(int id)
        {
            var company = _context.Companies.Include(x => x.CompanyImages).FirstOrDefault(x => x.Id == id);
            if (company == null) return RedirectToAction("notfound", "error");
            var PosterImage = company.CompanyImages.FirstOrDefault(x => x.PosterStatus == true);
            FileManager.Delete(_env.WebRootPath, "uploads/companies", PosterImage.Image);

            var Images = company.CompanyImages.FirstOrDefault(x => x.PosterStatus == false);
            foreach (var item in company.CompanyImages.Where(x => x.PosterStatus == false))
            {
                DeleteFile(item.Image);
            }
            _context.Companies.Remove(company);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
        private void IsRequired(Company company)
        {
            if (company.Name == null)
            {
                ModelState.AddModelError("Name", "İsim gerekli");
            }
            if (company.Title == null)
            {
                ModelState.AddModelError("Title", "Başlık gerekli");

            }

            if (company.Description == null)
            {
                ModelState.AddModelError("Description", "Açıklama gerekli!");

            }

            if (company.PosterImageFile == null)
            {
                ModelState.AddModelError("PosterImageFile", "Poster şekli ekleyiniz!");

            }

        }
        private void EditIsRequired(Company company)
        {
            if (company.Name == null)
            {
                ModelState.AddModelError("Name", "Name is required");
            }
            if (company.Title == null)
            {
                ModelState.AddModelError("Title", "Title is required");
            }

            if (company.Description == null)
            {
                ModelState.AddModelError("Description", "Description is required");
            }
        }
        private void PosterImageCheck(Company company)
        {
            if (company.PosterImageFile.ContentType != "image/png" && company.PosterImageFile.ContentType != "image/jpeg" && company.PosterImageFile.ContentType != "image/webp")
            {
                ModelState.AddModelError("PosterImageFile", "Image type only (png, jpeg and webp");
            }
            if (company.PosterImageFile.Length > 2097152)
            {
                ModelState.AddModelError("PosterImageFile", "Resim max size is  2 MB");
            }
        }
        private void ImagesCheck(Company company)
        {
            if (company.ImageFiles != null)
            {
                foreach (var image in company.ImageFiles)
                {
                    if (image.ContentType != "image/png" && image.ContentType != "image/jpeg" && image.ContentType != "image/webp")
                    {
                        ModelState.AddModelError("ImageFiles", "Resim formatı yanlış");
                        break;
                    }
                    if (image.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFiles", "Resim max size is  2 MB");
                        break;
                    }
                }
            }

        }

        private void CreateImages(Company company)
        {
            foreach (var image in company.ImageFiles)
            {
                CompanyImages newImage = new CompanyImages
                {
                    PosterStatus = false,
                    Companys = company,
                    Image = FileSave(image),
                };
                _context.CompanyImages.Add(newImage);
            }
        }

        private void CreatePosterImage(Company company)
        {
            CompanyImages Posterimage = new CompanyImages
            {
                PosterStatus = true,
                Companys = company,

                Image = FileSave(company.PosterImageFile),
            };
            _context.CompanyImages.Add(Posterimage);
        }
        private string FileSave(IFormFile ImageFile)
        {
            string image = FileManager.Save(_env.WebRootPath, "uploads/companies", ImageFile);
            return image;
        }
        private void EditImageSave(Company company, Company companyExist)
        {
            foreach (var image in company.ImageFiles)
            {
                if (image.ContentType != "image/png" && image.ContentType != "image/jpeg" && image.ContentType != "image/webp")
                {
                    continue;
                }
                if (image.Length > 41943040)
                {
                    continue;
                }

                CompanyImages newImage = new CompanyImages
                {
                    PosterStatus = false,
                    Image = FileManager.Save(_env.WebRootPath, "uploads/companies", image),

                };
                if (companyExist.CompanyImages == null)
                {
                    companyExist.CompanyImages = new List<CompanyImages>();
                }
                companyExist.CompanyImages.Add(newImage);
            }

        }


        private void EditChange(Company newCompany, Company oldCompany)
        {
            oldCompany.Name = newCompany.Name;
            oldCompany.Title = newCompany.Title;
            oldCompany.Description = newCompany.Description;
            oldCompany.IsHome = newCompany.IsHome;
            oldCompany.ModifiedDate = DateTime.UtcNow.AddHours(4);

        }
        private void SaveContext()
        {
            _context.SaveChanges();
        }
        private void SaveChange(Company company)
        {
            _context.Add(company);
        }
        private void DeleteFile(string image)
        {
            FileManager.Delete(_env.WebRootPath, "uploads/companies", image);
        }
        private void ImagesDelete(Company company, Company companyExist)
        {
            if (company.CompanyImagesIds != null)
            {
                foreach (var item in companyExist.CompanyImages.Where(x => x.PosterStatus == false && !company.CompanyImagesIds.Contains(x.Id)))
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/companies", item.Image);
                }
                companyExist.CompanyImages.RemoveAll(x => x.PosterStatus == false && !company.CompanyImagesIds.Contains(x.Id));

            }
            else
            {
                foreach (var item in companyExist.CompanyImages.Where(x => x.PosterStatus == false))
                {
                    DeleteFile(item.Image);
                }
                companyExist.CompanyImages.RemoveAll(x => x.PosterStatus == false);
            }
        }
        private void EditPosterImageSave(Company company, Company companyExist)
        {
            var posterFile = company.PosterImageFile;
            CompanyImages posterImage = companyExist.CompanyImages.FirstOrDefault(x => x.PosterStatus == true);

            var filename = FileSave(company.PosterImageFile);
            DeleteFile(companyExist.CompanyImages.FirstOrDefault(x => x.PosterStatus == true).Image);
            posterImage.Image = filename;
        }
    }
}
