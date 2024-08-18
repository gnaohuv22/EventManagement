using System;
using System.Collections.Generic;

namespace assignment3_b3w.Models;

public partial class Attendee
{
    public int AttendeeId { get; set; }

    public int EventId { get; set; }

    public int? UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public DateTime RegistrationTime { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User? User { get; set; }
}
