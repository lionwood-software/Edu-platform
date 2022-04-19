using Newtonsoft.Json;
using RouterApi.ApiClients.Identity.Enums;
using System.Collections.Generic;

namespace RouterApi.ApiClients.Identity.Models.Users
{
    public class UserVM
    {
        public UserVM()
        {
            Relations = new List<GridRelationVM>();
        }

        public string Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string FullName { get; set; }

        public bool IsInClass { get; set; }

        public string Code { get; set; }

        public UserType Type { get; set; }

        public List<string> Subjects { get; set; }

        [JsonProperty]
        public List<GridRelationVM> Relations { get; private set; }

        public string SchoolUId { get; set; }
    }
}
