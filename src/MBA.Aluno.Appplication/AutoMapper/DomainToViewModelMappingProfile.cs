using AutoMapper;
using MBA.Aluno.Appplication.ViewModel;
using MBA.Aluno.Domain.Entities;
using MBA.Core.SharedDto;
using MBA.Core.SharedDto.Aluno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Aluno.Appplication.AutoMapper
{
    internal class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Domain.Entities.Aluno, AlunoViewModel>();
            CreateMap<Domain.Entities.Aluno, AtualizarAlunoViewModel>();
            CreateMap<Domain.Entities.Aluno, AlunoDto>();

            CreateMap<Matricula, MatriculaViewModel>();
            CreateMap<Matricula, MatriculaDto>();


            CreateMap<Certificado, CertificadoDto>();

        }
    }
}
