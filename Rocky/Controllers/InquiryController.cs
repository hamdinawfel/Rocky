using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using System.Collections.Generic;

namespace Rocky.Controllers
{
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

        [BindProperty]
        public IEnumerable<InquiryHeader> InquiriesHeaders  { get; set; }

        public IActionResult Index()
        {
            return View();
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
