using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Employee : BaseEntity
    {
        public Employee() { }

        public Employee(string firstName, string lastName, string email, int appliedPromocodesCount, List<Role> roles)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            AppliedPromocodesCount = appliedPromocodesCount;
            Roles = roles;
        }

        public Employee(Guid id, string firstName, string lastName, string email, int appliedPromocodesCount, List<Role> roles)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            AppliedPromocodesCount = appliedPromocodesCount;
            Roles = roles;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }

        public List<Role> Roles { get; set; }

        public int AppliedPromocodesCount { get; set; }
    }
}