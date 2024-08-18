using System;
using System.Collections.Generic;

namespace assignment3_b3w.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<Attendee> Attendees { get; set; } = new List<Attendee>();
}
