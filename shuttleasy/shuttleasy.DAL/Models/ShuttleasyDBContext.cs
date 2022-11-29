using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;


namespace shuttleasy.DAL.Models
{
    public partial class ShuttleasyDBContext : DbContext
    {
        public ShuttleasyDBContext()
        {
            var configBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configBuilder.AddJsonFile(path, false);
            var root = configBuilder.Build();
            var appSetting = root.GetSection("ConnectionStrings:DefaultConnection");
            SqlConnectionString = appSetting.Value;
        }
        public string? SqlConnectionString { get; set; }
        public ShuttleasyDBContext(DbContextOptions<ShuttleasyDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Company> Companies { get; set; } = null!;
        public virtual DbSet<CompanyWorker> CompanyWorkers { get; set; } = null!;
        public virtual DbSet<DriversStatistic> DriversStatistics { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Passenger> Passengers { get; set; } = null!;
        public virtual DbSet<PassengerPayment> PassengerPayments { get; set; } = null!;
        public virtual DbSet<PassengerRating> PassengerRatings { get; set; } = null!;
        public virtual DbSet<ResetPassword> ResetPasswords { get; set; } = null!;
        public virtual DbSet<SessionHistory> SessionHistories { get; set; } = null!;
        public virtual DbSet<SessionPassenger> SessionPassengers { get; set; } = null!;
        public virtual DbSet<ShuttleBu> ShuttleBus { get; set; } = null!;
        public virtual DbSet<ShuttleSession> ShuttleSessions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)

            {
                if (SqlConnectionString != null)
                {
                    optionsBuilder.UseSqlServer(SqlConnectionString);
                }
                else
                {
                    throw new ArgumentNullException(nameof(SqlConnectionString));
                }

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("company");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.City)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.Name)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Rating).HasColumnName("rating");
            });

            modelBuilder.Entity<CompanyWorker>(entity =>
            {
                entity.HasKey(e => e.IdentityNum)
                    .HasName("PK__company___79E7A3EABA87A9B7");

                entity.ToTable("company_worker");

                entity.HasIndex(e => e.PhoneNumber, "UQ__company___A1936A6B53516339")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__company___AB6E616452330D12")
                    .IsUnique();

                entity.Property(e => e.IdentityNum)
                    .HasMaxLength(11)
                    .HasColumnName("identity_num");

                entity.Property(e => e.CompanyId).HasColumnName("company_id");

                entity.Property(e => e.Email)
                    .HasMaxLength(30)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .HasMaxLength(11)
                    .HasColumnName("name");

                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");

                entity.Property(e => e.PasswordSalt).HasColumnName("password_salt");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .HasColumnName("phone_number");

                entity.Property(e => e.ProfilePic).HasColumnName("profile_pic");

                entity.Property(e => e.Surname)
                    .HasMaxLength(11)
                    .HasColumnName("surname");

                entity.Property(e => e.Token).HasColumnName("token");

                entity.Property(e => e.Verified).HasColumnName("verified");

                entity.Property(e => e.WorkerType)
                    .HasMaxLength(11)
                    .HasColumnName("worker_type");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyWorkers)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__company_w__compa__38996AB5");
            });

            modelBuilder.Entity<DriversStatistic>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("drivers_statistics");

                entity.Property(e => e.DriverId)
                    .HasMaxLength(11)
                    .HasColumnName("driver_id");

                entity.Property(e => e.RateCount).HasColumnName("rate_count");

                entity.Property(e => e.RatingAvg).HasColumnName("rating_avg");

                entity.Property(e => e.SessionId).HasColumnName("session_id");

                entity.Property(e => e.WorkingHours).HasColumnName("working_hours");

                entity.HasOne(d => d.Driver)
                    .WithMany()
                    .HasForeignKey(d => d.DriverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__drivers_s__drive__398D8EEE");

                entity.HasOne(d => d.Session)
                    .WithMany()
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__drivers_s__sessi__3A81B327");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("notification");

                entity.Property(e => e.Content)
                    .IsUnicode(false)
                    .HasColumnName("content");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date");

                entity.Property(e => e.Email)
                    .HasMaxLength(30)
                    .HasColumnName("email");

                entity.Property(e => e.NotificationType).HasColumnName("notification_type");

                entity.HasOne(d => d.EmailNavigation)
                    .WithMany()
                    .HasPrincipalKey(p => p.Email)
                    .HasForeignKey(d => d.Email)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__notificat__email__45F365D3");

                entity.HasOne(d => d.Email1)
                    .WithMany()
                    .HasPrincipalKey(p => p.Email)
                    .HasForeignKey(d => d.Email)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__notificat__email__46E78A0C");
            });

            modelBuilder.Entity<Passenger>(entity =>
            {
                entity.HasKey(e => e.IdentityNum)
                    .HasName("PK__passenge__79E7A3EAF799E413");

                entity.ToTable("passenger");

                entity.HasIndex(e => e.PhoneNumber, "UQ__passenge__A1936A6B80707688")
                    .IsUnique();

                entity.HasIndex(e => e.PhoneNumber, "UQ__passenge__A1936A6BACB43492")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__passenge__AB6E616498DDC5F9")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__passenge__AB6E6164B5AD972C")
                    .IsUnique();

                entity.Property(e => e.IdentityNum)
                    .HasMaxLength(11)
                    .HasColumnName("identity_num");

                entity.Property(e => e.City)
                    .HasMaxLength(15)
                    .HasColumnName("city");

                entity.Property(e => e.Email)
                    .HasMaxLength(30)
                    .HasColumnName("email");

                entity.Property(e => e.IsPayment).HasColumnName("is_payment");

                entity.Property(e => e.Name)
                    .HasMaxLength(20)
                    .HasColumnName("name");

                entity.Property(e => e.PassengerAddress).HasColumnName("passenger_address");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(512)
                    .HasColumnName("password_hash");

                entity.Property(e => e.PasswordSalt)
                    .HasMaxLength(512)
                    .HasColumnName("password_salt");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .HasColumnName("phone_number");

                entity.Property(e => e.ProfilePic).HasColumnName("profile_pic");

                entity.Property(e => e.Token).HasColumnName("token");

                entity.Property(e => e.QrString).HasColumnName("qr_string");

                entity.Property(e => e.Surname)
                    .HasMaxLength(20)
                    .HasColumnName("surname");

                entity.Property(e => e.Verified).HasColumnName("verified");
            });

            modelBuilder.Entity<PassengerPayment>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("passenger_payment");

                entity.Property(e => e.IsPaymentVerified).HasColumnName("is_payment_verified");

                entity.Property(e => e.PassengerIdNum)
                    .HasMaxLength(11)
                    .HasColumnName("passenger_id_num");

                entity.Property(e => e.PaymentDate)
                    .HasColumnType("datetime")
                    .HasColumnName("payment_date");

                entity.Property(e => e.ShuttleSessionId).HasColumnName("shuttle_session_id");

                entity.HasOne(d => d.PassengerIdNumNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.PassengerIdNum)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__passenger__passe__3B75D760");

                entity.HasOne(d => d.ShuttleSession)
                    .WithMany()
                    .HasForeignKey(d => d.ShuttleSessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__passenger__shutt__3C69FB99");
            });

            modelBuilder.Entity<PassengerRating>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("passenger_ratings");

                entity.Property(e => e.Comment).HasColumnName("comment");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date");

                entity.Property(e => e.PassengerIdentityNum)
                    .HasMaxLength(11)
                    .HasColumnName("passenger_identity_num");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.Property(e => e.SessionId).HasColumnName("session_id");

                entity.HasOne(d => d.PassengerIdentityNumNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.PassengerIdentityNum)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__passenger__passe__3D5E1FD2");

                entity.HasOne(d => d.Session)
                    .WithMany()
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__passenger__sessi__3E52440B");
            });

            modelBuilder.Entity<ResetPassword>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("reset_password");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date");

                entity.Property(e => e.Email)
                    .HasMaxLength(30)
                    .HasColumnName("email");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .HasColumnName("phone_number");

                entity.Property(e => e.ResetKey).HasColumnName("reset_key");

                entity.HasOne(d => d.EmailNavigation)
                    .WithMany()
                    .HasPrincipalKey(p => p.Email)
                    .HasForeignKey(d => d.Email)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__reset_pas__email__47DBAE45");

                entity.HasOne(d => d.Email1)
                    .WithMany()
                    .HasPrincipalKey(p => p.Email)
                    .HasForeignKey(d => d.Email)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__reset_pas__email__48CFD27E");

                entity.HasOne(d => d.PhoneNumberNavigation)
                    .WithMany()
                    .HasPrincipalKey(p => p.PhoneNumber)
                    .HasForeignKey(d => d.PhoneNumber)
                    .HasConstraintName("FK__reset_pas__phone__49C3F6B7");

                entity.HasOne(d => d.PhoneNumber1)
                    .WithMany()
                    .HasPrincipalKey(p => p.PhoneNumber)
                    .HasForeignKey(d => d.PhoneNumber)
                    .HasConstraintName("FK__reset_pas__phone__4AB81AF0");
            });

            modelBuilder.Entity<SessionHistory>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("session_history");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date");

                entity.Property(e => e.DriverId)
                    .HasMaxLength(11)
                    .HasColumnName("driver_id");

                entity.Property(e => e.Rate).HasColumnName("rate");

                entity.Property(e => e.RateCount).HasColumnName("rate_count");

                entity.Property(e => e.SessionId).HasColumnName("session_id");

                entity.HasOne(d => d.Driver)
                    .WithMany()
                    .HasForeignKey(d => d.DriverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__session_h__drive__3F466844");

                entity.HasOne(d => d.Session)
                    .WithMany()
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__session_h__sessi__403A8C7D");
            });

            modelBuilder.Entity<SessionPassenger>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("session_passengers");

                entity.Property(e => e.EstimatedPickupTime)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("estimated_pickup_time");

                entity.Property(e => e.PassengerIdentity)
                    .HasMaxLength(11)
                    .HasColumnName("passenger_identity");

                entity.Property(e => e.PickupLatitude)
                    .HasMaxLength(11)
                    .HasColumnName("pickup_latitude");

                entity.Property(e => e.PickupLongtitude)
                    .HasMaxLength(11)
                    .HasColumnName("pickup_longtitude");

                entity.Property(e => e.PickupOrderNum).HasColumnName("pickup_order_num");

                entity.Property(e => e.PickupState)
                    .HasMaxLength(15)
                    .HasColumnName("pickup_state");

                entity.Property(e => e.SessionDate)
                    .HasColumnType("date")
                    .HasColumnName("session_date");

                entity.Property(e => e.SessionId).HasColumnName("session_id");

                entity.HasOne(d => d.PassengerIdentityNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.PassengerIdentity)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__session_p__passe__412EB0B6");

                entity.HasOne(d => d.Session)
                    .WithMany()
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__session_p__sessi__4222D4EF");
            });

            modelBuilder.Entity<ShuttleBu>(entity =>
            {
                entity.ToTable("shuttle_bus");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BusModel)
                    .HasMaxLength(15)
                    .HasColumnName("bus_model");

                entity.Property(e => e.Capacity).HasColumnName("capacity");

                entity.Property(e => e.CompanyId).HasColumnName("company_id");

                entity.Property(e => e.LicensePlate)
                    .HasMaxLength(7)
                    .HasColumnName("license_plate");

                entity.Property(e => e.State).HasColumnName("state");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.ShuttleBus)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__shuttle_b__compa__4316F928");
            });

            modelBuilder.Entity<ShuttleSession>(entity =>
            {
                entity.ToTable("shuttle_session");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BusId).HasColumnName("bus_id");

                entity.Property(e => e.CompanyId).HasColumnName("company_id");

                entity.Property(e => e.EndingPoint)
                    .HasMaxLength(15)
                    .HasColumnName("ending_point");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.PassengerCount).HasColumnName("passenger_count");

                entity.Property(e => e.SessionDate)
                    .HasColumnType("date")
                    .HasColumnName("session_date");

                entity.Property(e => e.StartTime)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("start_time");

                entity.Property(e => e.StartingLatitude)
                    .HasMaxLength(11)
                    .HasColumnName("starting_latitude");

                entity.Property(e => e.StartingLongtitude)
                    .HasMaxLength(11)
                    .HasColumnName("starting_longtitude");

                entity.HasOne(d => d.Bus)
                    .WithMany(p => p.ShuttleSessions)
                    .HasForeignKey(d => d.BusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__shuttle_s__bus_i__440B1D61");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.ShuttleSessions)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__shuttle_s__compa__44FF419A");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
