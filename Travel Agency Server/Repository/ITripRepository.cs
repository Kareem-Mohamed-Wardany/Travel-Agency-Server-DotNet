using Microsoft.AspNetCore.Identity;
using Travel_Agency_Server.Model;

namespace Travel_Agency_Server.Repository
{
    public interface ITripRepository
    {
        List<Trip> GetAllTrips(int Page, int ItemsPerPage);
        Trip? GetTripById(int Id);
        Task AddTripAsync(Trip NewTrip);
        Task DeleteTripAsync(int Id);
        Task UpdateTripAsync(int Id, Trip UpdatedTrip);
        Task<string> UploadImage(IFormFile imageFile);
        void DeleteImage(string Image);
        Task<int> TripsCountAsync();
        Task ReserveTripAsync(int TripId,string UserId);
        bool TripFoundById(int Id);
        Task CancelReservationAsync(int TripId, string UserId);
        List<Trip> AllUserReservedTrips(string UserId);
        List<IdentityUser> UsersReservedTrip(int tripId);
    }
}
