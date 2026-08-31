using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Reda.Data;
using Reda.Interfaces;
using Reda.Services;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// 1. إضافة الـ Controllers والـ Swagger مع ضبط الـ JSON لمنع الـ Cycles
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. جلب المفتاح السري من ملف appsettings.json ديناميكياً وضبط الـ JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// 3. قراءة الـ Connection String من ملف appsettings.json وتسجيل قاعدة البيانات
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 4. تحديد صلاحيات الـ CORS وسماح للدومين المخصص لـ React فقط
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins(new[] { "http://localhost:3000", "https://reda-store-five.vercel.app" })
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 5. تسجيل الخدمات (Dependency Injection)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<INotificationService, EmailNotificationService>();
builder.Services.AddScoped<ISendCodeToEmail, SendCodeToEmailService>();
builder.Services.AddScoped<IWebServices, WebServices>();
builder.Services.AddScoped<IFileServices, FileServices>();
var app = builder.Build();

// 6. إعدادات خط سير البيانات (HTTP Request Pipeline)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// تفعيل الـ CORS بالسياسة المحددة (قبل الـ Authentication)
app.UseCors("AllowReact");

// --- الترتيب هنا إجباري وحاسم جداً لحماية الـ APIs ---
app.UseAuthentication(); // 1. التحقق من التوكن والهوية أولاً
app.UseAuthorization();  // 2. التحقق من الصلاحيات والـ [Authorize] ثانياً

app.MapControllers();

app.Run();