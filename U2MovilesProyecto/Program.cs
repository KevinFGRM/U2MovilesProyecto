using AvisosAPI.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniJokeRPGAPI.Data;
using System.Text;
using U2MovilesProyecto.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MinijokerpgContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection"))));


builder.Services.AddScoped(typeof(Repository<>));

builder.Services.AddAutoMapper(x => { }, typeof(Program).Assembly);
builder.Services.AddHttpContextAccessor();




builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AmigosService>();
builder.Services.AddScoped<MensajesService>();
builder.Services.AddScoped<PartidasService>();
builder.Services.AddScoped<NotificacionesService>();



builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var jwtKey = builder.Configuration["Jwt:Key"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters.IssuerSigningKey =
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Jwt:Key") ?? ""));
        options.TokenValidationParameters.ValidateAudience = true;
        options.TokenValidationParameters.ValidateIssuer = true;
        options.TokenValidationParameters.ValidateLifetime = true;
        options.TokenValidationParameters.ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience");
        options.TokenValidationParameters.ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer");
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseFileServer();

app.Run();
