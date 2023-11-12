using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        [Range(1, int.MaxValue)]
        public int Price { get; set; }

        [Display(Name ="Category Type")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}
