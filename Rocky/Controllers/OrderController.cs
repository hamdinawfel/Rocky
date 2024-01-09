using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Repository.IRepository;

namespace Rocky.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        public OrderController(IOrderHeaderRepository orderHeaderRepository,
                               IOrderDetailRepository orderDetailRepository)
        {
            _orderHeaderRepository = orderHeaderRepository;
            _orderDetailRepository = orderDetailRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
