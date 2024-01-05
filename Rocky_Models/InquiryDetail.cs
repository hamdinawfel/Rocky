using Rocky_Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_Models
{
    public class InquiryDetail
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("InquiryHeaderId")]
        public int InquiryHeaderId { get; set; }

        public InquiryHeader InquiryHeader { get; set; }

        [ForeignKey("ProductId")]
        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}
