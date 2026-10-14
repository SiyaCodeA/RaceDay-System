using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.API.Models;

public partial class RaceDayContext : DbContext
{
    public RaceDayContext()
    {
    }

    public RaceDayContext(DbContextOptions<RaceDayContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Entry> Entries { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<Route> Routes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=RaceDayDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasOne(d => d.Event).WithMany(p => p.Categories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Category_Event");
        });

        modelBuilder.Entity<Entry>(entity =>
        {
            entity.Property(e => e.EntryDate).HasDefaultValueSql("(sysdatetime())", "DF_Entry_EntryDate");
            entity.Property(e => e.EntryStatus).HasDefaultValue("Pending", "DF_Entry_EntryStatus");

            entity.HasOne(d => d.Account).WithMany(p => p.Entries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Entry_Account");

            entity.HasOne(d => d.Event).WithMany(p => p.Entries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Entry_Event");

            entity.HasOne(d => d.Category).WithMany(p => p.Entries)
                .HasPrincipalKey(p => new { p.EventId, p.CategoryId })
                .HasForeignKey(d => new { d.EventId, d.CategoryId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Entry_Category");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasOne(d => d.Account).WithMany(p => p.Events)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Account");
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasOne(d => d.Entry).WithOne(p => p.Result)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Result_Entry");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasOne(d => d.Event).WithOne(p => p.Route)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Route_Event");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
