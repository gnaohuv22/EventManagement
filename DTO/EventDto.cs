using System.ComponentModel.DataAnnotations;

namespace assignment3_b3w.DTO
{
    public class EventDto
    {
        public int EventId { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string? Description { get; set; }
        [Required]
        public DateTime? StartTime { get; set; }
        [Required]
        public DateTime? EndTime { get; set; }
        [Required]
        public string? Location { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
