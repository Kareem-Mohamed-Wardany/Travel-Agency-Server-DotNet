using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Travel_Agency_Server.Model
{
    public class Trip
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please provide Trip Name")]
        [MinLength(5,ErrorMessage = "Trip Name must be at least 5 characters")]
        public string Name { get; set; } = String.Empty;

        [Required(ErrorMessage = "Please provide Description")]
        [MinLength(5, ErrorMessage = "Description must be at least 5 characters")]
        public string Description { get; set; } = String.Empty;

        [Required(ErrorMessage = "Please provide Location")]
        [MinLength(3, ErrorMessage = "Location must be at least 3 characters")]
        [MaxLength(20, ErrorMessage = "Location must be at most 20 characters")]
        public string Location { get; set; } = String.Empty;

        [Required(ErrorMessage = "Please provide Duration")]
        public string Duration { get; set; } = String.Empty;

        [Required(ErrorMessage = "Please provide Transportation")]
        [MinLength(3, ErrorMessage = "Transportation must be at least 3 characters")]
        [MaxLength(100, ErrorMessage = "Transportation must be at most 100 characters")]
        public string Transportation { get; set; } = String.Empty;

        public string? ExtraInfo { get; set; }

        [Required(ErrorMessage = "Please provide Image")]
        public string Image { get; set; } = String.Empty;

        [Required(ErrorMessage = "Please provide Price")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Please provide Depature Date")]
        public DateOnly DepartureDate { get; set; }

        [Required(ErrorMessage = "Please provide Available Seats")]
        public int AvailableSeats { get; set; }
    }
}
