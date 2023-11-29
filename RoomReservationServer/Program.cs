using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using NLog;
using RoomReservationServer.Extensions;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
Directory.CreateDirectory(@$"{builder.Configuration["ResourcesDir"]}");

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Uncomment this if large file support is needed:
    //serverOptions.Limits.MaxRequestBodySize = long.MaxValue;
});

// Add services to the container.

builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();

builder.Services.ConfigureLoggerService();



string connectionString = builder.Environment.IsDevelopment()
    ? builder.Configuration["mysqlconnection:connectionString"] : Environment.GetEnvironmentVariable("CONNECTION_STRING");

builder.Services.ConfigureMySqlContext(connectionString);



builder.Services.ConfigureRepositoryWrapper();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.ConfigureSharedController();

builder.Services.ConfigureEmailService(builder.Configuration);

builder.Services.ConfigureFileService();



string tokenKey = builder.Environment.IsDevelopment()
    ? builder.Configuration["TokenKey"] : Environment.GetEnvironmentVariable("TOKEN_KEY");

builder.Services.ConfigureJWTService(tokenKey);



builder.Services.ConfigureControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseHttpsRedirection();

// We call the UseCors method above the UseAuthorization method, as Microsoft recommends.
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();



var cultureInfo = new CultureInfo("sl-SL");
cultureInfo.NumberFormat.NumberDecimalSeparator = ".";

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;



app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @$"{builder.Configuration["ResourcesDir"]}")),
    RequestPath = new PathString($"/{builder.Configuration["ResourcesDir"]}")
});

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.MapControllers();

app.Run();
