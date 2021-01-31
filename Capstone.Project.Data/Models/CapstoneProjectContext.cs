using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class CapstoneProjectContext : DbContext
    {
        public CapstoneProjectContext()
        {
        }

        public CapstoneProjectContext(DbContextOptions<CapstoneProjectContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<PhotoCategory> PhotoCategories { get; set; }
        public virtual DbSet<PhotoReport> PhotoReports { get; set; }
        public virtual DbSet<PhotoReportDetail> PhotoReportDetails { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<RequestDeletePhoto> RequestDeletePhotos { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Type> Types { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-329BBUF;Database=CapstoneProject;User Id=sa;Password=123456;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryName).HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(100);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).ValueGeneratedNever();

                entity.Property(e => e.InsDateTime).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Order_User");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.PhotoId });

                entity.ToTable("OrderDetail");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_Order");

                entity.HasOne(d => d.Photo)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.PhotoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_Photo");
            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.ToTable("Photo");

                entity.Property(e => e.InsDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PhotoName).HasMaxLength(50);

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.UserId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Wmlink).HasColumnName("WMLink");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Photos)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK_Photo_Type");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Photos)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Photo_User");
            });

            modelBuilder.Entity<PhotoCategory>(entity =>
            {
                entity.HasKey(e => new { e.CategoryId, e.PhotoId });

                entity.ToTable("PhotoCategory");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.PhotoCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PhotoCategory_Category");

                entity.HasOne(d => d.Photo)
                    .WithMany(p => p.PhotoCategories)
                    .HasForeignKey(d => d.PhotoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PhotoCategory_Photo");
            });

            modelBuilder.Entity<PhotoReport>(entity =>
            {
                entity.ToTable("PhotoReport");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.UserId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Photo)
                    .WithMany(p => p.PhotoReports)
                    .HasForeignKey(d => d.PhotoId)
                    .HasConstraintName("FK_PhotoReport_Photo");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PhotoReports)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_PhotoReport_User");
            });

            modelBuilder.Entity<PhotoReportDetail>(entity =>
            {
                entity.HasKey(e => new { e.PhotoReportId, e.ReportId });

                entity.HasOne(d => d.PhotoReport)
                    .WithMany(p => p.PhotoReportDetails)
                    .HasForeignKey(d => d.PhotoReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PhotoReportDetails_PhotoReport");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.PhotoReportDetails)
                    .HasForeignKey(d => d.ReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PhotoReportDetails_Report");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Report");

                entity.Property(e => e.ReportReason).HasMaxLength(50);
            });

            modelBuilder.Entity<RequestDeletePhoto>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.PhotoId });

                entity.ToTable("RequestDeletePhoto");

                entity.Property(e => e.UserId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Photo)
                    .WithMany(p => p.RequestDeletePhotos)
                    .HasForeignKey(d => d.PhotoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestDeletePhoto_Photo");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RequestDeletePhotos)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestDeletePhoto_User");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Transaction_User");
            });

            modelBuilder.Entity<Type>(entity =>
            {
                entity.ToTable("Type");

                entity.Property(e => e.TypeName).HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.Username, "IX_User")
                    .IsUnique();

                entity.Property(e => e.UserId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DayOfBirth).HasColumnType("date");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.EncryptCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .IsFixedLength(true);

                entity.Property(e => e.SuspendTime).HasColumnType("date");

                entity.Property(e => e.Username).HasMaxLength(50);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_User_Role");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
