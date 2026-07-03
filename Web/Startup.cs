using Commands.Handlers.Transaction.AddDebitTransaction;
using Commands.Services;

using Domain;
using Domain.Interfaces;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.HttpOverrides;

using MudBlazor;
using MudBlazor.Services;

using Persistence.Configuration;
using Persistence.Extensions;
using Persistence.Repositories;

using Queries.Members.Handlers.SearchMembers;

using Web.Extensions;

namespace Web;

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
        var dbConnectionString = Configuration.GetConnectionString("HaSpMan")
            ?? throw new Exception("DB connection string cannot be null");

        services.AddCustomAuthentication(Configuration);
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddHaSpManContext(dbConnectionString);
        DbContextExtensions.MigrateHaSpManContext(dbConnectionString);

        services.Configure<StorageOptions>(Configuration.GetSection("Storage"));
        services.Configure<FinancialYearConfiguration>(Configuration.GetSection("FinancialYear"));

        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IBankAccountRepository, BankAccountRepository>();
        services.AddScoped<IAttachmentStorage, AttachmentStorage>();
        services.AddScoped<IFinancialYearRepository, FinancialYearRepository>();
        services.AddAutoMapper(
            o => o.LicenseKey = "",
            typeof(MapperProfiles.MemberProfile),
            typeof(MapperProfiles.TransactionProfile),
            typeof(Queries.MapperProfiles.MemberProfile),
            typeof(Queries.MapperProfiles.TransactionProfile));

        // Add query and command assemblies to mediatr
        var queryAssembly = typeof(SearchMembersQuery).Assembly;
        var commandAssembly = typeof(AddDebitTransactionCommand).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(new[] { queryAssembly, commandAssembly }));

        // For all the validators, register them with dependency injection as scoped
        AssemblyScanner.FindValidatorsInAssembly(commandAssembly)
            .ForEach(item => services.AddScoped(item.InterfaceType, item.ValidatorType));

        // Add the custom pipeline validation to DI
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddMudServices(cfg =>
        {
            cfg.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
            cfg.SnackbarConfiguration.VisibleStateDuration = 5000;
        });
        services.AddHttpContextAccessor();
        services.AddScoped<IUserAccessor, UserAccessor>();
        services.AddMailingService(Configuration);

        services.AddLocalizationAndConfiguration();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseForwardedHeaders();

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
        app.UseRequestLocalizationWithSupportedCultures();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}