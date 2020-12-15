
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Finportal.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Finportal.Helper;
using Finportal.Models;
using Finportal.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Finportal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationsDbContext>(options =>
                options.UseNpgsql(
                    DataHelper.GetConnectionString(Configuration)));

            services.AddIdentity<FPUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationsDbContext>();
           

            //Register email service 
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));//this line takes mailsetting config from json
            services.AddTransient<IEmailSender, EmailService>();//Uses the IOptions<MailSettings> instance

            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddScoped<INotificationService, BasicNotificationService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Lobby}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
