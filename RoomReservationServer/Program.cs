using Microsoft.AspNetCore.HttpOverrides;
using RoomReservationServer.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();

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
