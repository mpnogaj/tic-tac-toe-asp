using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TicTacToe.Db;
using TicTacToe.Hubs;
using TicTacToe.Services;
using TicTacToe.Services.Impl;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

services.AddAuthentication().AddJwtBearer(options =>
{
    var jwtSecurityKey = configuration["Jwt:SecurityKey"]!;
    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecurityKey));
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new()
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = signingKey
    };

    options.Events = new JwtBearerEvents()
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["jwt"];
            return Task.CompletedTask;
        }
    };
});

services.AddAuthorization(options =>
{
    options.AddPolicy("LoggedOnly", policy => policy.RequireClaim(ClaimTypes.Authentication));
});

services.AddDbContext<AppDbContext>(options => options.UseSqlite(configuration.GetConnectionString("Default")));

services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddSignalR();
services.AddCors();
services.AddControllers();

services.AddSingleton<IRoomManager, RoomManager>();
services.AddTransient<IJwtManager, JwtManager>();
services.AddTransient<IAccountManager, AccountManager>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    using var client = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    client.Database.EnsureCreated();
}


app.UseRouting();

app.UseCors(options => options.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapHub<TicTacToeHub>("/game");

app.Run();