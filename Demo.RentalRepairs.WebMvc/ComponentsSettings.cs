using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Infrastructure.Email;
using Demo.RentalRepairs.Infrastructure.Identity;
using Demo.RentalRepairs.Infrastructure.Repositories;

namespace Demo.RentalRepairs.WebMvc
{
    public class ComponentsSettings
    {
        public IdentityTypeEnum IdentityType { get; set;  }
        public RepositoryTypeEnum RepositoryType { get; set;  }
        public EmailServiceTypeEnum EmailServiceType { get; set;  }
    }
}
