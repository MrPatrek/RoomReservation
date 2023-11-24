using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using RoomReservationServer.Extensions;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Add services to the container.

builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();

builder.Services.ConfigureLoggerService();

builder.Services.ConfigureMySqlContext(builder.Configuration);

builder.Services.ConfigureRepositoryWrapper();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

// We call the UseCors method above the UseAuthorization method, as Microsoft recommends.
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
