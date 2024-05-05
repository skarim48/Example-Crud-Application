using ApiProject;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//Why this ? It's For using .Net core Identity. For Example : Signin function need Acces to Database.
builder.Services.AddDbContext<ModelProject.EFContext>(options =>
    options.UseSqlServer("Server=***;Database=ForCRUD;Trusted_Connection=True;TrustServerCertificate=True")
);
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false
    )
    .AddEntityFrameworkStores<ModelProject.EFContext>();

// JWT
var jwtIssuer = builder.Configuration.GetSection("jwt:Issuer").Get<string>();
var jwtAudience = builder.Configuration.GetSection("jwt:Audience").Get<string>();
var jwtKey = builder.Configuration.GetSection("jwt:key").Get<string>();
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

//
builder.Services.AddControllers();
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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
}); 

app.Run();
