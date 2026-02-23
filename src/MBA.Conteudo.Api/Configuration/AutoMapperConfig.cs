using AutoMapper;
using MBA.Conteudo.Api.Models;
using MBA.Conteudo.Api.Models.ValueObjects;
using MBA.Conteudo.Api.ViewModels;

namespace MBA.Conteudo.Api.Configuration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            // Curso Mappings
            CreateMap<Curso, CursoViewModel>()
                .ForMember(dest => dest.CargaHoraria, opt => opt.MapFrom(src => src.CargaHoraria()))
                .ForMember(dest => dest.QuantidadeAulas, opt => opt.MapFrom(src => src.QuantidadeAulas()));

            CreateMap<CadastroCursoViewModel, Curso>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Ativo, opt => opt.Ignore())
                .ForMember(dest => dest.Aulas, opt => opt.Ignore());

            CreateMap<AtualizacaoCursoViewModel, Curso>()
                .ForMember(dest => dest.Ativo, opt => opt.Ignore())
                .ForMember(dest => dest.Aulas, opt => opt.Ignore());

            // ConteudoProgramatico Mappings
            CreateMap<ConteudoProgramatico, ConteudoProgramaticoViewModel>().ReverseMap();

            // Aula Mappings
            CreateMap<Aula, AulaResultViewModel>().ReverseMap();
            CreateMap<AdicionarAulaViewModel, Aula>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Ativo, opt => opt.Ignore())
                .ForMember(dest => dest.Curso, opt => opt.Ignore());

            CreateMap<AtualizarAulaViewModel, Aula>()
                .ForMember(dest => dest.Ativo, opt => opt.Ignore())
                .ForMember(dest => dest.Curso, opt => opt.Ignore());
        }
    }
}
