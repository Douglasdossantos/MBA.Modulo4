using AutoMapper;
using MBA.Conteudo.Application.ViewModels;
using MBA.Conteudo.Domain.Entities;


namespace SaberOnline.Conteudo.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Curso, CursoViewModel>();
        }
    }
}
