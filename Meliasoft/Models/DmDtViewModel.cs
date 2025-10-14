using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Meliasoft.Models
{
    public class DmDtViewModel
    {
        public int UID { get; set; }

        public string MA_DT_OLD { get; set; }

        [Required]
        [Display(Name = "Nhóm đối tượng")]
        public string MA_NH_DT { get; set; }

        [Required]
        [Display(Name = "Khách hàng")]
        public string MA_DT { get; set; }

        [Required]
        [Display(Name = "Tên khách hàng")]
        public string TEN_DT { get; set; }

        [Display(Name = "Tên tiếng anh")]
        public string TEN_DT_E { get; set; }

        [Required]
        [Display(Name = "Tuyến đường")]
        public string MA_TD { get; set; }

        [Display(Name = "Địa chỉ")]
        public string DIA_CHI { get; set; }

        [Display(Name = "Mã số thuế")]
        public string MA_SO_VAT { get; set; }

        [Display(Name = "Đại diện")]
        public string DAI_DIEN { get; set; }

        [Display(Name = "Điện thoại")]
        public string DIEN_THOAI { get; set; }
        
        [Display(Name = "Fax")]
        public string FAX { get; set; }

        [Display(Name = "Email")]
        public string EMAIL { get; set; }

        [Display(Name = "Số tài khoản")]
        public string SO_TK_NH { get; set; }

        [Display(Name = "Thành phố")]
        public string TEN_NH { get; set; }

        [Display(Name = "Tên ngân hàng")]
        public string TP_NH { get; set; }
    }
}
