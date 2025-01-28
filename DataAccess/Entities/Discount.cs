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
    //die Product-Entität erst dann aus der Datenbank geladen wird, wenn du auf die Product-Eigenschaft zugreifst.
    public virtual Product? Product { get; set; }
}