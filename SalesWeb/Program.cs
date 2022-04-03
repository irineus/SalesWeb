using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using SalesWeb.Data;
using SalesWeb.Services;
using System.Globalization;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<SellerService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<SalesRecordService>();
builder.Services.AddScoped<SellerImgStorageService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPINSIGHTS-CONNECTIONSTRING"]);
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["AZURE-STORAGE-CONNECTION-STRING:blob"], preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["AZURE-STORAGE-CONNECTION-STRING:queue"], preferMsi: true);
});

// Add Azure App Configuration to the container.
var azAppConfigConnection = builder.Configuration["AzureAppConfiguration"];
if (!string.IsNullOrEmpty(azAppConfigConnection))
{
    // Use the connection string if it is available.
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(azAppConfigConnection)
        .ConfigureRefresh(refresh =>
        {
            // All configuration values will be refreshed if the sentinel key changes.
            refresh.Register("TestApp:Settings:Sentinel", refreshAll: true);
        });
    });
}
else if (Uri.TryCreate(builder.Configuration["Endpoints:AzureAppConfiguration"], UriKind.Absolute, out var endpoint))
{
    // Use Azure Active Directory authentication.
    // The identity of this app should be assigned 'App Configuration Data Reader' or 'App Configuration Data Owner' role in App Configuration.
    // For more information, please visit https://aka.ms/vs/azure-app-configuration/concept-enable-rbac
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(endpoint, new DefaultAzureCredential())
        .ConfigureRefresh(refresh =>
        {
            // All configuration values will be refreshed if the sentinel key changes.
            refresh.Register("TestApp:Settings:Sentinel", refreshAll: true);
        });
    });
}
builder.Services.AddAzureAppConfiguration();

var teste = builder.Configuration.GetValue<string>("SaleService:Installment:BlockingTypes").Split(',').Select(x => int.Parse(x)).ToList<int>(); // <- ************** Teste do AzureAppConfiguration

for (int i = 0; i <= teste.Max(); i++)
{
    bool contem = teste.Contains(i);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Localization setup
var enUS = new CultureInfo("en-US");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(enUS),
    SupportedCultures = new List<CultureInfo> { enUS },
    SupportedUICultures = new List<CultureInfo> { enUS }
};
app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAzureAppConfiguration();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
