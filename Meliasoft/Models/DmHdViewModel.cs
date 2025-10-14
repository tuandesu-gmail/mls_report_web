using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Meliasoft.Models
{
    public class DmHdViewModel
    {
        public int UID { get; set; }

        public string MA_HD_OLD { get; set; }

        [Required]
        [Display(Name = "Số đơn hàng")]
        public string MA_HD { get; set; }

        [Required]
        [Display(Name = "Nội dung")]
        public string TEN_HD { get; set; }

        [Display(Name = "Nội dung tiếng anh")]
        public string TEN_HD_E { get; set; }

        [Display(Name = "Ngày ký")]
        public DateTime NGAY_HD { get; set; }

        [Required]
        [Display(Name = "Vụ việc")]
        public string MA_VV { get; set; }

        [Display(Name = "Tài khoản")]
        public string TK { get; set; }

        [Display(Name = "Nhân viên")]
        public string MA_NV { get; set; }

        [Display(Name = "Khách hàng")]
        public string MA_DT { get; set; }
        
        [Display(Name = "Trị giá")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal TRI_GIA { get; set; }
    }
}