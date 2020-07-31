using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Demo.RentalRepairs.WebMvc.Models
{
    public class WorkerEditViewModel
    {
        public PersonContactInfo ContactInfo { get; set; } = new PersonContactInfo();

    }
}
