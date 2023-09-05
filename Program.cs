using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NoticiasApiAuth;
using NoticiasApiAuth.Data;
using NoticiasApiAuth.Models;
using NoticiasApiAuth.Repositories;
using NoticiasApiAuth.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>();

var key = Encoding.ASCII.GetBytes(Settings.Secret);


builder.Services.AddAuthentication(x => 
{ 
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("manager"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", (User model) =>
{
    var user = UserRepository.Get(model.Username, model.Password);

    if (user == null)
        return Results.NotFound(new { message = "Nome/Senha inválida" });

    var token = TokenService.GenerateToken(user);

    user.Password = "";

    return Results.Ok(new
    {
        user = user,
        token = token
    });
});

app.MapGet("/noticias", (AppDbContext context) =>
{
    var noticias = context.Noticias.ToList();
    return Results.Ok(noticias);
}).RequireAuthorization();


app.MapPost("/noticias", (Noticia noticia, AppDbContext context) =>
{
    context.Noticias.Add(noticia);
    context.SaveChanges();
    return Results.Created($"/noticias/{noticia.Id}", noticia);
}).RequireAuthorization("Admin");

app.Run();
