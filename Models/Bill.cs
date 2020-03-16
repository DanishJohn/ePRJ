using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ePRJ.Models
{
    public class Bill
    {
        [DisplayName("Bill ID")]
        public int order_id { get; set; }
        [Required]
        [DisplayName("Vehicle ID")]
        public string vehicle_id { get; set; }
        [Required]
        [DisplayName("Vehicle Name")]
        public string vehicle_name { get; set; }
        [Required]
        [DisplayName("Vehicle Brand")]
        public string vehicle_brand { get; set; }
        [Required]
        [DisplayName("Vehicle Price")]
        public Nullable<int> vehicle_price { get; set; }
        [Required]
        [DisplayName("Order Quantity")]
        public Nullable<int> quantity { get; set; }
        [Required]
        [DisplayName("Customer Email")]
        public string user_email { get; set; }
    }
}