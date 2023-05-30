using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Services;
using dotnet_react_xml_generator.Data;
using dotnet_react_xml_generator.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();

// add services to DI container
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    //Console.WriteLine(connectionString);

    var services = builder.Services;
    services.AddCors();
    services.AddControllers();

    services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlite(connectionString));

    // configure strongly typed settings object
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    // configure DI for application services
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<ApplicationDbSeeder>();
    services.AddScoped<YtDlService>();
    //services.AddScoped<YtDlServiceWProgress>();
    
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();

// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();

app.MapControllerRoute(
    name: "Trigger",
    pattern: "{controller=Trigger}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "WeatherForecast",
    pattern: "{controller=WeatherForecast}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "Users",
    pattern: "{controller=Users}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "YtApi",
    pattern: "{controller=YtApi}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

// create db scheme and seed data
using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        var seeder = scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>();
        await seeder.InitializeData();
    }

app.Run();
//app.Run("https://localhost:7066");
//app.Run("http://localhost:3000");
