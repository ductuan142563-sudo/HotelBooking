using HotelBooking.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        // Thêm các DbSet khác sau này (Booking, Service, Promotion...)

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ==================== HOTEL ====================
            builder.Entity<Hotel>(entity =>
            {
                entity.HasKey(h => h.HotelId);
                entity.Property(h => h.Name).IsRequired().HasMaxLength(200);
                entity.Property(h => h.Address).HasMaxLength(300);
                entity.Property(h => h.City).HasMaxLength(100);
                entity.Property(h => h.Phone).HasMaxLength(20);
                entity.Property(h => h.Email).HasMaxLength(100);
                entity.Property(h => h.Website).HasMaxLength(200);
                entity.HasIndex(h => h.Name);
            });

            // ==================== ROOM TYPE ====================
            builder.Entity<RoomType>(entity =>
            {
            entity.HasKey(rt => rt.RoomTypeId);
            entity.Property(rt => rt.Name).IsRequired().HasMaxLength(100);
            entity.Property(rt => rt.Description).HasMaxLength(500);
            entity.Property(rt => rt.BedType).HasMaxLength(50);
            entity.Property(rt => rt.BasePrice).HasColumnType("decimal(18,2)");
            entity.Property(rt => rt.Area).HasColumnType("de