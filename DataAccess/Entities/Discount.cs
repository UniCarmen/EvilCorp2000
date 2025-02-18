using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public partial class Discount
{
    [Key]
    [Required]
    public Guid ProductId { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Required]
    public double DiscountPercentage { get; set; }

    public Guid DiscountId { get; set; }
    //INFO: Product-Entität wird erst aus der DB geladen, wenn ich auf die Product-Eigenschaft zugreife
    public virtual Product? Product { get; set; }
}