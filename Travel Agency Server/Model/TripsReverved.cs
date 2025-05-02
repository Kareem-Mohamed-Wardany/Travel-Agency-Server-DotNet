using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Travel_Agency_Server.Model
{
    public class TripsReverved
    {
        public int Id { get; set; }
        [ForeignKey("Trip")]
        public int TripId { get; set; }
        public Trip Trip { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public DateTime reservedAt { get; set; } = DateTime.UtcNow;
    }
}
