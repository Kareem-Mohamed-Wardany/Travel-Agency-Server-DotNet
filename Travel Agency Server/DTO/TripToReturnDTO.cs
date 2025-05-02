using System.ComponentModel.DataAnnotations;

namespace Travel_Agency_Server.DTO
{
    public class TripToReturnDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;

        public string Description { get; set; } = String.Empty;

        public string Location { get; set; } = String.Empty;

        public string Duration { get; set; } = String.Empty;

        public string Transportation { get; set; } = String.Empty;

        public string? ExtraInfo { get; set; }

        public string Image { get; set; } = String.Empty;

        public double Price { get; set; }

        public DateOnly DepartureDate { get; set; }

        public int AvailableSeats { get; set; }
    }
}
