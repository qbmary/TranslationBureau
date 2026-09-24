using System.Globalization;
using Microsoft.AspNetCore.Localization;
using TranslationBureau.Application;
using TranslationBureau.Infrastructure;

// Принудительно — InvariantCulture. Десятичный разделитель — точка.
// Это гарантирует, что сервер и клиент договорятся о формате чисел.
var invariant = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = invariant;
CultureInfo.DefaultThreadCurrentUICulture = invariant;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Локализация запроса — тоже InvariantCulture
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(invariant),
    SupportedCultures = new[] { invariant },
    SupportedUICultures = new[] { invariant }
};
app.UseRequestLocalization(localizationOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Translators}/{action=Index}/{id?}");

app.Run();