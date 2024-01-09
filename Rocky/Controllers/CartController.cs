using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Rocky_Models;
using Rocky_Models.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Rocky_DataAccess.Data;
using Rocky_Utility.Constants;
using Rocky.Extensions;
using Rocky_Utility.Configuration.Models;
using Rocky_Utility.Email;
using Rocky_DataAccess.Repository.IRepository;
using System;
using Rocky_DataAccess.Repository;
using Microsoft.AspNetCore.Http;

namespace Rocky.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IInquiryHeaderRepository _inquiryHeaderRepository;
        private readonly IInquiryDetailRepository _inquiryDetailRepository;
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IOptions<EmailSettings> _emailSettings;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }
        public CartController(IProductRepository productRepository,
                              IApplicationUserRepository applicationUserRepository,
                              IInquiryHeaderRepository inquiryHeaderRepository,
                              IInquiryDetailRepository inquiryDetailRepository,
                              IOrderHeaderRepository orderHeaderRepository,
                              IOrderDetailRepository orderDetailRepository,

                              IWebHostEnvironment webHostEnvironment,
                              IEmailSenderService emailSenderService,
                              IOptions<EmailSettings> emailSettings)
        {
            _productRepository = productRepository;
            _applicationUserRepository = applicationUserRepository;
            _inquiryHeaderRepository = inquiryHeaderRepository;
            _inquiryDetailRepository = inquiryDetailRepository;
            _orderHeaderRepository = orderHeaderRepository;
            _orderDetailRepository = orderDetailRepository;

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

            var productsInCart = _productRepository.FindAll(x => productsIds.Contains(x.Id));

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
            var shoppingCartList = new List<ShoppingCart>();

            bool sessionExist = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard) != null &&
                HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard).Count > 0;

            if (sessionExist)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCard);
            }

            var productsIds = shoppingCartList.Select(x => x.ProductId).ToList();

            var products = _productRepository.FindAll(x => productsIds.Contains(x.Id)).ToList();

            var currentUserId = GetCurrentUserId();

            ProductUserVM productUserVM = new ProductUserVM
            {
                ApplicationUser = _applicationUserRepository.FirstOrDefault(x => x.Id == currentUserId),
                Products = products
            };

            return View(productUserVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(ProductUserVM ProductUserVM)
        {
            SendConfirmationEmail();
            UpdateInquiry();
            CreateOrder();
            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
        }

        private string GetCurrentUserId()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier); //var userId = User.FindFirstValue(ClaimTypes.Name)

            return claims.Value;
        }
        private void UpdateInquiry()
        {
            var currentUserId = GetCurrentUserId();

            var inquiryHeader = new InquiryHeader
            {
                ApplicationUserId = currentUserId,
                InquiryDate = DateTime.UtcNow,
                Email = ProductUserVM.ApplicationUser.Email,
                PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                FullName = ProductUserVM.ApplicationUser.FullName,
            };

            _inquiryHeaderRepository.Add(inquiryHeader);
            _inquiryHeaderRepository.SaveChanges();

            var inquiriesDetail = new List<InquiryDetail>();

            foreach (var product in ProductUserVM.Products)
            {
                var inquiryDetail = new InquiryDetail
                {
                    ProductId = product.Id,
                    InquiryHeaderId = inquiryHeader.Id
                };

                inquiriesDetail.Add(inquiryDetail);
            }

            _inquiryDetailRepository.AddRange(inquiriesDetail);
            _inquiryDetailRepository.SaveChanges();
        }

        private void SendConfirmationEmail()
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
        }

        private void CreateOrder()
        {
            var currentUserId = GetCurrentUserId();

            OrderHeader orderHeader = new OrderHeader()
            {
                CreatedByUserId = currentUserId,
                FinalOrderTotal = ProductUserVM.Products.Sum(x => x.Price),
                City = ProductUserVM.ApplicationUser.City,
                StreetAddress = ProductUserVM.ApplicationUser.StreetAddress,
                State = ProductUserVM.ApplicationUser.State,
                PostalCode = ProductUserVM.ApplicationUser.PostalCode,
                FullName = ProductUserVM.ApplicationUser.FullName,
                Email = ProductUserVM.ApplicationUser.Email,
                PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                OrderDate = DateTime.Now,
                OrderStatus = WC.StatusPending
            };
            _orderHeaderRepository.Add(orderHeader);
            _orderHeaderRepository.SaveChanges();


            foreach (var prod in ProductUserVM.Products)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderHeaderId = orderHeader.Id,
                    ProductId = prod.Id
                };
                _orderDetailRepository.Add(orderDetail);

            }

            _orderDetailRepository.SaveChanges();
        }  
    }
}
