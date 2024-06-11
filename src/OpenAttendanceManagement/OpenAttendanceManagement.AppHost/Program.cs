var builder = DistributedApplication.CreateBuilder(args);

var pgsqlPassword = builder.AddParameter("postgresPassword", secret: true);

var postgresHost = builder.AddPostgres("oamDb", password: pgsqlPassword)
    .WithDataVolume()
    .WithPgAdmin();

var authDb = postgresHost.AddDatabase("authdb");
var eventDb = postgresHost.AddDatabase("eventdb");


var apiService = builder.AddProject<Projects.OpenAttendanceManagement_ApiService>("apiservice")
    .WithReference(authDb);


builder.AddProject<Projects.OpenAttendanceManagement_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);


builder.Build().Run();
