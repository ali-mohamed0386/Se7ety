using Microsoft.EntityFrameworkCore;
using Se7ety.Api.Domain.Entities;

namespace Se7ety.Api.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<DoctorProfile> DoctorProfiles => Set<DoctorProfile>();
    public DbSet<PatientProfile> PatientProfiles => Set<PatientProfile>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Rating> Ratings => Set<Rating>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUsers(modelBuilder);
        ConfigurePatientProfiles(modelBuilder);
        ConfigureDoctorProfiles(modelBuilder);
        ConfigureAppointments(modelBuilder);
        ConfigureRatings(modelBuilder);
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<User>();

        entity.ToTable("Users");
        entity.HasKey(user => user.Id);

        entity.HasIndex(user => user.Email).IsUnique();

        entity.Property(user => user.Email)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(user => user.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();

        entity.Property(user => user.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entity.Property(user => user.CreatedAtUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasOne(user => user.PatientProfile)
            .WithOne(profile => profile.User)
            .HasForeignKey<PatientProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(user => user.DoctorProfile)
            .WithOne(profile => profile.User)
            .HasForeignKey<DoctorProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigurePatientProfiles(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<PatientProfile>();

        entity.ToTable("PatientProfiles");
        entity.HasKey(profile => profile.Id);

        entity.HasIndex(profile => profile.UserId).IsUnique();

        entity.Property(profile => profile.PhoneNumber).HasMaxLength(32);
        entity.Property(profile => profile.Bio).HasMaxLength(1000);
        entity.Property(profile => profile.ProfileImageUrl).HasMaxLength(500);
        entity.Property(profile => profile.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");
    }

    private static void ConfigureDoctorProfiles(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<DoctorProfile>();

        entity.ToTable("DoctorProfiles");
        entity.HasKey(profile => profile.Id);

        entity.HasIndex(profile => profile.UserId).IsUnique();
        entity.HasIndex(profile => profile.Specialty);

        entity.Property(profile => profile.Name).HasMaxLength(150);
        entity.Property(profile => profile.Specialty).HasMaxLength(100);
        entity.Property(profile => profile.Location).HasMaxLength(250);
        entity.Property(profile => profile.Phone).HasMaxLength(32);
        entity.Property(profile => profile.Price).HasPrecision(18, 2);
        entity.Property(profile => profile.Bio).HasMaxLength(1500);
        entity.Property(profile => profile.ProfileImageUrl).HasMaxLength(500);
        entity.Property(profile => profile.WorkingTimes).HasMaxLength(2000);
        entity.Property(profile => profile.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");
    }

    private static void ConfigureAppointments(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Appointment>();

        entity.ToTable("Appointments");
        entity.HasKey(appointment => appointment.Id);

        entity.HasIndex(appointment => new { appointment.DoctorProfileId, appointment.ScheduledAtUtc })
            .IsUnique()
            .HasFilter("[Status] IN ('Pending', 'Accepted')");

        entity.Property(appointment => appointment.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entity.Property(appointment => appointment.Notes).HasMaxLength(500);
        entity.Property(appointment => appointment.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasOne(appointment => appointment.PatientProfile)
            .WithMany(profile => profile.Appointments)
            .HasForeignKey(appointment => appointment.PatientProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(appointment => appointment.DoctorProfile)
            .WithMany(profile => profile.Appointments)
            .HasForeignKey(appointment => appointment.DoctorProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureRatings(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Rating>();

        entity.ToTable("Ratings", table => table.HasCheckConstraint("CK_Ratings_Value", "[Value] BETWEEN 1 AND 5"));
        entity.HasKey(rating => rating.Id);

        entity.HasIndex(rating => new { rating.PatientProfileId, rating.DoctorProfileId })
            .IsUnique();

        entity.Property(rating => rating.Comment).HasMaxLength(500);
        entity.Property(rating => rating.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasOne(rating => rating.PatientProfile)
            .WithMany(profile => profile.Ratings)
            .HasForeignKey(rating => rating.PatientProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(rating => rating.DoctorProfile)
            .WithMany(profile => profile.Ratings)
            .HasForeignKey(rating => rating.DoctorProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
