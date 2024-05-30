using System;
using System.Collections.Generic;

namespace SampleProject.Models.ProductInventory;

public partial class Login
{
    public int? Id { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public virtual User? IdNavigation { get; set; }
}
