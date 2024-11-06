using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RazorPagesSpielwiese.Entities;

public partial class Category
{
    [StringLength(50)]
    public string CategoryName { get; set; } = null!;

    [Key]
    public Guid CategoryId { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<ProductCategoryMapping> ProductCategoryMappings { get; set; } = new List<ProductCategoryMapping>();
}
