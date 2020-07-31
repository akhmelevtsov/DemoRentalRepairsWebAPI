using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Demo.RentalRepairs.WebMvc.Models
{
    public class TenantEditViewModel
    {
        public PersonContactInfo ContactInfo { get; set; } = new PersonContactInfo();
        public string UnitNumber { get; set; }
        

       
        [Display(Name = "Property")]
        public string SelectedPropertyCode { get; set; }

        public List<SelectListItem> PropertyList { get; set; }

       
        [Display(Name = "Unit")]
        public string SelectedUnitNumber { get; set; }

        public List<SelectListItem> UnitList { get; set; }

    }
}
