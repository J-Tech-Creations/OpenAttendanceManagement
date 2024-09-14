using Projects;
var builder = DistributedApplication.CreateBuilder(args);
// please add user secrets for the following values
//   "Parameters:postgresPassword": "your_password_here"
var pgsqlPassword = builder.AddParameter("postgresPassword", true);

var postgresHost = builder
    .AddPostgres("oamDb", password: pgsqlPassword)
    .WithDataVolume()
    .WithPgAdmin();

var authDb = postgresHost.AddDatabase("authdb");
var eventDb = postgresHost.AddDatabase("sekibanPostgres");

var azureStorage = builder
    .AddAzureStorage("eventStorage")
    .RunAsEmulator(configure => configure.WithDataVolume());
var blob = azureStorage.AddBlobs("sekibanBlob");

var apiService = builder
    .AddProject<OpenAttendanceManagement_ApiService>("apiservice")
    .WithReference(authDb)
    .WithReference(eventDb)
    .WithReference(blob);


builder
    .AddProject<OpenAttendanceManagement_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

var siteAdminApiService = builder
    .AddProject<OpenAttendanceManagement_SiteAdminApi>("siteadminapiservice")
    .WithReference(authDb)
    .WithReference(eventDb)
    .WithReference(blob);

builder
    .AddProject<OpenAttendanceManagement_SiteAdminWeb>("siteadminweb")
    .WithExternalHttpEndpoints()
    .WithReference(siteAdminApiService);

builder.Build().Run();
