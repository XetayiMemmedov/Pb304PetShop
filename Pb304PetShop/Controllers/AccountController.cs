using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Pb304PetShop.DataContext.Entities;
using Pb304PetShop.Models;

namespace Pb304PetShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMailService _mailService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMailService mailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mailService = mailService;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task <IActionResult> Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = new AppUser
            {
                UserName = register.UserName,
                FullName = register.FullName,
                Email = register.Email,
            };
            var result = await _userManager.CreateAsync(user, register.Password);
            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(register); 

            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmlink = Url.Action("ConfirmEmail", "Account", new {userid = user.Id, token = token}, protocol: Request.Scheme);
            _mailService.SendMail(new Mail
            {
                Email = user.Email,
                Subject = "Email Confirmation",
                TextBody = $"Please confirm your account by clicking this link: {confirmlink}"
            });
            return RedirectToAction(nameof(Login));

        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View(nameof(Login));
            }

            return View("Error");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existUser = await _userManager.FindByNameAsync(login.UserName);
            if (existUser == null)
            {
                ModelState.AddModelError("", "Username pr password is not correct");
                return View();
            }
            if (!await _userManager.IsEmailConfirmedAsync(existUser))
            {
                ModelState.AddModelError("", "Please confirm your email before logging in.");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(existUser, login.Password, login.RememberMe, true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username pr password is not correct");
                return View();
            }
            if(login.ReturnUrl !=null)
            {
                return Redirect(login.ReturnUrl);
            }
            return RedirectToAction("Index", "Shop");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePassword)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(changePassword);
            }
            return RedirectToAction(nameof(Login));

        }
        //public class AzerbaijaniIdentityErrorDescriber : IdentityErrorDescriber
        //{
        //    private readonly IStringLocalizer<AzerbaijaniIdentityErrorDescriber> _localizer;

        //    public AzerbaijaniIdentityErrorDescriber(IStringLocalizer<AzerbaijaniIdentityErrorDescriber> localizer)
        //    {
        //        _localizer = localizer;
        //    }

        //    public override IdentityError PasswordTooShort(int length)
        //    {
        //        return new IdentityError
        //        {
        //            Code = nameof(PasswordTooShort),
        //            Description = string.Format(_localizer["PasswordTooShort"], length)
        //        };
        //    }

        //    public override IdentityError DuplicateUserName(string userName)
        //    {
        //        return new IdentityError
        //        {
        //            Code = nameof(DuplicateUserName),
        //            Description = string.Format(_localizer["DuplicateUserName"], userName)
        //        };
        //    }

        //}

        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPassword)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(forgotPassword.Email);
            if (user == null)
            {
                return RedirectToAction(nameof(ForgotPassword));
            }
            var email = forgotPassword.Email;
            var resettoken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetlink = Url.Action("ResetPassword", "Account", new { resettoken, email }, Request.Scheme, Request.Host.ToString());
            _mailService.SendMail(new Mail { Email = forgotPassword.Email, Subject ="Reset Password", TextBody = resetlink });
            return RedirectToAction(nameof(Login));

        }

        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPassword, string email)
        {
            if(!ModelState.IsValid)
            {
                return View(resetPassword);
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.ResetPasswordAsync(user, resetPassword.ResetToken, resetPassword.NewPassword);
            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(resetPassword);
            }
            return RedirectToAction(nameof(Login));
        }

    }
}
