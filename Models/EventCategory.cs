using System;
using System.Collections.Generic;

namespace assignment3_b3w.Models;

public partial class EventCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
