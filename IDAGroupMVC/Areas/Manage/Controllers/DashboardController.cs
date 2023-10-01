using IDAGroupMVC.Areas.Manage.ViewModels;
using IDAGroupMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDAGroupMVC.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin , Admin")]
    public class DashboardController : Controller
    {
        private readonly DataContext _context;

        public DashboardController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var today = DateTime.UtcNow.AddHours(4).Day;
            var month = DateTime.UtcNow.AddHours(4).Month;

            DashboardViewModel dashVM = new DashboardViewModel
            {
                CompanyCount = _context.Companies.Count(),
         

            };
            return View(dashVM);
        }
    }
}
