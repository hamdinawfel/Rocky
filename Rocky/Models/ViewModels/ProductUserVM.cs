using System.Collections.Generic;

namespace Rocky.Models.ViewModels
{
    public class ProductUserVM
    {
        public ProductUserVM()
        {
            Products = new List<Product>();
        }

        public ApplicationUser ApplicationUser { get; set; }
        public IList<Product> Products { get; set; }
    }
}
