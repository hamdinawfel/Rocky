using Microsoft.AspNetCore.Mvc;
using Rocky.Constants;
using Rocky.Data;
using Rocky.Models;
using Rocky.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Rocky.Controllers
{
    public class CartController : Controller
    {
        private readonly RockyDbContext _db;
        public CartController(RockyDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var shoppingCartList = new List<ShoppingCart>();

            bool sessionExist = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard) != null &&
                HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard).Count > 0;

            if (sessionExist)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard);
            }

            var productsIds = shoppingCartList.Select(x => x.ProductId).ToList();

            var productsInCart = _db.Product.Where(x => productsIds.Contains(x.Id)).ToList();

            return View(productsInCart);
        }

        public IActionResult Remove(int productId)
        {
            var shoppingCartList = new List<ShoppingCart>();

            bool sessionExist = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard) != null &&
                HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard).Count > 0;

            if (sessionExist)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard);
            }

            var itemToRemove = shoppingCartList.SingleOrDefault(x => x.ProductId == productId);

            if (itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WC.SessionCard, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }
    }
}
