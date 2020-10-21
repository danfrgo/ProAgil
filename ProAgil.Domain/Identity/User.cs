using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProAgil.Domain.Identity
{
    // id INT
    public class User : IdentityUser<int> // ao ser IUser e nao so identity, ja tenho acesso a DTO's..-
                                     //... já construidas pelo sistema

    //escrever o resto das dtos

    {
        // campo full name tambem sera criado na tabela users
        [Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; }

        public List<UserRole> UserRoles {get; set;}


    }
}