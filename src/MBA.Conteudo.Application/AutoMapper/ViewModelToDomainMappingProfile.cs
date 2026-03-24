using AutoMapper;
using MBA.Conteudo.Application.ViewModels;
using MBA.Conteudo.Domain.Entities;


namespace MBA.Conteudo.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<CursoViewModel, Curso>();
        }
    }
}