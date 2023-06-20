using WebApi.Helpers;
using WebApi.Services;
using Microsoft.Extensions.DependencyInjection;
using dotnet_react_xml_generator;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();

// add services to DI container
{
    var services = builder.Services;
    services.AddCors();
    services.AddControllers();

    // configure strongly typed settings object
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    // configure DI for application services
    services.AddScoped<YtDlService>();
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


app.MapControllerRoute(
    name: "YtApi",
    pattern: "{controller=YtApi}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");


if (!app.Environment.IsDevelopment())
{
    string? shortcut = builder.Configuration.GetValue<string>("AppSettings:Shortcut");
    PwaManager.multiPlatOpenShortcut(shortcut);
}

app.Run();
//app.Run("https://localhost:7066");
//app.Run("http://localhost:3000");

//Console.WriteLine("Hello world! (after app run)"); // niet hier, pas gedaan als app gesloten wordt