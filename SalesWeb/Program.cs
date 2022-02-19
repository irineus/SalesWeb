using Microsoft.EntityFrameworkCore;
using SalesWeb.Data;
using SalesWeb.Services;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

var builder = WebApplication.CreateBuilder(args);

//builder.Host.ConfigureLogging(logging =>
//{
//    logging.ClearProviders();
//    logging.AddConsole();
//    logging.AddApplicationInsights();
//    logging.AddDebug();
//});

var keyVaultEndpoint = builder.Configuration["AzureKeyVault:Endpoint"];
var keyVaultTenantId = builder.Configuration["AzureKeyVault:TenantId"];
var keyVaultClientId = builder.Configuration["AzureKeyVault:ClientId"];
var keyVaultClientSecret = builder.Configuration["AzureKeyVault:ClientSecret"];

var credential = new ClientSecretCredential(keyVaultTenantId, keyVaultClientId, keyVaultClientSecret);

var client = new SecretClient(new Uri(keyVaultEndpoint), credential);

builder.Configuration.AddAzureKeyVault(client, new AzureKeyVaultConfigurationOptions());

var connectionString = builder.Configuration.GetConnectionString("SalesWebContext");

builder.Services.AddDbContext<SalesWebContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDbContext<SellerService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPINSIGHTS-CONNECTIONSTRING"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
