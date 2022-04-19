using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace IdentityServer.Models.System
{
    public class PrivateUserVM
    {
        public PrivateUserVM()
        {
            Claims = new List<IdentityUserClaim<string>>();
            Roles = new List<string>();
        }

        public string Id { get; set; }

        public string FirstName { get; set; }

        public string FullName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Email { get; set; }

        public string ImageURL { get; set; }

        public bool IsInClass { get; set; }

        public List<IdentityUserClaim<string>> Claims { get; private set; }

        public List<string> Roles { get; set; }
    }
}
