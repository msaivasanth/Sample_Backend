using System;
using System.Collections.Generic;

namespace SampleProject.Models.ProductInventory;

public partial class Image
{
    public int? Id { get; set; }

    public string? ImageUrl { get; set; }

    public virtual Product? IdNavigation { get; set; }
}
