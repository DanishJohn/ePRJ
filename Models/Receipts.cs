using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ePRJ.Models
{
    public class Receipts
    {
        [DisplayName("ID")]
        public int receipt_id { get; set; }
        [DisplayName("Vehicle ID")]
        public string vehicle_id { get; set; }
        [DisplayName("Status")]
        public bool receipt_status { get; set; }
    }
}