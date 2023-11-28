using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using NLog;
using RoomReservationServer.Extensions;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Uncomment this if large file support is needed:
    //serverOptions.Limits.MaxRequestBodySize = long.MaxValue;
});

// Add services to the container.

builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();

builder.Services.ConfigureLoggerService();

builder.Services.ConfigureMySqlContext(builder.Configuration);

builder.Services.ConfigureRepositoryWrapper();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.ConfigureSharedController();

builder.Services.ConfigureEmailService(builder.Configuration);

builder.Services.ConfigureFileService();

builder.Services.ConfigureJWTService(builder.Configuration);

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

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
    RequestPath = new PathString("/Resources")
});

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.MapControllers();

app.Run();
