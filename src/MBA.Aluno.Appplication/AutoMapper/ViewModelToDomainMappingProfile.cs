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
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<AlunoViewModel, Domain.Entities.Aluno>();
            CreateMap<AtualizarAlunoViewModel, Domain.Entities.Aluno>();
            CreateMap<AlunoDto, Domain.Entities.Aluno>();


            CreateMap<MatriculaViewModel, Matricula>();
            CreateMap<MatriculaDto, Matricula>();


            CreateMap<CertificadoDto, Certificado>();


        }
    }
}
