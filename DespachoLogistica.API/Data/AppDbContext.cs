using DespachoLogistica.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DespachoLogistica.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Bodega> Bodegas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<SolicitudDetalle> SolicitudDetalles { get; set; }
        public DbSet<KardexMovimiento> KardexMovimientos { get; set; } // ✅ NUEVO

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>()
                .Property(p => p.StockMinimo)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Stock>()
                .Property(s => s.Cantidad)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<SolicitudDetalle>()
                .Property(s => s.Cantidad)
                .HasColumnType("decimal(18,2)");

            // ✅ Configuración Kardex
            modelBuilder.Entity<KardexMovimiento>()
                .Property(k => k.Cantidad)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<KardexMovimiento>()
                .Property(k => k.StockAntes)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<KardexMovimiento>()
                .Property(k => k.StockDespues)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Stock>()
                .HasIndex(s => new { s.ProductoID, s.BodegaID })
                .IsUnique();

            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Codigo)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Rol>().HasData(
                new Rol { RolID = 1, Nombre = "Admin" },
                new Rol { RolID = 2, Nombre = "Bodeguero" },
                new Rol { RolID = 3, Nombre = "Solicitante" },
                new Rol { RolID = 4, Nombre = "Gerente" }
            );

            modelBuilder.Entity<Bodega>().HasData(
                new Bodega
                {
                    BodegaID = 1,
                    Nombre = "Bodega Central",
                    Ubicacion = "Zona 12, Guatemala",
                    Activa = true
                }
            );
        }
    }
}
