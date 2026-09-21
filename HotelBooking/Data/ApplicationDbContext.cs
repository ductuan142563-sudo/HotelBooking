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

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<BookingService> BookingServices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ===== ApplicationUser =====
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.UserName).IsUnique();
            });

            // ===== Hotel =====
            builder.Entity<Hotel>(entity =>
            {
                entity.HasKey(h => h.HotelId);
                entity.Property(h => h.Name).IsRequired().HasMaxLength(200);
                entity.Property(h => h.Address).IsRequired().HasMaxLength(300);
                entity.Property(h => h.City).IsRequired().HasMaxLength(100);
                entity.HasIndex(h => h.City);
                entity.HasIndex(h => h.Star);
            });

            // ===== RoomType =====
            builder.Entity<RoomType>(entity =>
            {
                entity.HasKey(rt => rt.RoomTypeId);
                entity.Property(rt => rt.Name).IsRequired().HasMaxLength(100);
                entity.Property(rt => rt.BasePrice).HasColumnType("decimal(18,2)");
            });

            // ===== Room =====
            builder.Entity<Room>(entity =>
            {
                entity.HasKey(r => r.RoomId);
                entity.Property(r => r.RoomNumber).IsRequired().HasMaxLength(20);
                entity.Property(r => r.Price).HasColumnType("decimal(18,2)");

                entity.HasOne(r => r.Hotel)
                      .WithMany(h => h.Rooms)
                      .HasForeignKey(r => r.HotelId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.RoomType)
                      .WithMany(rt => rt.Rooms)
                      .HasForeignKey(r => r.RoomTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Unique RoomNumber trong cùng 1 Hotel
                entity.HasIndex(r => new { r.HotelId, r.RoomNumber }).IsUnique();
            });

            // ===== Booking =====
            builder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.BookingId);
                entity.Property(b => b.BookingCode).IsRequired().HasMaxLength(20);
                entity.HasIndex(b => b.BookingCode).IsUnique();

                entity.Property(b => b.TotalRoomPrice).HasColumnType("decimal(18,2)");
                entity.Property(b => b.TotalServicePrice).HasColumnType("decimal(18,2)");
                entity.Property(b => b.DiscountAmount).HasColumnType("decimal(18,2)");
                entity.Property(b => b.FinalAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(b => b.User)
                      .WithMany(u => u.Bookings)
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Hotel)
                      .WithMany(h => h.Bookings)
                      .HasForeignKey(b => b.HotelId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Promotion)
                      .WithMany(p => p.Bookings)
                      .HasForeignKey(b => b.PromotionId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ===== BookingDetail =====
            builder.Entity<BookingDetail>(entity =>
            {
                entity.HasKey(bd => bd.BookingDetailId);
                entity.Property(bd => bd.Price).HasColumnType("decimal(18,2)");
                entity.Property(bd => bd.SubTotal).HasColumnType("decimal(18,2)");

                entity.HasOne(bd => bd.Booking)
                      .WithMany(b => b.BookingDetails)
                      .HasForeignKey(bd => bd.BookingId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(bd => bd.Room)
                      .WithMany(r => r.BookingDetails)
                      .HasForeignKey(bd => bd.RoomId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== Service =====
            builder.Entity<Service>(entity =>
            {
                entity.HasKey(s => s.ServiceId);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(150);
                entity.Property(s => s.Price).HasColumnType("decimal(18,2)");
            });

            // ===== BookingService =====
            builder.Entity<BookingService>(entity =>
            {
                entity.HasKey(bs => bs.BookingServiceId);
                entity.Property(bs => bs.Price).HasColumnType("decimal(18,2)");
                entity.Property(bs => bs.SubTotal).HasColumnType("decimal(18,2)");

                entity.HasOne(bs => bs.Booking)
                      .WithMany(b => b.BookingServices)
                      .HasForeignKey(bs => bs.BookingId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(bs => bs.Service)
                      .WithMany(s => s.BookingServices)
                      .HasForeignKey(bs => bs.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== Payment =====
            builder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.PaymentId);
                entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");

                entity.HasOne(p => p.Booking)
                      .WithOne(b => b.Payment)
                      .HasForeignKey<Payment>(p => p.BookingId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== Invoice =====
            builder.Entity<Invoice>(entity =>
            {
                entity.HasKey(i => i.InvoiceId);
                entity.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(30);
                entity.HasIndex(i => i.InvoiceNumber).IsUnique();

                entity.Property(i => i.RoomAmount).HasColumnType("decimal(18,2)");
                entity.Property(i => i.ServiceAmount).HasColumnType("decimal(18,2)");
                entity.Property(i => i.Discount).HasColumnType("decimal(18,2)");
                entity.Property(i => i.Total).HasColumnType("decimal(18,2)");

                entity.HasOne(i => i.Booking)
                      .WithOne(b => b.Invoice)
                      .HasForeignKey<Invoice>(i => i.BookingId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== Promotion =====
            builder.Entity<Promotion>(entity =>
            {
                entity.HasKey(p => p.PromotionId);
                entity.Property(p => p.Code).IsRequired().HasMaxLength(50);
                entity.HasIndex(p => p.Code).IsUnique();
                entity.Property(p => p.DiscountValue).HasColumnType("decimal(18,2)");
                entity.Property(p => p.MinimumAmount).HasColumnType("decimal(18,2)");
                entity.Property(p => p.MaxDiscount).HasColumnType("decimal(18,2)");
            });

            // ===== Review =====
            builder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.ReviewId);

                entity.HasOne(r => r.User)
                      .WithMany(u => u.Reviews)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Hotel)
                      .WithMany(h => h.Reviews)
                      .HasForeignKey(r => r.HotelId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Room)
                      .WithMany()
                      .HasForeignKey(r => r.RoomId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(r => r.Booking)
                      .WithMany()
                      .HasForeignKey(r => r.BookingId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ===== Wishlist =====
            builder.Entity<Wishlist>(entity =>
            {
                entity.HasKey(w => w.WishlistId);

                entity.HasOne(w => w.User)
                      .WithMany(u => u.Wishlists)
                      .HasForeignKey(w => w.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(w => w.Hotel)
                      .WithMany(h => h.Wishlists)
                      .HasForeignKey(w => w.HotelId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Một user chỉ wishlist 1 hotel 1 lần
                entity.HasIndex(w => new { w.UserId, w.HotelId }).IsUnique();
            });

            // ===== Notification =====
            builder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.NotificationId);

                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== ActivityLog =====
            builder.Entity<ActivityLog>(entity =>
            {
                entity.HasKey(a => a.ActivityLogId);

                entity.HasOne(a => a.User)
                      .WithMany(u => u.ActivityLogs)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}