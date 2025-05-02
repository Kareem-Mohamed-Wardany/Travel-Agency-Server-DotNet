using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel_Agency_Server.DTO;
using Travel_Agency_Server.Errors;
using Travel_Agency_Server.Model;
using Travel_Agency_Server.Repository;

namespace Travel_Agency_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ITripRepository _tripRepo;
        private readonly IMapper _mapper;

        public AdminController(ITripRepository tripRepo, IMapper mapper)
        {
            _tripRepo = tripRepo;
            _mapper = mapper;
        }

        [HttpPost("addtrip")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> AddNewTrip([FromForm] TripDTO NewTrip, IFormFile imageFile)
        {
            string imageURL = await _tripRepo.UploadImage(imageFile);
            if (imageURL == null) return NotFound(new ApiResponse(404, "Image cannot be found"));
            NewTrip.Image = imageURL;

            var trip = _mapper.Map<Trip>(NewTrip);
            await _tripRepo.AddTripAsync(trip);
            return Ok(new ApiResponse(201, "Trip Created Successfully!", trip));
        }
        
        [HttpDelete("deletetrip")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> DeleteTrip(int Id)
        {
            if (!_tripRepo.TripFoundById(Id)) return NotFound(new ApiResponse(404, "Trip cannot be found"));
            await _tripRepo.DeleteTripAsync(Id);
            return Ok(new ApiResponse(200,"Trip Delete Successfully!"));

        }

        [HttpPut("updatetrip/{Id:int}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UpdateTrip([FromRoute]int Id, [FromForm] TripDTO UpdatedTripDTO, IFormFile imageFile)
        {
            var trip = _tripRepo.GetTripById(Id);
            if (trip == null) return NotFound(new ApiResponse(404, "Trip cannot be found"));
            string imageURL = await  _tripRepo.UploadImage(imageFile);
            if(imageURL==null) return NotFound(new ApiResponse(404, "Image cannot be found"));
            UpdatedTripDTO.Image = imageURL;

            var TripFromDTO = _mapper.Map<Trip>(UpdatedTripDTO);
            await _tripRepo.UpdateTripAsync(Id, TripFromDTO);
            return Ok(new ApiResponse(200, "Trip Updated Successfully!",trip));

        }


        [HttpPost("tripusers")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UsersReservedTrip(int Id)
        {
            if (!_tripRepo.TripFoundById(Id)) return NotFound(new ApiResponse(404, "Trip cannot be found"));

            var Users =_mapper.Map<List<UserToReturnDTO>>( _tripRepo.UsersReservedTrip(Id));
            
            return Ok(new ApiResponse(200, "Users Retrevied Successfully!", Users));

        }


    }
}
