using Microsoft.AspNetCore.Identity;

namespace uniqilo.Models
{
    public class User:IdentityUser
    {
        public string FulllNamme { get; set; }
        public string ProfilImage { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
