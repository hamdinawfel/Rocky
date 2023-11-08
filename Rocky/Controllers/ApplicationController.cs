using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using System.Collections.Generic;

namespace Rocky.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly RockyDbContext _db;
        public ApplicationController(RockyDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Application> apps = _db.Application;
            return View(apps);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Application application)
        {
            _db.Application.Add(application);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
