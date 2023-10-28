using AutoMapper;
using ToDoListApi.Dtos;

namespace ToDoListApi.ExtensionServices
{
    public class MappingService : Profile
    {
        public MappingService()
        {
            CreateMap<ToDoListApi.Entities.Task, TaskDto>().ReverseMap();
        }
    }

}
