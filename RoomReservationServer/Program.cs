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
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureMySqlContext(builder.Environment, builder.Configuration);
builder.Services.ConfigureRepositoryWrapper();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureSharedController();
builder.Services.ConfigureEmailService(builder.Configuration);
builder.Services.ConfigureFileService();
builder.Services.ConfigureJWTService(builder.Environment, builder.Configuration);
builder.Services.ConfigureControllers();

var app = builder.Build();

ApplyCulture();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @$"{builder.Configuration["ResourcesDir"]}")),
    RequestPath = new PathString($"/{builder.Configuration["ResourcesDir"]}")
});

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



void ApplyCulture()
{
    var cultureInfo = new CultureInfo("sl-SL");
    cultureInfo.NumberFormat.NumberDecimalSeparator = ".";

    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
}
