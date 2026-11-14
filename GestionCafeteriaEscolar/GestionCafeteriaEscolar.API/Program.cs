using GestionCafeteriaEscolar.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using GestionCafeteriaEscolar.Domain.Entities;
using GestionCafeteriaEscolar.Infrastructure.Core;
using GestionCafeteriaEscolar.Infrastructure.Repositories;
using GestionCafeteriaEscolar.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<GenericRepository<Producto>>();
builder.Services.AddScoped<GenericRepository<Venta>>();
builder.Services.AddScoped<EstudianteRepository>();
builder.Services.AddScoped<DetalleVentaRepository>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<EstudianteService>();
builder.Services.AddScoped<VentaService>();
builder.Services.AddScoped<DetalleVentaService>();
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "GestionCafeteriaEscolar API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
