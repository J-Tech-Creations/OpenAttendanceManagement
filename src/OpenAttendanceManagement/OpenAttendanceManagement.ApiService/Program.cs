using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenAttendanceManagement.ApiService;
using OpenAttendanceManagement.AuthCommon;
using OpenAttendanceManagement.ServiceDefaults;
var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
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
// builder.Services.AddAuthentication(options =>
//     {
//         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//     })
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = builder.Configuration["Jwt:Issuer"],
//             ValidAudience = builder.Configuration["Jwt:Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//         };
//     });
// builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.AddNpgsqlDbContext<AuthDbContext>("authdb");
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AuthDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering",
    "Scorching"
};

app.MapGet(
        "/weatherforecast",
        () =>
        {
            var forecast = Enumerable.Range(1, 5)
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
    .WithOpenApi()
    .RequireAuthorization()
    ;
app.MapIdentityApi<IdentityUser>();
app.MapDefaultEndpoints();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    dbContext.Database.Migrate();
    AppContextSeed.Seed(dbContext);
}

app.Run();
