using Microsoft.AspNetCore.Identity;

namespace TestePrazo.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; }
    }
}
