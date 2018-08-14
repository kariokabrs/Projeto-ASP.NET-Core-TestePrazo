using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestePrazo.Data;
using TestePrazo.Models;
using TestePrazo.Services;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace TestePrazo
{
    public class Startup
    {
       
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDataProtection().SetDefaultKeyLifetime(TimeSpan.FromDays(30));
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"c:\"));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

            // Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Signin settings
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); //default 30
                options.Lockout.MaxFailedAccessAttempts = 3; // default 10
                options.Lockout.AllowedForNewUsers = true;
 
                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.Name = "YourAppCookieName";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });
            // end Identity

            // policy e Roles
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequerAdmin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("SoBanBanBan", policy => policy.RequireRole("Admin", "GodUser", "BackupAdmin"));
                //options.AddPolicy("AtLeast21", policy => policy.Requirements.Add(new IdadeRequirement(21)));
            });

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ITarefaService, TarefaService>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseExceptionHandler("/Home/Error");
                //app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            Task.Run(() => CreateUserRoles(services));
        }

        private IEnumerable<Claim> GetClaimFromCookie(HttpContext httpContext, string cookieName, string cookieSchema)
        {
            var opt = httpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var cookie = opt.CurrentValue.CookieManager.GetRequestCookie(httpContext, cookieName);

            if (!string.IsNullOrEmpty(cookie))
            {
                var dataProtector = opt.CurrentValue.DataProtectionProvider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", cookieSchema, "v2");

                var ticketDataFormat = new TicketDataFormat(dataProtector);
                var ticket = ticketDataFormat.Unprotect(cookie);
                return ticket.Principal.Claims;
            }
            return null;
        }

        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole> RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            UserManager<ApplicationUser> UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            IdentityResult roleResult;

            Boolean checkRoleAdmin = await RoleManager.RoleExistsAsync("Admin");
            Boolean checkRoleUsuarioBasico = await RoleManager.RoleExistsAsync("UsuarioBasico");
            if (!checkRoleAdmin)
            {
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!checkRoleUsuarioBasico)
            {
                roleResult = await RoleManager.CreateAsync(new IdentityRole("UsuarioBasico"));
            }

            ApplicationUser user = await UserManager.FindByEmailAsync("admin@admin.com");
            if (user == null)
            {
                ApplicationUser novoUsuario = new ApplicationUser { Nome = "admin", UserName = "admin", Email = "admin@admin.com" };
                await UserManager.CreateAsync(user, "admin");
            }
            ApplicationUser userRole = new ApplicationUser();
            await UserManager.AddToRoleAsync(userRole, "Admin");
        }
    }

    public class IdadeRequirement : IAuthorizationRequirement
    {
        public int IdadeMinima { get; private set; }

        public IdadeRequirement(int idadeMinima)
        {
            IdadeMinima = idadeMinima;
        }
    }
}
