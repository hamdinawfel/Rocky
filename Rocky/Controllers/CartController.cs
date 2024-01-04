using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky.Utils.Email;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Rocky_DataAccess.Data;
using Rocky_Utility.Constants;
using Rocky.Extensions;

namespace Rocky.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly RockyDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IOptions<EmailSettings> _emailSettings;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }
        public CartController(RockyDbContext db,
                              IWebHostEnvironment webHostEnvironment,
                              IEmailSenderService emailSenderService,
                              IOptions<EmailSettings> emailSettings)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSenderService = emailSenderService;
            _emailSettings = emailSettings;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);  //var userId = User.FindFirstValue(ClaimTypes.Name);


            var shoppingCartList = new List<ShoppingCart>();

            bool sessionExist = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard) != null &&
                HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard).Count > 0;

            if (sessionExist)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard);
            }

            var productsIds = shoppingCartList.Select(x => x.ProductId).ToList();

            var products = _db.Product.Where(x => productsIds.Contains(x.Id)).ToList();

            ProductUserVM productUserVM = new ProductUserVM
            {
                ApplicationUser = _db.ApplicationUser.FirstOrDefault(x => x.Id == claims.Value),
                Products = products
            };

            return View(productUserVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(ProductUserVM ProductUserVM)
        {
            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() +
                "Inquiry.html";

            var subject = "New Inquiry";
            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }
            //Name: { 0}
            //Email: { 1}
            //Phone: { 2}
            //Products: {3}

            StringBuilder productListSB = new StringBuilder();
            foreach (var prod in ProductUserVM.Products)
            {
                productListSB.Append($" - Name: {prod.Name} <span style='font-size:14px;'> (ID: {prod.Id})</span><br />");
            }

            string messageBody = string.Format(HtmlBody,
                ProductUserVM.ApplicationUser.FullName,
                ProductUserVM.ApplicationUser.Email,
                ProductUserVM.ApplicationUser.PhoneNumber,
            productListSB.ToString());



             _emailSenderService.SendEmailAsync(new EmailDto
            {
                Addresses = _emailSettings.Value.EmailAdmin,
                Message = messageBody,
                Subject = subject,
                //Attachments = new List<string> { filePath }
            });

            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
        }
    }
}
