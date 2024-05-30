using System;
using System.Collections.Generic;

namespace SampleProject.Models.ProductInventory;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Gender { get; set; }

    public string? Email { get; set; }

    public string? ProfileImage { get; set; }
}
