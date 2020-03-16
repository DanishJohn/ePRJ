using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ePRJ.Models
{
    public class Service
    {
        [DisplayName("ID")]
        public int service_id { get; set; }
        [DisplayName("Customer")]
        [Required]
        public string service_customer { get; set; }
        [Required]
        [DisplayName("Vehicle's ID")]
        public string service_vehicle_id { get; set; }
        [Required]
        [DisplayName("Service's Status")]
        public bool service_status { get; set; }
    }
}