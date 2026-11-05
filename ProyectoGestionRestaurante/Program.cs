using System.Text;
using Aplicaciones.DTOs;
using Aplicaciones.Extensiones;
using Aplicaciones.Interfaces;
using Infraestructura.Extensiones;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProyectoGestionRestaurante.Seguridad;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger con el botón "Authorize" para pegar el token.
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegue solo el token que devuelve api/auth/login (sin la palabra Bearer)."
    });

    opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Cada biblioteca registra sus propias parejas interfaz-clase.
builder.Services.AgregarInfraestructura();
builder.Services.AgregarAplicacion();
builder.Services.AddSingleton<IGeneradorToken, JwtService>();

// Autenticación con JWT (todo esto va ANTES de builder.Build()).
var jwt = builder.Configuration.GetSection("JwtSettings");
var clave = Encoding.UTF8.GetBytes(jwt["Key"]!);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(clave)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Como todo exige sesión, se crea un supervisor inicial para poder entrar la primera vez.
// (Los datos están en memoria: se vuelve a crear en cada arranque.)
using (var scope = app.Services.CreateScope())
{
    var empleados = scope.ServiceProvider.GetRequiredService<IEmpleadoServicio>();
    empleados.Crear(new EmpleadoEntrada { Nombre = "admin", Tipo = "Supervisor", Contrasena = "admin123" });
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();    // abre wwwroot/index.html en la dirección principal
app.UseStaticFiles();     // sirve la pantalla (HTML, CSS y JS)
app.UseAuthentication();   // primero: quién eres
app.UseAuthorization();    // después: qué puedes hacer
app.MapControllers();

app.Run();
