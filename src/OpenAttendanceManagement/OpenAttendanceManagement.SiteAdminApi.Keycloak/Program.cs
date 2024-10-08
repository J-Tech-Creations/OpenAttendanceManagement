using Microsoft.OpenApi.Models;
using OpenAttendanceManagement.AuthCommon;
using OpenAttendanceManagement.Common;
using OpenAttendanceManagement.Domain;
using OpenAttendanceManagement.ServiceDefaults;
using Sekiban.Core.Dependency;
using Sekiban.Infrastructure.Postgres;
using Sekiban.Web.Authorizations;
using Sekiban.Web.Authorizations.Definitions;
using Sekiban.Web.Dependency;
using Sekiban.Web.OpenApi.Extensions;
var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddControllers();

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

builder.Services.AddTransient<IOamAuthentication, OamAuthenticationKeycloak>();

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


app.MapControllers();

app.Run();
