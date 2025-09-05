using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EquipCheck.Models.DB;

public partial class DBContext : DbContext
{
    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActionLogs> ActionLogs { get; set; }

    public virtual DbSet<AssetsManagements> AssetsManagements { get; set; }

    public virtual DbSet<Departments> Departments { get; set; }

    public virtual DbSet<FormChecklistItems> FormChecklistItems { get; set; }

    public virtual DbSet<FormSubmissionItems> FormSubmissionItems { get; set; }

    public virtual DbSet<FormSubmissionLogs> FormSubmissionLogs { get; set; }

    public virtual DbSet<FormSubmissions> FormSubmissions { get; set; }

    public virtual DbSet<FormsManagements> FormsManagements { get; set; }

    public virtual DbSet<OmDetail> OmDetail { get; set; }

    public virtual DbSet<OptionManagements> OptionManagements { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActionLogs>(entity =>
        {
            entity.HasKey(e => e.Pid).HasName("PK__ActionLo__C5775520CD042CAB");

            entity.Property(e => e.Pid).HasColumnName("PID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Event).HasMaxLength(50);
            entity.Property(e => e.Ip)
                .HasMaxLength(50)
                .HasColumnName("IP");
        });

        modelBuilder.Entity<AssetsManagements>(entity =>
        {
            entity.HasKey(e => e.AssetUid).HasName("PK__AssetsMa__893D2BDE99947F2E");

            entity.Property(e => e.AssetUid)
                .ValueGeneratedNever()
                .HasColumnName("AssetUID");
            entity.Property(e => e.AssetCode).HasMaxLength(20);
            entity.Property(e => e.AssetName).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.Omduid).HasColumnName("OMDUID");
            entity.Property(e => e.PurchaseDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UserUid).HasColumnName("UserUID");
        });

        modelBuilder.Entity<Departments>(entity =>
        {
            entity.HasKey(e => e.DepartmentUid).HasName("PK__Departme__16865758588DEB91");

            entity.HasIndex(e => e.DepartmentName, "UQ__Departme__D949CC34D1492604").IsUnique();

            entity.Property(e => e.DepartmentUid)
                .ValueGeneratedNever()
                .HasColumnName("DepartmentUID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DepartmentName).HasMaxLength(50);
            entity.Property(e => e.ManagerUid).HasColumnName("ManagerUID");
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(1);
        });

        modelBuilder.Entity<FormChecklistItems>(entity =>
        {
            entity.HasKey(e => e.ChecklistItemUid).HasName("PK__FormChec__92A1C4181A6F1F7D");

            entity.Property(e => e.ChecklistItemUid)
                .ValueGeneratedNever()
                .HasColumnName("ChecklistItemUID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FormUid).HasColumnName("FormUID");
            entity.Property(e => e.ItemName).HasMaxLength(100);
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(1);

            entity.HasOne(d => d.FormU).WithMany(p => p.FormChecklistItems)
                .HasForeignKey(d => d.FormUid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FM_FChecklist");
        });

        modelBuilder.Entity<FormSubmissionItems>(entity =>
        {
            entity.HasKey(e => e.SubmissionItemUid).HasName("PK__FormSubm__67D8C0F7BA58F158");

            entity.Property(e => e.SubmissionItemUid)
                .ValueGeneratedNever()
                .HasColumnName("SubmissionItemUID");
            entity.Property(e => e.ChecklistItemUid).HasColumnName("ChecklistItemUID");
            entity.Property(e => e.IsChecked).HasDefaultValue(1);
            entity.Property(e => e.Remark).HasMaxLength(200);
            entity.Property(e => e.SubmissionUid).HasColumnName("SubmissionUID");

            entity.HasOne(d => d.SubmissionU).WithMany(p => p.FormSubmissionItems)
                .HasForeignKey(d => d.SubmissionUid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FS_FSItem");
        });

        modelBuilder.Entity<FormSubmissionLogs>(entity =>
        {
            entity.HasKey(e => e.SubmissionLogUid).HasName("PK__FormSubm__1B3D1B179BE066D9");

            entity.Property(e => e.SubmissionLogUid)
                .ValueGeneratedNever()
                .HasColumnName("SubmissionLogUID");
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.SubmissionUid).HasColumnName("SubmissionUID");
            entity.Property(e => e.UserUid).HasColumnName("UserUID");

            entity.HasOne(d => d.SubmissionU).WithMany(p => p.FormSubmissionLogs)
                .HasForeignKey(d => d.SubmissionUid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FS_FSLog");
        });

        modelBuilder.Entity<FormSubmissions>(entity =>
        {
            entity.HasKey(e => e.SubmissionUid).HasName("PK__FormSubm__CA4E92A86FD69050");

            entity.Property(e => e.SubmissionUid)
                .ValueGeneratedNever()
                .HasColumnName("SubmissionUID");
            entity.Property(e => e.AssestUid).HasColumnName("AssestUID");
            entity.Property(e => e.CheckDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DepartmentUid).HasColumnName("DepartmentUID");
            entity.Property(e => e.FormUid).HasColumnName("FormUID");
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.Tel).HasMaxLength(20);
            entity.Property(e => e.UserUid).HasColumnName("UserUID");
        });

        modelBuilder.Entity<FormsManagements>(entity =>
        {
            entity.HasKey(e => e.FormUid).HasName("PK__FormsMan__33A276AF4EECD426");

            entity.Property(e => e.FormUid)
                .ValueGeneratedNever()
                .HasColumnName("FormUID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FormName).HasMaxLength(100);
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.PeriodEnd).HasColumnType("datetime");
            entity.Property(e => e.PeriodStart).HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Year).HasMaxLength(10);
        });

        modelBuilder.Entity<OmDetail>(entity =>
        {
            entity.HasKey(e => e.Omduid).HasName("PK__OM_Detai__5FD07C5AE7EB7256");

            entity.ToTable("OM_Detail");

            entity.Property(e => e.Omduid)
                .ValueGeneratedNever()
                .HasColumnName("OMDUID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DetailName).HasMaxLength(50);
            entity.Property(e => e.ModifydDate).HasColumnType("datetime");
            entity.Property(e => e.Omuid).HasColumnName("OMUID");
            entity.Property(e => e.Remark).HasMaxLength(200);
            entity.Property(e => e.Status).HasDefaultValue(1);

            entity.HasOne(d => d.Omu).WithMany(p => p.OmDetail)
                .HasForeignKey(d => d.Omuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OM_OMDetail");
        });

        modelBuilder.Entity<OptionManagements>(entity =>
        {
            entity.HasKey(e => e.Omuid).HasName("PK__OptionMa__AF9B712DD2D64BB7");

            entity.Property(e => e.Omuid)
                .ValueGeneratedNever()
                .HasColumnName("OMUID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifydDate).HasColumnType("datetime");
            entity.Property(e => e.OptionName).HasMaxLength(50);
            entity.Property(e => e.Remark).HasMaxLength(200);
            entity.Property(e => e.Status).HasDefaultValue(1);
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.UserUid).HasName("PK__Users__A1F26A8A7CA8ED8B");

            entity.HasIndex(e => e.UserAccount, "UQ__Users__D68041C0658EBDAF").IsUnique();

            entity.Property(e => e.UserUid)
                .ValueGeneratedNever()
                .HasColumnName("UserUID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DepartmentUid).HasColumnName("DepartmentUID");
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(300);
            entity.Property(e => e.Remark).HasMaxLength(200);
            entity.Property(e => e.SignaturePath).HasMaxLength(200);
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Tel).HasMaxLength(20);
            entity.Property(e => e.UserAccount).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
