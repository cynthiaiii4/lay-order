using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace sys.Models
{
    public class OrderDetail
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

       
        [Required(ErrorMessage = "{0}必填")]
        [Display(Name = "訂單編號")]
        public int Oid { get; set; }
        [ForeignKey("Oid")]
        public virtual  Order order { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [Display(Name = "產品ID")]
        public int Pid { get; set; }
        [ForeignKey("Pid")]
        public virtual ProductList ProductList { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [Display(Name = "附餐選項")]
        public string Options { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [Display(Name = "訂購數量")]
        public int Qty { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [Display(Name = "單品狀態")]
        public string Status { get; set; }
    }
}