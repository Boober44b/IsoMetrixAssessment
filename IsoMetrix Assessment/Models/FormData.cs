using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IsoMetrix_Assessment.Models
{
    public class FormData
    {
        [Required(ErrorMessage = "Please select a format option")]
        [DisplayName("Format Option")]
        public string FormatOption { get; set; }
        [Required(ErrorMessage = "Please provide an address")]
        public string Address { get; set; }
    }
}