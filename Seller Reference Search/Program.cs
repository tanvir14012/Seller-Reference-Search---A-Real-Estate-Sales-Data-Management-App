using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;
using Seller_Reference_Search.Extensions;
using Seller_Reference_Search.Infrastructure;
using Seller_Reference_Search.Infrastructure.Data;
using Seller_Reference_Search.Infrastructure.Data.Models;
using Seller_Reference_Search.Infrastructure.Interfaces;
using Seller_Reference_Search.Models.Utility;
using Seller_Reference_Search.Services;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

builder.Services.AddAntiforgery(options => 
    options.HeaderName = "X-CSRF-TOKEN");

// Db context
var dbConnString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(dbConnString));

builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);

})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();


// Configure cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/account/logout";
    options.AccessDeniedPath = "/";
    // Make cookie unaccessible through client-side script
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;

    options.Events.OnRedirectToLogin = context =>
    {
        // Customize how you handle the redirect to the login page
        context.HttpContext.Response.Redirect("/login");
        return Task.CompletedTask;
    };
});


// Add default authorization policy requiring authenticated users
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});


builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ExcelService>();
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<QueuedHostedService>();

// Add the custom route transformer
builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap.Add("slugify", typeof(SlugifyParameterTransformer));
});


var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

using (var scope = app.Services.CreateScope())
{
    var scopedProvider = scope.ServiceProvider;
    try
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        var dbContext = scopedProvider.GetRequiredService<AppDbContext>();
        var roleMgr = scopedProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userMgr = scopedProvider.GetRequiredService<UserManager<AppUser>>();
        await AppDbContextSeed.SeedAsync(dbContext, builder.Configuration, logger, roleMgr, userMgr);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

//Transform URLs
app.UseLowercaseUrls();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
   name: "login",
   pattern: "login",
   defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
   name: "logout",
   pattern: "logout",
   defaults: new { controller = "Account", action = "Logout" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller:slugify=Home}/{action:slugify=Index}/{id?}");

app.Run();
