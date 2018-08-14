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

            services.AddDataProtection().SetDefaultKeyLifetime(TimeSpan.FromDays(30)); // nao pode ser menos que uma semana. 
            // pegar minmha chave encriptografada e ver o tpo de algoritimo usado na segurança;
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"c:\"));

            //services.AddScoped<IDataProtector>();

            // EF DbContext
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
                options.Password.RequiredLength = 8;
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
                options.AddPolicy("RequerAdmin", policy => policy.RequireRole("Admin")); // o nome do Role conforme BD
                options.AddPolicy("SoBanBanBan", policy => policy.RequireRole("Admin", "GodUser", "BackupAdmin"));
                // options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber"));
                // options.AddPolicy("Founders", policy => policy.RequireClaim("EmployeeNumber", "1", "2", "3", "4", "5"));
                options.AddPolicy("AtLeast21", policy => policy.Requirements.Add(new MinimumAgeRequirement(21)));
            });

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
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

        // decrypt cookie
        private IEnumerable<Claim> GetClaimFromCookie(HttpContext httpContext, string cookieName, string cookieSchema)
        {
            // Get the encrypted cookie value
            var opt = httpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var cookie = opt.CurrentValue.CookieManager.GetRequestCookie(httpContext, cookieName);

            // Decrypt if found
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
                //criar o role Admin
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!checkRoleUsuarioBasico)
            {
                //criar o role UsuarioBasico
                roleResult = await RoleManager.CreateAsync(new IdentityRole("UsuarioBasico"));
            }
            //Assign Admin role to the main User here we have given our newly registered 
            //login id for Admin management
            ApplicationUser user = await UserManager.FindByEmailAsync("kariokabrs@msn.com");
            var User = new ApplicationUser();
            await UserManager.AddToRoleAsync(user, "Admin");
        }
    }
    // custom policy
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public int MinimumAge { get; private set; }

        public MinimumAgeRequirement(int minimumAge)
        {
            MinimumAge = minimumAge;
        }
    }
}
