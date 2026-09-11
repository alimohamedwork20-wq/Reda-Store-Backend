using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Reda.Data;
using Reda.Filters;
using Reda.Interfaces;
using Reda.Middlware;
using Reda.Services;
using Reda.Validators;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// ===============================
// Controllers + JSON + Validation Filter
// ===============================

builder.Services.AddControllers(options =>
{
    // تشغيل ValidationFilter على جميع الـ Controllers
    options.Filters.Add<ValidationFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});


// ===============================
// Swagger
// ===============================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ===============================
// FluentValidation
// ===============================

// تسجيل جميع الـ Validators الموجودة في المشروع
builder.Services.AddValidatorsFromAssemblyContaining<ValidationRegister>();


// تسجيل الـ ValidationFilter
builder.Services.AddScoped<ValidationFilter>();


// ===============================
// JWT Authentication
// ===============================

var jwtKey = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,

        ValidateLifetime = true,

        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey!)
        )
    };
});


// ===============================
// Database
// ===============================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));


// ===============================
// CORS
// ===============================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "https://reda-store-five.vercel.app"
            )
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


// ===============================
// Problem Details
// ===============================

builder.Services.AddProblemDetails();


// ===============================
// Dependency Injection
// ===============================

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<INotificationService, EmailNotificationService>();
builder.Services.AddScoped<ISendCodeToEmail, SendCodeToEmailService>();
builder.Services.AddScoped<IAdminServices, AdminServices>();
builder.Services.AddScoped<IFileServices, FileServices>();


// ===============================
// Build
// ===============================

var app = builder.Build();


// ===============================
// Swagger
// ===============================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// ===============================
// HTTP Request Pipeline
// ===============================

app.UseHttpsRedirection();


// Exception Middleware
app.UseMiddleware<ExceptionMiddleware>();


// CORS
app.UseCors("AllowReact");


// Authentication
app.UseAuthentication();


// Authorization
app.UseAuthorization();


// Controllers
app.MapControllers();


// Run
app.Run();