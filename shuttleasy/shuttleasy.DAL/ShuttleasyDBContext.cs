using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using shuttleasy.DAL.Models;

namespace shuttleasy.DAL
{
    public partial class ShuttleasyDBContext : DbContext
    {


        public ShuttleasyDBContext()
        {

        }

        public ShuttleasyDBContext(DbContextOptions<ShuttleasyDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<City> Cities { get; set; } = null!;
        public virtual DbSet<Company> Companies { get; set; } = null!;
        public virtual DbSet<CompanyWorker> CompanyWorkers { get; set; } = null!;
        public virtual DbSet<DriversStatistic> DriversStatistics { get; set; } = null!;
        public virtual DbSet<GeoPoint> GeoPoint { get; set; } = null!;   
        public virtual DbSet<NotificationPassenger> NotificationPassengers { get; set; } = null!;
        public virtual DbSet<NotificationWorker> NotificationWorkers { get; set; } = null!;
        public virtual DbSet<Passenger> Passengers { get; set; } = null!;
        public virtual DbSet<PassengerPayment> PassengerPayments { get; set; } = null!;
        public virtual DbSet<PassengerRating> PassengerRatings { get; set; } = null!;
        public virtual DbSet<PickupArea> PickupArea { get; set; } = null!;
        public virtual DbSet<PickupPoint> PickupPoint { get; set; } = null!;
        public virtual DbSet<ResetPassword> ResetPasswords { get; set; } = null!;
        public virtual DbSet<SessionHistory> SessionHistories { get; set; } = null!;
        public virtual DbSet<SessionPassenger> SessionPassengers { get; set; } = null!;
        public virtual DbSet<ShuttleBus> ShuttleBus { get; set; } = null!;
        public virtual DbSet<ShuttleSession> ShuttleSessions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=shuttleasydbserver1.database.windows.net;Initial Catalog=ShuttleasyDB;User ID=emreyilmaz;Password=Easypeasy1");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(e => e.CityNumber);

                entity.ToTable("city");

                entity.Property(e => e.CityNumber)
                    .ValueGeneratedNever()
                    .HasColumnName("city_number");

                entity.Property(e => e.Name)
                    .HasMaxLength(14)
                    .HasColumnName("name");
            });

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

                entity.Property(e => e.PhoneNumber)
                   .HasMaxLength(10)
                   .HasColumnName("phone_number");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");
                entity.Property(e => e.VotesNumber)
                    .HasColumnName("votes_number");
            });

            modelBuilder.Entity<CompanyWorker>(entity =>
            {
                entity.ToTable("company_worker");

                entity.HasIndex(e => e.PhoneNumber, "UQ__company___A1936A6B70D443BD")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__company___AB6E616465DFB48D")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

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


            });


            modelBuilder.Entity<DriversStatistic>(entity =>
            {
                entity.ToTable("drivers_statistics");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DriverId).HasColumnName("driver_id");

                entity.Property(e => e.RateCount).HasColumnName("rate_count");

                entity.Property(e => e.RatingAvg).HasColumnName("rating_avg");

                entity.Property(e => e.SessionId).HasColumnName("session_id");

                entity.Property(e => e.WorkingHours).HasColumnName("working_hours");

            });
            modelBuilder.Entity<GeoPoint>(entity =>
            {
                entity.ToTable("geopoint");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Latitude)
                    .HasMaxLength(50)
                    .HasColumnName("latitude");

                entity.Property(e => e.Longtitude)
                    .HasMaxLength(50)
                    .HasColumnName("longtitude");
                entity.Property(e => e.LocationName)
                   .HasMaxLength(100)
                   .HasColumnName("location_name");

            });


            modelBuilder.Entity<NotificationPassenger>(entity =>
            {
                entity.ToTable("notification_passenger");

                entity.Property(e => e.Id).HasColumnName("id");

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

            });

            modelBuilder.Entity<NotificationWorker>(entity =>
            {
                entity.ToTable("notification_worker");

                entity.Property(e => e.Id).HasColumnName("id");

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

            });

            modelBuilder.Entity<Passenger>(entity =>
            {
                entity.ToTable("passenger");

                entity.HasIndex(e => e.PhoneNumber, "UQ__passenge__A1936A6B6A226F13")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__passenge__AB6E6164501FF29A")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.City)
                    .HasMaxLength(15)
                    .HasColumnName("city");

                entity.Property(e => e.Email)
                    .HasMaxLength(30)
                    .HasColumnName("email");

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

                entity.Property(e => e.QrString).HasColumnName("qr_string");

                entity.Property(e => e.Surname)
                    .HasMaxLength(20)
                    .HasColumnName("surname");

                entity.Property(e => e.Token).HasColumnName("token");

                entity.Property(e => e.Verified).HasColumnName("verified");
            });

            modelBuilder.Entity<PassengerPayment>(entity =>
            {
                entity.ToTable("passenger_payment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IsPaymentVerified).HasColumnName("is_payment_verified");

                entity.Property(e => e.PassengerIdentity).HasColumnName("passenger_identity");

                entity.Property(e => e.PaymentDate)
                    .HasColumnType("datetime")
                    .HasColumnName("payment_date");

                entity.Property(e => e.ShuttleSessionId).HasColumnName("shuttle_session_id");

            });

            modelBuilder.Entity<PassengerRating>(entity =>
            {
                entity.ToTable("passenger_ratings");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment).HasColumnName("comment");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date");

                entity.Property(e => e.PassengerIdentity).HasColumnName("passenger_identity");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.Property(e => e.SessionId).HasColumnName("session_id");

            });
            modelBuilder.Entity<PickupArea>(entity =>
            {
                entity.ToTable("pickup_area");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.SessionId).HasColumnName("session_id");
                entity.Property(e => e.PolygonPoints).HasColumnName("polygon_points");

            });
            modelBuilder.Entity<PickupPoint>(entity =>
            {
                entity.ToTable("pickup_point");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.GeoPointId).HasColumnName("geopoint_id");

            });

            

            modelBuilder.Entity<ResetPassword>(entity =>
            {
                entity.ToTable("reset_password");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date");

                entity.Property(e => e.Email)
                    .HasMaxLength(30)
                    .HasColumnName("email");

                entity.Property(e => e.ResetKey)
                    .HasMaxLength(6)
                    .HasColumnName("reset_key");
            });

            modelBuilder.Entity<SessionHistory>(entity =>
            {
                entity.ToTable("session_history");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date");

                entity.Property(e => e.DriverId).HasColumnName("driver_id");

                entity.Property(e => e.Rate).HasColumnName("rate");

                entity.Property(e => e.RateCount).HasColumnName("rate_count");

                entity.Property(e => e.SessionId).HasColumnName("session_id");

            });

            modelBuilder.Entity<SessionPassenger>(entity =>
            {
                entity.ToTable("session_passengers");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.PickupId).HasColumnName("pickup_id");

                entity.Property(e => e.EstimatedPickupTime)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("estimated_pickup_time");

                

                entity.Property(e => e.PickupOrderNum).HasColumnName("pickup_order_num");

                entity.Property(e => e.PickupState)
                    .HasMaxLength(15)
                    .HasColumnName("pickup_state");


                entity.Property(e => e.SessionId).HasColumnName("session_id");


            });

            modelBuilder.Entity<ShuttleBus>(entity =>
            {
                entity.ToTable("shuttle_bus");

                entity.HasIndex(e => e.LicensePlate, "UQ__shuttle___F72CD56EA1FBA143")
                    .IsUnique();

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


            });

            modelBuilder.Entity<ShuttleSession>(entity =>
            {
                entity.ToTable("shuttle_session");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BusId).HasColumnName("bus_id");

                entity.Property(e => e.CompanyId).HasColumnName("company_id");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.PassengerCount).HasColumnName("passenger_count");

                entity.Property(e => e.StartTime)

                    .HasColumnName("start_time");

                entity.Property(e => e.DriverId).HasColumnName("driver_id");
                entity.Property(e => e.StartGeopoint).HasColumnName("start_geopoint");
                entity.Property(e => e.FinalGeopoint).HasColumnName("final_geopoint");
                entity.Property(e => e.DestinationName)
                .HasMaxLength(100)
                .HasColumnName("destination_name");
                entity.Property(e => e.Return).HasColumnName("return");
                entity.Property(e => e.SessionDate)
                .HasMaxLength(15)
                .HasColumnName("session_date");



            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
