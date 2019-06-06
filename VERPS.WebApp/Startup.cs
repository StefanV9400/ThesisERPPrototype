using ExactOnline.Models;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web;
using VERPS.WebApp.Database;
using VERPS.WebApp.Database.Models;
using static ExactOnline.CustomAuth.ExactOnlineConnect;
//using VERPS.WebApp.Services;

namespace VERPS.WebApp
{
    public class Startup
    {
        private readonly ILogger _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }
        public object Globals { get; private set; }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.Configure<RazorViewEngineOptions>(options =>
            {
                // Areas
                options.AreaViewLocationFormats.Clear();
                options.AreaViewLocationFormats.Add("/MyAreas/{2}/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/MyAreas/{2}/Views/Shared/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });

            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("nl"),
                        new CultureInfo("en"),
                    };

                    opts.DefaultRequestCulture = new RequestCulture("nl");
                    // Formatting numbers, dates, etc.
                    opts.SupportedCultures = supportedCultures;
                    // UI strings that we have localized.
                    opts.SupportedUICultures = supportedCultures;
                });

            services.AddDbContext<VERPSDBContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection"),
                    x => x.MigrationsAssembly("VERPS.WebApp.Database")));

            services.AddIdentity<User, IdentityRole>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<VERPSDBContext>()
                .AddDefaultTokenProviders();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddLocalization(options => options.ResourcesPath = "Areas/ExactOnline/Resources");

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "ExactOnline";
            })
            .AddCookie()
            .AddOAuth("ExactOnline", options =>
            {
                options.ClientId = Configuration["ExactOnline:ClientId"];
                options.ClientSecret = Configuration["ExactOnline:ClientSecret"];
                options.CallbackPath = new PathString("/Home/Index");

                options.AuthorizationEndpoint = "https://start.exactonline.nl/api/oauth2/auth";
                options.TokenEndpoint = "https://start.exactonline.nl/api/oauth2/token ";
                options.UserInformationEndpoint = "https://start.exactonline.nl/api/v1/current/Me";

                options.SaveTokens = true;

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        // Set cookies
                        var cookieOptions = new CookieOptions()
                        {
                            Expires = DateTime.Now.AddMinutes(10),
                        };

                        // Response into an object
                        var json = await response.Content.ReadAsStringAsync();
                        var converted = JsonConvert.DeserializeObject<dynamic>(json);
                        var jString = converted.d["results"][0];

                        var user = Convert.ToString(jString);
                        var newUser = (ExactOnlineUser)JsonConvert.DeserializeObject<ExactOnlineUser>(user);

                        context.Identity.AddClaim(new Claim(ClaimTypes.Name, newUser.UserName));
                        context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, newUser.UserName));
                        context.Identity.AddClaim(new Claim("urn:exactonline:fullname", newUser.FullName));
                        context.Identity.AddClaim(new Claim("urn:exactonline:currentdivision", newUser.CurrentDivision.ToString()));
                        context.Identity.AddClaim(new Claim("urn:exactonline:email", newUser.Email));

                        context.RunClaimActions();
                    }
                };
            });
            _logger.LogInformation("Added TodoRepository to services");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                _logger.LogInformation("In Development environment");
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "ExactOnline",
                  template: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
