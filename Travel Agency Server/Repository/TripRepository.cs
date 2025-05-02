using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Travel_Agency_Server.DBContext;
using Travel_Agency_Server.Model;

namespace Travel_Agency_Server.Repository
{
    public class TripRepository : ITripRepository
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TripRepository(Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task AddTripAsync(Trip NewTrip)
        {
            await _context.Trips.AddAsync(NewTrip);
            await _context.SaveChangesAsync();
        }

        public void DeleteImage(string Image)
        {
            var imageRelativePath = Image.TrimStart('/'); // remove the leading slash
            var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, imageRelativePath);

            // Check if the old file exists, then delete it
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            
        }

        public async Task DeleteTripAsync(int Id)
        {
            var trip = GetTripById(Id);

            if (!string.IsNullOrEmpty(trip.Image))
            {
                DeleteImage(trip.Image);
            }
            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();
        }

        public List<Trip> GetAllTrips(int Page, int ItemsPerPage)
          => _context.Trips.Skip((Page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();  
        

        public Trip? GetTripById(int Id) => _context.Trips.FirstOrDefault(x => x.Id == Id);


        public async Task UpdateTripAsync(int Id, Trip UpdatedTrip)
        {
            var trip = GetTripById(Id);
            if (!string.IsNullOrEmpty(trip.Image))
            {
                DeleteImage(trip.Image);
            }
            trip.Name = UpdatedTrip.Name;
            trip.Description = UpdatedTrip.Description;
            trip.Location = UpdatedTrip.Location;
            trip.Duration = UpdatedTrip.Duration;
            trip.Description = UpdatedTrip.Description;
            trip.DepartureDate = UpdatedTrip.DepartureDate;
            trip.Image = UpdatedTrip.Image;
            trip.Price = UpdatedTrip.Price;
            trip.ExtraInfo = UpdatedTrip.ExtraInfo;
            trip.Transportation = UpdatedTrip.Transportation;
            trip.AvailableSeats = UpdatedTrip.AvailableSeats;
            await _context.SaveChangesAsync();
        }

        public async Task<string> UploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return "";


            // Generate a unique file name
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            string file = "/images/" + fileName;
            return file;
        }

        public async Task<int> TripsCountAsync()
        => await _context.Trips.CountAsync();

        public async Task ReserveTripAsync(int TripId, string UserId)
        {
            var trip = GetTripById(TripId);

            trip.AvailableSeats -= 1;

            var TripReservation = new TripsReverved();
            TripReservation.TripId = TripId;
            TripReservation.UserId = UserId;

            await _context.TripsReverved.AddAsync(TripReservation);
            await _context.SaveChangesAsync();


        }

        public bool TripFoundById(int Id)
        {
            return GetTripById(Id) != null;
        }

        public async Task CancelReservationAsync(int TripId, string UserId)
        {
            var trip = GetTripById(TripId);

            trip.AvailableSeats += 1;

            var ReservedTrip = await _context.TripsReverved.FirstOrDefaultAsync(t => t.TripId == TripId && t.UserId == UserId);

            _context.TripsReverved.Remove(ReservedTrip);
            await _context.SaveChangesAsync();
        }

        public List<Trip> AllUserReservedTrips(string UserId)
        {
            return _context.TripsReverved
            .Where(tr => tr.UserId == UserId)
            .Include(tr => tr.Trip)
            .Select(tr => tr.Trip!)
            .ToList();
        }

        public List<IdentityUser> UsersReservedTrip(int tripId)
        {
            return _context.TripsReverved
            .Where(tr => tr.TripId == tripId)
            .Include(tr => tr.User)
            .Select(tr => tr.User!)
            .ToList();
        }
    }
}
