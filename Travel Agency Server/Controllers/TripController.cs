using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel_Agency_Server.DTO;
using Travel_Agency_Server.Errors;
using Travel_Agency_Server.Repository;

namespace Travel_Agency_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ITripRepository _tripRepo;
        private readonly IMapper _mapper;

        public TripController(ITripRepository tripRepo, IMapper mapper)
        {
            _tripRepo = tripRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetAllTrips(int Page, int Limit)
        {
            if (Page <= 0 || Limit <= 0)
            {
                return BadRequest(new ApiResponse(400, "Page and Limit must be greater than 0."));
            }

            var totalTrips = await _tripRepo.TripsCountAsync();
            var totalPages = (int)Math.Ceiling(totalTrips / (double)Limit);

            if (Page > totalPages)
            {
                return BadRequest(new ApiResponse(400, "Page number exceeds the total number of pages."));
            }
            var trips = _mapper.Map<List<TripToReturnDTO>>(_tripRepo.GetAllTrips(Page, Limit));
            return Ok(new ApiResponse(200,"Trips Retrived Successfully!", new {Page= Page,Limit=Limit, trips=trips, totalPages= totalPages }));
        }

        [HttpPost("reserve/{Id:int}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> Reserve(int Id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(!_tripRepo.TripFoundById(Id)) return NotFound(new ApiResponse(404, "Trip cannot be found"));
            await _tripRepo.ReserveTripAsync(Id, userId);
            return Ok(new ApiResponse(200,"Trip Reserved Successfully!"));
        }
        [HttpPost("cancel/{Id:int}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Cancel(int Id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!_tripRepo.TripFoundById(Id)) return NotFound(new ApiResponse(404, "Trip cannot be found"));
            await _tripRepo.CancelReservationAsync(Id, userId);
            return Ok(new ApiResponse(200, "Trip Reservation canceled Successfully!"));
        }
        [HttpGet("reservations")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(401)]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Reservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var trips = _mapper.Map<List<TripToReturnDTO>>(_tripRepo.AllUserReservedTrips(userId));
            return Ok(new ApiResponse(200, "Reserved Trips Retrived Successfully!",trips));
        }
    }
}
