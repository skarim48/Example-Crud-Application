using ApiProject;
using BlazorApp1.Components;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModelProject;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<EFContext>(options =>
    options.UseSqlServer("Server=OUJ9ZLKN53\\SQLEXPRESS;Database=ForCRUD;Trusted_Connection=True;TrustServerCertificate=True")
);

// Configure logging providers
builder.Logging.AddConsole(); // Add Console logging provider
builder.Logging.AddDebug();   // Add Debug logging provider



//Jwt configuration starts here
var jwtIssuer = builder.Configuration.GetSection("jwt:Issuer").Get<string>();
var jwtAudience = builder.Configuration.GetSection("jwt:Audience").Get<string>();
var jwtKey = builder.Configuration.GetSection("jwt:key").Get<string>();
builder.Services.AddCors();
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddTransient<ApiProject.LoggingMiddleware>();
builder.Services.AddAuthorization(config =>
{
    //config.AddPolicy(JWTPolicies.Admin, JWTPolicies.AdminPolicy());
    //config.AddPolicy(JWTPolicies.User, JWTPolicies.UserPolicy());
});

var app = builder.Build();
app.UseMiddleware<LoggingMiddleware>();
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.UseAuthentication();
app.UseAuthorization();


app.Run();
