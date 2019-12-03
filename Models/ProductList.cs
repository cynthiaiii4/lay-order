using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace sys.Models
{
    public class ProductList
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        
        [Required(ErrorMessage = "{0}必填")]
        [Display(Name = "產品類別ID")]
        public int PCid { get; set; }
        [ForeignKey("PCid")]
        public virtual ProductCategory ProductCategory { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [Display(Name = "產品名稱")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [Display(Name = "產品介紹")]
        public string Description { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [Display(Name = "產品價格")]
        public int Price { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [Display(Name = "產品圖片")]
        public string Img { get; set; }

        
        [Display(Name = "附餐選項一")]
        public string Sides1 { get; set; }
        [Display(Name = "附餐選項二")]
        public string Sides2 { get; set; }

        [Display(Name = "附餐選項三")]
        public string Sides3 { get; set; }

        [Display(Name = "附餐選項四")]
        public string Sides4 { get; set; }

        public  ICollection<OrderDetail> OrderDetails { get; set; }
    }
}