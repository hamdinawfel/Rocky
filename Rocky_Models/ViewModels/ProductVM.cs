using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Rocky_Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
