using System.Globalization;
using Microsoft.AspNetCore.Localization;
using OcelotUI.UI;
using OcelotUI.UI.Services;
using OcelotUI.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOcelotUI(builder.Configuration);
builder.Services.AddScoped<ICultureSwitcher, WebCultureSwitcher>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("zh-TW") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseAntiforgery();

app.MapGet("/api/set-culture", (string culture, string redirectUri, HttpContext context) =>
{
    context.Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
    return Results.LocalRedirect(redirectUri);
});

app.MapRazorComponents<OcelotUI.Web.Components.App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(OcelotUI.UI.Components.Routes).Assembly);

app.Run();
