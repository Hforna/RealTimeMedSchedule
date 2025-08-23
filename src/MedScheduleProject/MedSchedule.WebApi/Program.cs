using MedSchedule.Application;
using MedSchedule.Infrastructure;
using MedSchedule.Infrastructure.Services;
using MedSchedule.WebApi.Filter;
using MedSchedule.WebApi.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                        Enter 'Bearer' [space] and then your token in the text input below.
                        Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddMvc(d => d.Filters.Add(typeof(ExceptionHandlingFilter)));

builder.Services.AddRouting(d => d.LowercaseUrls = true);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

var signKey = builder.Configuration.GetValue<string>("services:auth:token:signKey");

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

var tokenValidationParams = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signKey!)),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,     
    RequireExpirationTime = true,   
    ClockSkew = TimeSpan.FromMinutes(5),      

    NameClaimType = JwtRegisteredClaimNames.Sub,
    RoleClaimType = JwtRegisteredClaimNames.Typ
};

builder.Services.AddSingleton<TokenValidationParameters>(tokenValidationParams);

builder.Services.AddAuthorization(d =>
{
    d.AddPolicy("OnlyPatients", f => f.RequireRole("Patient", "Admin"));
    d.AddPolicy("OnlyAdmin", f => f.RequireRole("Admin"));
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt => {
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParams;
});

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
