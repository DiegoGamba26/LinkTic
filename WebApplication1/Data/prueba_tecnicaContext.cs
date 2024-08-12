using System;
using Microsoft.EntityFrameworkCore;
using WEBAPI.Models;

namespace WEBAPI.Data
{
    public partial class prueba_tecnicaContext : DbContext
    {
        public prueba_tecnicaContext()
        {
        }

        public prueba_tecnicaContext(DbContextOptions<prueba_tecnicaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Reservation> Reservations { get; set; } = null!;
        public virtual DbSet<Service> Services { get; set; } = null!;
        public virtual DbSet<Users> Users { get; set; } = null!;
        public virtual DbSet<UserReservation> UserReservations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la relación muchos a muchos entre User y Reservation
            modelBuilder.Entity<UserReservation>()
                .HasKey(ur => new { ur.UserId, ur.ReservationID });

            modelBuilder.Entity<UserReservation>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserReservations) // Asegúrate de que esta propiedad exista en el modelo User
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserReservation>()
                .HasOne(ur => ur.Reservation)
                .WithMany(r => r.UserReservations) // Asegúrate de que esta propiedad exista en el modelo Reservation
                .HasForeignKey(ur => ur.ReservationID);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reservations_Customers");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reservations_Services");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
