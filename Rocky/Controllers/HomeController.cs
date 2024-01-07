using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rocky.Extensions;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility.Constants;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Rocky.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        private readonly ILogger<HomeController> _logger;

        public HomeController(IProductRepository productRepository,
                              ICategoryRepository categoryRepository,
                              ILogger<HomeController> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var products = _productRepository.FindAll(includeProperties: "Category");
            var categories = _categoryRepository.FindAll();

            var homeVM = new HomeVM
            {
                Products = products,
                Categories = categories
            };

            return View(homeVM);
        }

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            var shoppingCartList = new List<ShoppingCart>();
            if(HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard) != null &&
                HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard);
            }

            shoppingCartList.Add(new ShoppingCart { ProductId = id});
            HttpContext.Session.Set(WC.SessionCard, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int? id)
        {
            var shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard) != null &&
                HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard);
            }

            var product = _productRepository.FirstOrDefault(x => x.Id == id, includeProperties:"Category");

            var detailsVM = new DetailsVM
            {
                Product = product,
                ExistsCart = shoppingCartList.Any(x => x.ProductId == id)
            };

            return View(detailsVM);
        }

        public IActionResult RemoveFromCart(int id)
        {
            var shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard) != null &&
                HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard).Count() > 0)
            {
                TempData[WC.Success] = "Item add to cart successfully";
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard);
            }

            var itemToRemove = shoppingCartList.SingleOrDefault(x => x.ProductId == id);

            if(itemToRemove != null)
            {
                TempData[WC.Success] = "Item removed from cart successfully";
                shoppingCartList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WC.SessionCard, shoppingCartList);
            return RedirectToAction(nameof(Index));
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
