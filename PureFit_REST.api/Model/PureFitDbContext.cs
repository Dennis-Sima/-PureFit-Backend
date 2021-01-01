using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PureFit_REST.api.Model
{
    public partial class PureFitDbContext : DbContext
    {
        public PureFitDbContext()
        {
        }

        public PureFitDbContext(DbContextOptions<PureFitDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Essen_history> Essen_history { get; set; }
        public virtual DbSet<Fitness_Uebungen> Fitness_Uebungen { get; set; }
        public virtual DbSet<Fitness_history> Fitness_history { get; set; }
        public virtual DbSet<Kunde> Kunde { get; set; }
        public virtual DbSet<Muskelgruppe> Muskelgruppe { get; set; }
        public virtual DbSet<Schwierigkeitsgrad> Schwierigkeitsgrad { get; set; }
        public virtual DbSet<Trainingslevel> Trainingslevel { get; set; }
        public virtual DbSet<User> User { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Essen_history>(entity =>
            {
                entity.Property(e => e.ES_Nr).ValueGeneratedOnAdd();

                entity.HasOne(d => d.ES_Kunde_NrNavigation)
                    .WithMany(p => p.Essen_history)
                    .HasForeignKey(d => d.ES_Kunde_Nr)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Fitness_Uebungen>(entity =>
            {
                entity.Property(e => e.FU_Nr).ValueGeneratedNever();

                entity.HasOne(d => d.FU_SchwierigkeitsgradNavigation)
                    .WithMany(p => p.Fitness_Uebungen)
                    .HasForeignKey(d => d.FU_Schwierigkeitsgrad)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Fitness_history>(entity =>
            {
                entity.HasKey(e => new { e.FH_Fitness_Uebungen_Nr, e.FH_Kunde_Nr, e.FH_Date });

                entity.HasOne(d => d.FH_Fitness_Uebungen_NrNavigation)
                    .WithMany(p => p.Fitness_history)
                    .HasForeignKey(d => d.FH_Fitness_Uebungen_Nr)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.FH_Kunde_NrNavigation)
                    .WithMany(p => p.Fitness_history)
                    .HasForeignKey(d => d.FH_Kunde_Nr)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Kunde>(entity =>
            {
                entity.HasKey(e => e.K_Nr);

                // Änderung auf ValueGeneratedOnAdd() da AUTOINCREMENT von Scaffold nicht gelesen wird.
                entity.Property(e => e.K_Nr)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.K_Vorname)
                        .IsRequired()
                        .HasColumnType("VARCHAR(200)");

                entity.Property(e => e.K_Zuname)
                       .IsRequired()
                       .HasColumnType("VARCHAR(200)");
                entity.Property(e => e.K_Geschlecht)
                       .IsRequired()
                       .HasColumnType("CHAR(1)");
                entity.Property(e => e.K_Groesse)
                       .IsRequired()
                       .HasColumnType("DECIMAL(3,2)");
                entity.Property(e => e.K_Gewicht)
                       .IsRequired()
                       .HasColumnType("DECIMAL(5,2)");
                entity.Property(e => e.K_GebDatum)
                      .IsRequired()
                      .HasColumnType("DATE");
                entity.Property(e => e.K_Email)

                      .HasColumnType("VARCHAR(200)");
                entity.Property(e => e.K_TelefonNr)
                      .HasColumnType("VARCHAR(45)");

                entity.HasOne(d => d.K_TrainingslevelNavigation)
                    .WithMany(p => p.Kunde)
                    .HasForeignKey(d => d.K_Trainingslevel)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Muskelgruppe>(entity =>
            {
                entity.Property(e => e.M_Nr).ValueGeneratedNever();
            });

            modelBuilder.Entity<Schwierigkeitsgrad>(entity =>
            {
                entity.Property(e => e.S_Nr).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Trainingslevel>(entity =>
            {
                entity.Property(e => e.tr_levelNr).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.U_ID);

                entity.HasIndex(e => e.U_Name)
                    .HasName("idx_u_name")
                    .IsUnique();

                // Änderung auf ValueGeneratedOnAdd() da AUTOINCREMENT von Scaffold nicht gelesen wird.
                entity.Property(e => e.U_ID)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.U_Hash)
                    .IsRequired()
                    .HasColumnType("CHAR(64)");

                entity.Property(e => e.U_Name)
                    .IsRequired()
                    .HasColumnType("VARCHAR(100)");

                entity.Property(e => e.U_Role).HasColumnType("VARCHAR(50)");

                entity.Property(e => e.U_Salt)
                    .IsRequired()
                    .HasColumnType("CHAR(32)");

                entity.HasOne(d => d.U_Kunde_NrNavigation)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.U_Kunde_Nr);

            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
