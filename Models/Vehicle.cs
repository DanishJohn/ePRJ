using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ePRJ.Models
{
    public class Vehicle
    {
        [Required]
        [Display(Name = "Vehicle ID")]
        public string vehicle_id { get; set; }
        [Required]
        [Display(Name = "Vehicle Name")]
        public string vehicle_name { get; set; }
        [Required]
        [Display(Name="Brand")]
        public string vehicle_brand { get; set; }
        [Required]
        [Display(Name="Price")]
        public int vehicle_price { get; set; }
        [Required]
        [Display(Name = "Stock")]
        public int vehicle_stock { get; set; }
        [Display(Name = "Image")]
        public string img_path { get; set; }
        [Display(Name = "Vehicle Description")]
        public string vehicle_description { get; set; }
    }
}