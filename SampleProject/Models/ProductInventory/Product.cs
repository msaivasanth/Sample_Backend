using System;
using System.Collections.Generic;

namespace SampleProject.Models.ProductInventory;

public partial class Product
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? Price { get; set; }

    public double? Rating { get; set; }

    public string? BrandId { get; set; }

    public string? CategoryId { get; set; }

    public string? Thumbnail { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Category? Category { get; set; }
}
