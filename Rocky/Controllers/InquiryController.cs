using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky.Extensions;
using Rocky_DataAccess.Repository;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility.Constants;
using System.Collections.Generic;
using System.Linq;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class InquiryController : Controller
    {

        private readonly IInquiryHeaderRepository _inquiryHeaderRepository;
        private readonly IInquiryDetailRepository _inquiryDetailRepository;


        [BindProperty]
        public InquiryVM InquiryVM { get; set; }
        public InquiryController(IInquiryHeaderRepository inquiryHeaderRepository,
                                 IInquiryDetailRepository inquiryDetailRepository)
        {
            _inquiryHeaderRepository = inquiryHeaderRepository;
            _inquiryDetailRepository = inquiryDetailRepository;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Details(int id)
        {
            InquiryVM inquiryVM = new InquiryVM
            {
                InquiryHeader = _inquiryHeaderRepository.FirstOrDefault(x => x.Id == id),
                InquiryDetail = _inquiryDetailRepository.FindAll(x => x.InquiryHeader.Id == id, includeProperties: "Product")
        };
           
            return View(inquiryVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            var shoppingCartList = new List<ShoppingCart>();

            var inquiriesDetails = _inquiryDetailRepository.FindAll(x => x.InquiryHeader.Id == InquiryVM.InquiryHeader.Id);

            foreach(var detail in inquiriesDetails)
            {
                var shoppingCard = new ShoppingCart
                {
                    ProductId = detail.ProductId,
                };
                shoppingCartList.Add(shoppingCard);
            }
            HttpContext.Session.Clear();

            HttpContext.Session.Set(WC.SessionCard, shoppingCartList);
            HttpContext.Session.Set(WC.SessionInquiryId, InquiryVM.InquiryHeader.Id);


            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete()
        {
            var inquiryHeader = _inquiryHeaderRepository.FirstOrDefault(x => x.Id == InquiryVM.InquiryHeader.Id);
            var inquiriesDetail = _inquiryDetailRepository.FindAll(x => x.Id == InquiryVM.InquiryHeader.Id);

            _inquiryDetailRepository.RemoveRange(inquiriesDetail);
            _inquiryHeaderRepository.Remove(inquiryHeader);

            _inquiryDetailRepository.SaveChanges();
            _inquiryDetailRepository.SaveChanges();

            TempData[WC.Success] = "Action completed successfully";

            return RedirectToAction(nameof(Index));
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _inquiryHeaderRepository.FindAll() });
        }

        #endregion
    }
}
