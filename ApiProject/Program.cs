using ApiProject;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ModelProject.EFContext>(options =>
    options.UseSqlServer("Server=OUJ9ZLKN53\\SQLEXPRESS;Database=ForCRUD;Trusted_Connection=True;TrustServerCertificate=True")
);
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false
    )
    .AddEntityFrameworkStores<ModelProject.EFContext>();

// Add services to the container.
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

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});
//app.MapGet("/api/Users/Test", () =>
//{

//});
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
}); 

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
