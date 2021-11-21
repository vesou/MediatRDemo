using Mediator.Commands;
using Mediator.Entities;
using Mediator.Queries;

namespace Mediator
{
    public partial class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<VehicleInfo, GetVehicleInfo.Response>();
            CreateMap<PlaceBid.Command, BidRequest>();
        }
    }
}