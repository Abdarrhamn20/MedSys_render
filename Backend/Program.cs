using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MedicalSystem.Data;
using MedicalSystem.Services;
using MedicalSystem.Helpers;
using MedicalSystem.Hubs;
using WebPush;

var builder = WebApplication.CreateBuilder(args);

// === Database ===
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(GetPostgresConnectionString(builder.Configuration),
        x => x.EnableRetryOnFailure()));

// === Settings Service (SystemSettings table) ===
builder.Services.AddScoped<ISettingsService, SettingsService>();

// === JWT Authentication ===
// ÙŠÙÙØ¶Ù‘Ù„ Ø¶Ø¨Ø· Ù…ÙØªØ§Ø­ Ø§Ù„ØªÙˆÙ‚ÙŠØ¹ Ø¹Ø¨Ø± Ù…ØªØºÙŠØ± Ø¨ÙŠØ¦Ø© JWT_KEY Ø£Ùˆ Jwt:Key (Ù„Ø§ ÙŠÙØµØ±Ù‘Ø­ Ø§Ø³ØªØ®Ø¯Ø§Ù… Ø§Ù„Ù…ÙØªØ§Ø­ Ø§Ù„Ø§ÙØªØ±Ø§Ø¶ÙŠ ÙÙŠ Ø§Ù„Ø¥Ù†ØªØ§Ø¬)
var jwtKey = builder.Configuration["JWT_KEY"]
             ?? builder.Configuration["Jwt:Key"]
             ?? string.Empty;
if (string.IsNullOrEmpty(jwtKey))
    throw new InvalidOperationException("Ù…ÙØªØ§Ø­ ØªÙˆÙ‚ÙŠØ¹ JWT ØºÙŠØ± Ù…ÙØ¹Ø±Ù‘Ù. ÙŠØ±Ø¬Ù‰ Ø¶Ø¨Ø· JWT_KEY Ø£Ùˆ Jwt:Key ÙÙŠ Ø§Ù„Ø¥Ø¹Ø¯Ø§Ø¯Ø§Øª.");

var usesDefaultJwtKey = jwtKey == "MedicalSystem_SuperSecretKey_2026_IVS_Project_SecureToken!@#$%";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    // SignalR ÙŠÙ…Ø±Ù‘Ø± Ø§Ù„ØªÙˆÙƒÙ† Ø¹Ø¨Ø± query string (access_token) Ù„Ø£Ù†Ù‡ Ù„Ø§ ÙŠÙ…ÙƒÙ† Ø§Ø³ØªØ®Ø¯Ø§Ù… Ø±Ø¤ÙˆØ³ ÙÙŠ WebSocket
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/hubs/telemedicine") || path.StartsWithSegments("/hubs/notifications")))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// === CORS (Ù…Ù‚ÙŠÙ‘Ø¯ ÙÙ‚Ø· Ø¨Ø§Ù„Ù…Ø³Ø§Ø±Ø§Øª Ø§Ù„Ù…ÙØµØ±Ù‘Ø­ Ø¨Ù‡Ø§Ø› Ø§Ù„ÙˆØ§Ø¬Ù‡Ø© ØªÙØ®Ø¯ÙŽÙ… Ù…Ù† Ù†ÙØ³ Ø§Ù„Ø®Ø§Ø¯Ù… ÙÙ„Ø§ Ø¯Ø§Ø¹ÙŠ Ù„Ù€ AllowAll) ===
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowConfigured", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            // Ù„Ø§ ÙŠÙØ³Ù…Ø­ Ø¨Ø£ÙŠ Ø·Ù„Ø¨Ø§Øª Ø¹Ø¨Ø± Ø§Ù„Ù…ÙˆØ§Ù‚Ø¹ Ø¥Ù„Ø§ Ù…Ø§ ÙƒØ§Ù† Ù…Ù† Ù†ÙØ³ Ø§Ù„Ø£ØµÙ„
            policy.WithOrigins();
        }
    });
});

// === Services (DI) ===
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IAppNotificationService, AppNotificationService>();
builder.Services.AddSingleton(new WebPushClient());

// === Ø®Ø¯Ù…Ø© Ø®Ù„ÙÙŠØ© Ù„Ø¥Ø´Ø¹Ø§Ø±Ø§Øª Ø­Ø§Ù† Ù…ÙˆØ¹Ø¯ Ø§Ù„Ø¬Ù„Ø³Ø© ===
builder.Services.AddHostedService<NotificationBackgroundService>();

// === Controllers ===
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();

if (usesDefaultJwtKey)
    app.Logger.LogWarning("ØªØ­Ø°ÙŠØ± Ø£Ù…Ù†ÙŠ: ÙŠØªÙ… Ø§Ø³ØªØ®Ø¯Ø§Ù… Ù…ÙØªØ§Ø­ JWT Ø§Ù„Ø§ÙØªØ±Ø§Ø¶ÙŠ Ù„Ù„ØªØ·ÙˆÙŠØ±. ØºÙŠÙ‘Ø±Ù‡ Ù‚Ø¨Ù„ Ø§Ù„Ù†Ø´Ø± ÙÙŠ Ø§Ù„Ø¥Ù†ØªØ§Ø¬ (JWT_KEY).");

// === Auto-apply pending migrations on startup ===
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// === Middleware Pipeline ===
app.UseSwagger();
app.UseSwaggerUI();

// Global exception handler
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { error = "Ø­Ø¯Ø« Ø®Ø·Ø£ ØºÙŠØ± Ù…ØªÙˆÙ‚Ø¹ ÙÙŠ Ø§Ù„Ø®Ø§Ø¯Ù…. ÙŠØ±Ø¬Ù‰ Ø§Ù„Ù…Ø­Ø§ÙˆÙ„Ø© Ù„Ø§Ø­Ù‚Ø§Ù‹." }));
    }
});

app.UseCors("AllowConfigured");
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<TelemedicineHub>("/hubs/telemedicine");
app.MapHub<NotificationHub>("/hubs/notifications");

// Fallback for AngularJS SPA routing
app.MapFallbackToFile("index.html");

static string GetPostgresConnectionString(IConfiguration cfg)
{
    var raw = cfg["DATABASE_URL"];
    if (!string.IsNullOrWhiteSpace(raw) && raw.StartsWith("postgres://"))
    {
        var uri = new Uri(raw);
        var info = uri.UserInfo ?? "";
        var parts = info.Split(':', 2);
        var user = parts.Length > 0 ? Uri.UnescapeDataString(parts[0]) : "";
        var pass = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : "";
        return $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={user};Password={pass};Timeout=30;Trust Server Certificate=true";
    }
    var conn = cfg.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrWhiteSpace(conn)) return conn;
    throw new InvalidOperationException("Missing database connection. Set DATABASE_URL or ConnectionStrings:DefaultConnection.");
}

app.Run();
