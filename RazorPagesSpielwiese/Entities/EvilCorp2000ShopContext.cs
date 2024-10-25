using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RazorPagesSpielwiese.Entities;

public partial class EvilCorp2000ShopContext : DbContext
{
    public EvilCorp2000ShopContext()
    {
    }

    public EvilCorp2000ShopContext(DbContextOptions<EvilCorp2000ShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EvilCorp2000Shop;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Discount>(entity =>
        {
            //Def PK, wird automatisch von der DB generiert
            entity.HasKey(e => e.DiscountId).HasName("PK_Discounts_1");

            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(18, 0)");

            //ein Product kann mehrere discounts haben
            entity.HasOne(d => d.Product).WithMany(p => p.Discounts)
                //productid ist FK von Discounts
                .HasForeignKey(d => d.ProductId)
                //wenn product gelöscht, werden zugehörigen einträge auf null gesetzt
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Discounts_Products");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ProductClass)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.ProductPrice).HasColumnType("decimal(18, 0)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
