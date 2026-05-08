using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Se7ety.Api.Data;
using Se7ety.Api.Domain.Entities;
using Se7ety.Api.DTOs.Common;
using Se7ety.Api.Mapping;
using Se7ety.Api.Middleware;
using Se7ety.Api.Options;
using Se7ety.Api.Repositories.Implementations;
using Se7ety.Api.Repositories.Interfaces;
using Se7ety.Api.Services.Implementations;
using Se7ety.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// =====================
// 🔥 PORT FIX (IMPORTANT FOR RENDER)
// =====================
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// =====================
// Database
// =====================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string is not configured.");

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT options are not configured.");

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllers();

// =====================
// CORS (🔥 مهم لـ Flutter)
// =====================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(item => item.Value?.Errors.Count > 0)
            .ToDictionary(
                item => item.Key,
                item => item.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

        var response = new ApiErrorResponse(
            StatusCodes.Status400BadRequest,
            "Validation failed.",
            context.HttpContext.TraceIdentifier,
            errors);

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Se7ety API",
        Version = "v1"
    });
});

// DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Services (زي ما هي عندك)
builder.Services.AddAutoMapper(_ => { }, typeof(ApplicationMappingProfile).Assembly);
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

var app = builder.Build();

// =====================
// Middleware order (IMPORTANT)
// =====================
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// مهم: ما تستخدمش HTTPS redirect في Render
// app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapControllers();

// Root
app.MapGet("/", () => Results.Ok("Se7ety API is running 🚀"));

app.Run();
