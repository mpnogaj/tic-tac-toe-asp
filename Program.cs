using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TicTacToe.Hubs;
using TicTacToe.Services;
using TicTacToe.Services.Impl;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

services.AddSingleton<IRoomManager, RoomManager>();

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

services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddSignalR();
services.AddCors();
services.AddControllers();

var app = builder.Build();

app.UseRouting();

app.UseCors(options => options.WithOrigins("http://192.168.88.135:3000", "http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials());

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