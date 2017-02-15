using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using HotelApp.Models.DataModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HotelApp.Models;
using Microsoft.AspNetCore.Identity;

namespace HotelApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Добавляем сервисы для EntityFramework Core 
            services.AddDbContext<DataContext>(optionBuilder =>
            optionBuilder.UseSqlServer(Configuration.GetConnectionString("DBConnection")));

            //Сервисы для Identity
            services.AddIdentity<HotelUser, IdentityRole>().AddEntityFrameworkStores<DataContext>();

            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();
            //Аутентификация на основе куки
            app.UseIdentity();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //Инициализация начальных данных
            SetInitialData(app.ApplicationServices).Wait();
        }
        public async Task SetInitialData(IServiceProvider serviceProvider)
        {
            //Определяем менеджеры Identity для работы с пользователями и ролями
            UserManager<HotelUser> userManger = serviceProvider.GetRequiredService<UserManager<HotelUser>>();
            RoleManager<IdentityRole> roleManger = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string guestUserName = Configuration.GetSection("InitialUsernames:GuestUsername").Value;
            string sysadminUserName = Configuration.GetSection("InitialUsernames:SysAdminUsername").Value;
            string administratorUserName = Configuration.GetSection("InitialUsernames:AdminUsername").Value;
            string guestPassword = Configuration.GetSection("InitialPasswords:GuestPassword").Value;
            string sysadminPassword = Configuration.GetSection("InitialPasswords:SysAdminPassword").Value;
            string administratorPassword = Configuration.GetSection("InitialPasswords:AdminPassword").Value;
            //Создаем роли в базе данных ролей
            if (await roleManger.FindByIdAsync("sysadmin") == null)
                await roleManger.CreateAsync(new IdentityRole("sysadmin"));

            if (await roleManger.FindByIdAsync("user") == null)
                await roleManger.CreateAsync(new IdentityRole("user"));

            if (await roleManger.FindByIdAsync("administrator") == null)
                await roleManger.CreateAsync(new IdentityRole("administrator"));

            //Добавляем начальных пользователей в базу данных
            if (await userManger.FindByNameAsync(guestUserName) == null)
            {
                HotelUser guest = new HotelUser { UserName = guestUserName, Name = "guestName" , SurName= "guestSurName" };
                var result = await userManger.CreateAsync(guest, guestPassword);
                if (result.Succeeded)
                    await userManger.AddToRoleAsync(guest,"user");
            }
            if (await userManger.FindByNameAsync(sysadminUserName) == null)
            {
                HotelUser hotelSysAdministrator = new HotelUser { UserName = sysadminUserName , Name = "sysAdminName", SurName = "sysAdmnSurName" };
                var result = await userManger.CreateAsync(hotelSysAdministrator, sysadminPassword);
                if (result.Succeeded)
                    await userManger.AddToRoleAsync(hotelSysAdministrator, "sysadmin");
            }
            if (await userManger.FindByNameAsync(administratorUserName) == null)
            {
                HotelUser hotelAdministrator = new HotelUser { UserName = administratorUserName, Name = "adminName", SurName = "admnSurName" };
                var result = await userManger.CreateAsync(hotelAdministrator, administratorPassword);
                if (result.Succeeded)
                    await userManger.AddToRoleAsync(hotelAdministrator, "administrator");
            }
        }
    }
}
