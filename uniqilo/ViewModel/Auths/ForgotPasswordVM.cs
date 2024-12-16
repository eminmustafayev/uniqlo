using System.ComponentModel.DataAnnotations;

namespace uniqilo.ViewModel.Auths
{
    public class ForgotPasswordVM
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
