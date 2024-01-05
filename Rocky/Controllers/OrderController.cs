using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Repository.IRepository;

namespace Rocky.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
