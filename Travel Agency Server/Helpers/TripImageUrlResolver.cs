using AutoMapper;
using AutoMapper.Execution;
using Travel_Agency_Server.DTO;
using Travel_Agency_Server.Model;

namespace Travel_Agency_Server.Helpers
{
    public class TripImageUrlResolver : IValueResolver<Trip, TripToReturnDTO, string>
    {

            private readonly IConfiguration _configuration;

            public TripImageUrlResolver(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public string Resolve(Trip source, TripToReturnDTO destination, string destMember, ResolutionContext context)
            {
                if (!string.IsNullOrEmpty(source.Image))
                {
                    return $"{_configuration["ApiBaseUrl"]}{source.Image}";
                }
                return string.Empty;
            
        }
    }
}
