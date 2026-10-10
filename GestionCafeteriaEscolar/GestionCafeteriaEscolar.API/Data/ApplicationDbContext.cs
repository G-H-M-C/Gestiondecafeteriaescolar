using GestionCafeteriaEscolar.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionCafeteriaEscolar.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Estudiante> Estudiantes { get; set; }
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<DetalleVenta> DetallesVenta { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Precio).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Estudiante>(entity =>
        {
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Matricula).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Grado).IsRequired().HasMaxLength(30);
            entity.HasIndex(e => e.Matricula).IsUnique();
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.Property(v => v.Total).HasPrecision(18, 2);
            entity.HasOne<Estudiante>()
                .WithMany()
                .HasForeignKey(v => v.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.Property(d => d.PrecioUnitario).HasPrecision(18, 2);
            entity.Property(d => d.Subtotal).HasPrecision(18, 2);
            entity.HasOne<Venta>()
                .WithMany()
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Producto>()
                .WithMany()
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Sándwich de jamón", Precio = 25, Stock = 50 },
            new Producto { Id = 2, Nombre = "Jugo de naranja", Precio = 15, Stock = 40 });

        modelBuilder.Entity<Estudiante>().HasData(
            new Estudiante { Id = 1, Nombre = "Ana Torres", Matricula = "EST-001", Grado = "5to A" },
            new Estudiante { Id = 2, Nombre = "Luis Rojas", Matricula = "EST-002", Grado = "6to B" });
    }
}
