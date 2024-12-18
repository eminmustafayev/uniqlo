using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Net.Mail;
using System.Net;
using uniqilo.Models;
using uniqilo.ViewModel.Auths;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.Options;
using uniqilo.Helpers;

namespace uniqilo.Controllers
{
    public class AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IOptions<SmtpOptions> options) : Controller
    {
        readonly SmtpOptions _smtp = options.Value;
        private string? returnUrl;

        bool isAuthenticaded => User.Identity?.IsAuthenticated ?? false;

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Register(UserCreateVM vm)
        {
            if(isAuthenticaded) return RedirectToAction("Index" , "Home");
            if (!ModelState.IsValid)
                return View();
            User user = new User
            {
                Email = vm.Email,
                FulllNamme = vm.Fullname,
                UserName = vm.Username,
                ProfilImage = "photo.jpg"
            };
            var result = await userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            return View();
        }
    
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]


        public async Task<IActionResult> Login(LoginVM vm, string? returnUrl)
        {
            if (!ModelState.IsValid) return View();
            User? user = null;
            if (vm.UserNameOrEmail.Contains("@"))
            {
                user = await userManager.FindByEmailAsync(vm.UserNameOrEmail);
            }
            else
            {
                user = await userManager.FindByNameAsync(vm.UserNameOrEmail);
            }
            if (user is null)
            {
                ModelState.AddModelError("", "Username or password is wrong!");
                return View();
            }
            var result = await signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, true);
            if (!result.Succeeded)
            {
                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Username or password is wrong");
                }
                if (!result.IsLockedOut)
                {
                    ModelState.AddModelError("", "wait until" + user.LockoutEnd!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                return View();
            }
            if (string.IsNullOrEmpty(returnUrl))
            {
                if (await userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Index", new { Controller = "Dashboard", Area = "Admin" });
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }


        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("E-poct tapilmadi.");
            }
            var opt = options.Value;

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action(
                action: "ResetPassword",
                controller: "Account",
                values: new { token, email = user.Email },
                protocol: "https");

            SmtpClient smtp = new SmtpClient
            {
            
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential("eminem-bp215@code.edu.az", "hnbf ywfl xrtb zipp")
            };

            MailMessage msg = new MailMessage
            {
                From = new MailAddress("eminem - bp215@code.edu.az", "Emin Agalarov"),
                Subject = "Reset Password",
                Body = $"<p>Kodunuzu berpa etmek ucun <a href='{resetLink}'>bu linke</a> klikleyin</p>",
                IsBodyHtml = true
            };
            msg.To.Add(email);

            smtp.Send(msg);

            return Ok("Emaile gonderildi");
        }
    }
}
