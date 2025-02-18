using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Models;
public partial class CategoryDTO
{
    [StringLength(50)]
    public string CategoryName { get; set; } = null!;

    [Key]
    public Guid CategoryId { get; set; }
}
