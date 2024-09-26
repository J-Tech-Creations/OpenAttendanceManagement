using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OpenAttendanceManagement.ServiceDefaults;
using OpenAttendanceManagement.Web.Keycloak;
using OpenAttendanceManagement.Web.Keycloak.Components;
var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder
    .Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder
    .Services
    .AddHttpContextAccessor()
    .AddTransient<AuthorizationHandler>();

builder
    .Services
    .AddHttpClient<WeatherApiClient>(
        client =>
        {
            // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
            // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
            client.BaseAddress = new Uri("https+http://apiservicekeycloak");
        })
    .AddHttpMessageHandler<AuthorizationHandler>();

var oidcScheme = OpenIdConnectDefaults.AuthenticationScheme;

builder
    .Services
    .AddAuthentication(oidcScheme)
    .AddKeycloakOpenIdConnect(
        "keycloak",
        "oamtenant",
        oidcScheme,
        options =>
        {
            options.ClientId = "oamclient";
            options.ResponseType = OpenIdConnectResponseType.Code;
            // options.Scope.Add("oamclient-dedicated");
            options.ClientSecret = builder.Configuration["services:keycloak:clientSecret"] ?? string.Empty;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
            options.SaveTokens = true;
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddCascadingAuthenticationState();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app
    .MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapLoginAndLogout();

app.Run();
