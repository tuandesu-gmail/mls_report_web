using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meliasoft.Models
{
    public class ReportChartModel
    {
        public int Id { get; set; }
        public int ReferenceId { get; set; }
        
        public string Name { get; set; }
        public string Title { get; set; }
        public bool Visible { get; set; }
        public string ChartConfig { get; set; }
        public string ChartDataSample { get; set; }
        public string StoredProcedureName { get; set; }
        //public string CompanyName { get; set; }
        public string Note { get; set; }
    }
}