using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Data;
using Domain.Interfaces;
using Persistence.Repositories;
using Persistence.Extensions;
using MediatR;
using Commands;
using MudBlazor.Services;
using MudBlazor;
using Queries;
using Web.Extensions;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var dbConnectionString = Configuration.GetConnectionString("HaSpMan");

            services.AddCustomAuthentication(Configuration);

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHaSpManContext(dbConnectionString);
            services.MigrateHaSpManContext(dbConnectionString);
            services.AddSingleton<WeatherForecastService>();
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddAutoMapper(typeof(Commands.MapperProfiles.MemberProfile), typeof(MapperProfiles.MemberProfile), typeof(Queries.MapperProfiles.MemberProfile));
            services.AddMediatR(typeof(AddMemberCommand), typeof(SearchMembersQuery));
            services.AddMudServices(cfg =>
            {
                cfg.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
                cfg.SnackbarConfiguration.VisibleStateDuration = 5000;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
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
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
