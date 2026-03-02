using AutoMapper;
using MBA.Aluno.Appplication.Interfaces;
using MBA.Aluno.Appplication.ViewModel;
using MBA.Aluno.Domain.Entities;
using MBA.Aluno.Domain.Interface;
using MBA.Core.DomainObjects;
using MBA.Core.SharedDto.Aluno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Aluno.Appplication.Services
{
    public class MatriculaAppService : IMatriculaAppService
    {
        private readonly IMatriculaRepository _matriculaRepository;
        private readonly IMapper _mapper;

        public MatriculaAppService(IMatriculaRepository matriculaRepository,
                            IMapper mapper)
        {
            _mapper = mapper;
            _matriculaRepository = matriculaRepository;
        }

        public async Task<Guid> CadastrarMatriculaAsync(MatriculaViewModel dto)
        {


            var matricula = _mapper.Map<Matricula>(dto);
            matricula.CriarData();
            matricula.statusPendentePagamento();
            await _matriculaRepository.AdicionarAsync(matricula);

            await _matriculaRepository.UnitOfWork.Commit();
            return matricula.Id;
        }

        public async Task<MatriculaDto> ObterPorIdAsync(Guid matriculaId)
        {
            var matricula = await _matriculaRepository.ObterPorIdAsync(matriculaId)
                         ?? throw new DomainException("matricula não encontrado");

            return _mapper.Map<MatriculaDto>(matricula);
        }


    }
}
