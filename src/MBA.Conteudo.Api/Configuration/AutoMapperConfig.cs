using AutoMapper;
using MBA.API.ViewModels;
using MBA.Conteudo.Api.ViewModels;
using MBA.Conteudo.Application.ViewModels;
using MBA.Core.SharedDto;
using MBA.Core.SharedDto.Aluno;


namespace MBA.Conteudo.Api.Configuration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<AulaViewModel, AulaDto>();
            CreateMap<CadastroCursoViewModel, CadastroCursoDto>();
            CreateMap<AtualizacaoCursoViewModel, AtualizacaoCursoDto>();

            CreateMap<AlunoDto, AlunoViewModel>();
            CreateMap<MatriculaCursoDto, MatriculaCursoViewModel>();
            CreateMap<CertificadoDto, CertificadoViewModel>();

            CreateMap<EvolucaoAlunoDto, EvolucaoAlunoViewModel>();
            CreateMap<EvolucaoMatriculaCursoDto, EvolucaoMatriculaCursoViewModel>();

            CreateMap<AulaCursoDto, AulaCursoViewModel>();
        }
    }
}
