using System.Collections.Generic;
using System;

namespace PromoCodeFactory.WebHost.Models
{
    public class EmployeeUpdate
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public List<Guid> RoleIds { get; set; }

        public int AppliedPromocodesCount { get; set; }
    }
}
