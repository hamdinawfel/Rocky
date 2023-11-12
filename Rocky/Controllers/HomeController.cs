using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using System.Diagnostics;
using System.Linq;

namespace Rocky.Controllers
{
    public class HomeController : Controller
    {
        private readonly RockyDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(RockyDbContext db,
                              ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var homeVM = new HomeVM
            {
                Products = _db.Product.Include(x => x.Category),
                Categories = _db.Category
            };

            return View(homeVM);
        }

        public IActionResult Details(int? id)
        {
            var detailsVM = new DetailsVM
            {
                Product = _db.Product.Include(x => x.Category).Where(x => x.Id == id).FirstOrDefault(),
                ExistsCart = false
            };

            return View(detailsVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
