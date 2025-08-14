using MedSchedule.Application;
using MedSchedule.Infrastructure;
using MedSchedule.Infrastructure.Services;
using MedSchedule.WebApi.Filter;
using MedSchedule.WebApi.Middlewares;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMvc(d => d.Filters.Add(typeof(ExceptionHandlingFilter)));

builder.Services.AddRouting(d => d.LowercaseUrls = true);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddTransient<CultureInfoMiddleware>();

builder.Services.AddRateLimiter(cfg =>
{
    cfg.AddFixedWindowLimiter("create-appointment", fixedWindowOptions =>
    {
        fixedWindowOptions.PermitLimit = 2;
        fixedWindowOptions.Window = TimeSpan.FromMinutes(1);
        fixedWindowOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        fixedWindowOptions.QueueLimit = 4;
    });
});

builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("services:email"));

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<CultureInfoMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.UseRateLimiter();

app.Run();
