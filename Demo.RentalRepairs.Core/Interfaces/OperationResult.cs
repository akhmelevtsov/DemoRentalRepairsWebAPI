using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.Enums;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public class OperationResult
    {
        public bool Succeeded { get; set; }
        public List<Tuple<string, string>> ErrorsValueTuples { get; set; }
        public UserRolesEnum UserRole { get; set; }
    }
}
