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

// please add user secrets for the following values
//   "Parameters:keycloak-password": "your_password_here"
var keycloak = builder
    .AddKeycloak("keycloak", 18080)
    .WithDataVolume();


var apiServiceKeycloak = builder
    .AddProject<OpenAttendanceManagement_ApiService_Keycloak>("apiservicekeycloak")
    .WithReference(keycloak);
var apiServiceAdminKeycloak = builder
    .AddProject<OpenAttendanceManagement_SiteAdminApi_Keycloak>("siteadminapiservicekeycloak")
    .WithReference(keycloak);

var webKeycloak = builder
    .AddProject<OpenAttendanceManagement_Web_Keycloak>("keycloakweb")
    .WithExternalHttpEndpoints()
    .WithReference(apiServiceKeycloak)
    .WithReference(keycloak);

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
