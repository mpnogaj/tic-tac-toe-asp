using AutoMapper;
using TicTacToe.Dto;
using TicTacToe.Models;

namespace TicTacToe;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Player, PlayerDto>();
        CreateMap<Room, RoomDto>()
            .ForMember(dest => dest.ReadyDict,
                opt =>
                    opt.MapFrom(x =>
                        new Dictionary<Guid, bool>(x.ReadyDict.Keys.Select(key =>
                            new KeyValuePair<Guid, bool>(key.Guid, x.ReadyDict[key])))));
    }
}