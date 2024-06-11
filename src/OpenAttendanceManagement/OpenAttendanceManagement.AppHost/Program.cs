var builder = DistributedApplication.CreateBuilder(args);

var pgsqlPassword = builder.AddParameter("postgresPassword", secret: true);

var postgresHost = builder.AddPostgres("oamDb", password: pgsqlPassword)
    .WithDataVolume()
    .WithPgAdmin();

var authDb = postgresHost.AddDatabase("authdb");
var eventDb = postgresHost.AddDatabase("eventdb");


var authService=builder.AddProject<Projects.OpenAttendanceManagement_AuthService>("authservice")
    .WithExternalHttpEndpoints()
    .WithReference(authDb)
    .WithEnvironment("Jwt:Issuer", builder.Configuration["Jwt:Issuer"])
    .WithEnvironment("Jwt:Audience", builder.Configuration["Jwt:Audience"])
    .WithEnvironment("Jwt:Key", builder.Configuration["Jwt:Key"]);



var apiService = builder.AddProject<Projects.OpenAttendanceManagement_ApiService>("apiservice")
    .WithReference(authDb)
    .WithEnvironment("Jwt:Issuer", builder.Configuration["Jwt:Issuer"])
    .WithEnvironment("Jwt:Audience", builder.Configuration["Jwt:Audience"])
    .WithEnvironment("Jwt:Key", builder.Configuration["Jwt:Key"]);


builder.AddProject<Projects.OpenAttendanceManagement_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(authService);


builder.Build().Run();
