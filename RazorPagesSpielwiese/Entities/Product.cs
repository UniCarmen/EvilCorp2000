using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ConstrainedExecution;

namespace RazorPagesSpielwiese.Entities;

public partial class Product
{
    [Key]
    public int ProductId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string ProductName { get; set; } = null!;

    public string? ProductDescription { get; set; }

    public string? ProductPicture { get; set; }
    
    [Required]
    public int AmountOnStock { get; set; }

    public int? Rating { get; set; }
    
    [Required]
    public decimal ProductPrice { get; set; }
    
    [Required]
    public int ProductClass { get; set; }
    //die Product-Entität erst dann aus der Datenbank geladen wird, wenn du auf die Product-Eigenschaft zugreifst.
    
    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}