using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ProAgil.Domain.Identity
{
    public class Role : IdentityRole<int> // id INT
    {
        public List<UserRole> UserRoles {get; set;}

    }
}