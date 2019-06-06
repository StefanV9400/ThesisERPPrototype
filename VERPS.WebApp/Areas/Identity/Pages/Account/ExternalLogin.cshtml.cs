using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ExactOnline.CustomAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VERPS.WebApp.Database;
using VERPS.WebApp.Database.Models;
using VERPS.WebApp.Helpers;

namespace VERPS.WebApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly VERPSDBContext _context;

        public ExternalLoginModel(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<ExternalLoginModel> logger,
            VERPSDBContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Username { get; set; }
            public string CompanyName { get; set; }
            public string ProviderName { get; set; }
            [Display(Name = "Leverancier")]
            public string SupplierId { get; set; }
            [Display(Name = "Administratie")]
            public int DivisionId { get; set; }
            public string PaymentConditionCode { get; set; }
            public Guid UserId { get; set; }
            public List<SelectListItem> Suppliers { get; set; }
            public List<SelectListItem> Divisions { get; set; }
            public List<SelectListItem> PaymentConditions { get; set; }
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {

                // Store new retrieved token

                // Check if already exists
                var user = _context.Users
                    .Include(nameof(VERPS.WebApp.Database.Models.User.ExactToken))
                    .FirstOrDefault(x => x.UserName == info.ProviderKey);

                if (user != null)
                {
                    var token = _context.ExactTokens
                        .FirstOrDefault(x => x.Id == user.ExactToken.Id);
                    token.Token = info.AuthenticationTokens.FirstOrDefault(x => x.Name == "access_token").Value;
                    token.RefreshToken = info.AuthenticationTokens.FirstOrDefault(x => x.Name == "refresh_token").Value;
                    token.RefreshTime = DateTime.Parse(info.AuthenticationTokens.FirstOrDefault(x => x.Name == "expires_at").Value);
                    _context.SaveChanges();
                }
                else
                {
                    await _signInManager.SignOutAsync();
                }



                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                _logger.LogInformation($"Not existing account, will create new account of user {info.Principal.Identity.Name} on provider {info.LoginProvider} ");
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                LoginProvider = info.LoginProvider;

                // Set Exact things
                var exactHelper = new ExactHelper();
                ExactOnlineConnect.data.AccessToken = info.AuthenticationTokens.FirstOrDefault(x => x.Name == "access_token").Value;
                ExactOnlineConnect.data.Context = _context;
                var me = await ExactOnlineConnect.data.GetMe();
                ExactOnlineConnect.data.CurrentDivision = me.CurrentDivision;

                // Data
                var suppliers = await ExactOnlineConnect.data.GetSuppliers(me.CurrentDivision);
                var divisions = await ExactOnlineConnect.data.GetDivisions(me.CurrentDivision);
                var paymentConditions = await ExactOnlineConnect.data.GetPaymentConditions(me.CurrentDivision);

                // SelectLists
                var sups = exactHelper.SuppliersToSelectList(suppliers);
                var divs = divisions != null ? exactHelper.DivisionsToSelectList(divisions) : new List<SelectListItem>();
                var paycs = exactHelper.PaymentConditionsToSelectList(paymentConditions);

                // Model
                Input = new InputModel
                {
                    Suppliers = sups,
                    Divisions = divs,
                    DivisionId = me.CurrentDivision,
                    PaymentConditions = paycs,
                    ProviderName = info.LoginProvider,
                    UserId = me.UserID,
                    Username = me.UserName,
                };

                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                var exactToken = new ExactToken
                {
                    Token = info.AuthenticationTokens.FirstOrDefault(x => x.Name == "access_token").Value,
                    RefreshToken = info.AuthenticationTokens.FirstOrDefault(x => x.Name == "refresh_token").Value,
                    RefreshTime = DateTime.Parse(info.AuthenticationTokens.FirstOrDefault(x => x.Name == "expires_at").Value),
                };

                // Set configuration
                var config = new ExactConfiguration()
                {
                    Created = DateTime.Now,
                    DivsionId = Input.DivisionId,
                    SupplierId = Input.SupplierId,
                    Modified = DateTime.Now,
                    UserId = Input.UserId.ToString(),
                    BuyerId = Input.UserId,
                    PaymentConditionId = Input.PaymentConditionCode,
                };

                //if (Input.Provider == ProviderEnum.ExactOnline)
                //{
                //    user.IsExact = true;
                //}
                //var result = await _userManager.CreateAsync(user, Input.Password);
                //if (result.Succeeded)
                //{
                //    _logger.LogInformation("User created a new account with password.");

                //    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //    var callbackUrl = Url.Page(
                //        "/Account/ConfirmEmail",
                //        pageHandler: null,
                //        values: new { userId = user.Id, code = code },
                //        protocol: Request.Scheme);

                //    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                //        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                //    await _signInManager.SignInAsync(user, isPersistent: false);
                //    return LocalRedirect(returnUrl);
                //}
                //foreach (var error in result.Errors)
                //{
                //    ModelState.AddModelError(string.Empty, error.Description);
                //}
                

                // Create Uer
                var user = new User
                {
                    UserName = Input.Username,
                    Email = Input.Username,
                    CompanyName = Input.CompanyName,
                    Token = info.ProviderKey,
                    HasConfig = true,
                    UserID = Input.UserId.ToString(),
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    ExactConfiguration = _context.ExactConfigurations.Add(config).Entity,
                    IsExact = true,
                    ExactToken = exactToken,
                   
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation($"User: ({user.Id}) created an account using {info.LoginProvider} provider.");
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
