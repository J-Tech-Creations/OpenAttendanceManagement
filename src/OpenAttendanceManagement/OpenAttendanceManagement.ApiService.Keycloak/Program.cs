using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using OpenAttendanceManagement.ApiService.Keycloak;
using OpenAttendanceManagement.AuthCommon;
using OpenAttendanceManagement.Common;
using OpenAttendanceManagement.Common.UseCases;
using OpenAttendanceManagement.Domain;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using OpenAttendanceManagement.Domain.Usecases;
using OpenAttendanceManagement.ServiceDefaults;
using ResultBoxes;
using Sekiban.Core;
using Sekiban.Core.Dependency;
using Sekiban.Infrastructure.Postgres;
using Sekiban.Web.Authorizations;
using Sekiban.Web.Authorizations.Definitions;
using Sekiban.Web.Dependency;
using Sekiban.Web.OpenApi.Extensions;
var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.AddSecurityDefinition(
            "bearerAuth",
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });
        c.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                            { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                    },
                    new string[] { }
                }
            });
    });

builder
    .Services
    .AddAuthentication()
    .AddKeycloakJwtBearer(
        "keycloak",
        "oamtenant",
        options =>
        {
            options.Authority = "http://localhost:18080/realms/oamtenant";
            options.Audience = "oamclient";
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters.ValidateAudience = false;
        });

builder.Services.AddAuthorization();

builder.AddSekibanWithDependency<OamDomainDependency>();
builder.AddSekibanPostgresDbWithAzureBlobStorage();
builder.AddSekibanWebFromDomainDependency<OamDomainDependency>(
    definition => definition.AuthorizationDefinitions =
        new AuthorizeDefinitionCollectionWithKeycloak(
            new AllowOnlyWithRolesAndDenyIfNot<AllMethod, OamRoles>(OamRoles.SiteAdmin)));
builder.Services.AddSwaggerGen(options => options.ConfigureForSekibanWeb());

builder.Services.AddTransient<ISekibanUsecaseExecutor, SekibanUsecaseExecutor>();
builder.Services.AddTransient<ISekibanUsecaseContext, SekibanUsecaseContext>();

builder.Services.AddTransient<IOamUserManager, OamUserManagerKeycloak>();
builder.Services.AddTransient<IOamAuthentication, OamAuthenticationKeycloak>();
builder.Services.AddTransient<TokenServiceKeycloak>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app
    .MapGet(
        "/weatherforecast",
        () =>
        {
            var forecast = Enumerable
                .Range(1, 5)
                .Select(
                    index =>
                        new WeatherForecast(
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            summaries[Random.Shared.Next(summaries.Length)]
                        ))
                .ToArray();
            return forecast;
        })
    .WithName("GetWeatherForecast")
    .WithOpenApi()
    .RequireAuthorization();

app
    .MapGet(
        "/user/roles",
        (TokenServiceKeycloak tokenServiceKeycloak) =>
            tokenServiceKeycloak
                .GetRolesAsync()
                .Match(
                    roles => Results.Ok(roles),
                    error => Results.Unauthorized()))
    .WithName("GetUserRoles")
    .WithOpenApi()
    .RequireAuthorization();
app.MapControllers();

app
    .MapGet(
        "/user/tenants",
        (ISekibanExecutor sekibanExecutor, TokenServiceKeycloak tokenServiceKeycloak) =>
            tokenServiceKeycloak
                .GetEmailAsync()
                .Remap(AuthIdentityEmail.FromString)
                .Conveyor(email => sekibanExecutor.ExecuteQuery(new BelongingTenantQuery(email)))
                .ToResults()
    )
    .WithName("GetUserTenants")
    .WithOpenApi()
    .RequireAuthorization();

app
    .MapPost(
        "/admin/startmonth",
        async ([FromBody] CheckOrStartTenantTerm input, [FromServices] ISekibanExecutor executor)
            => await executor
                .ExecuteUsecase(input)
                .Match(success => Results.Ok(), exception => Results.Problem(exception.Message))
    )
    .WithName("StartMonth")
    .WithOpenApi()
    .RequireAuthorization();

app.MapControllers();
app.Run();
