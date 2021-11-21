using AutoMapper;
using Mediator.Commands;
using Mediator.Entities;
using Mediator.Queries;

namespace Mediator
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VehicleInfo, GetVehicleInfo.Response>();
            CreateMap<PlaceBid.Command, BidRequest>();
            CreateMap<BidRequest, PlaceBid.Command>();
        }
    }
}