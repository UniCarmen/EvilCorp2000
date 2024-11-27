using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RazorPagesSpielwiese.Entities;

public partial class Product
{
    [Key]
    public Guid ProductId { get; set; }

    [Required]
    [StringLength(100)]
    public string ProductName { get; set; } = null!;

    public string? ProductDescription { get; set; }

    public string? ProductPicture { get; set; }

    [Required]
    public int AmountOnStock { get; set; }

    public double? Rating { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 0)")]
    public decimal ProductPrice { get; set; }

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    //TODO: Status: Aktiv / nicht aktiv
}
