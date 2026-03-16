using AutoMapper;
using MBA.Aluno.Appplication.Interfaces;
using MBA.Aluno.Appplication.ViewModel;
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
    public class AlunoAppService : IAlunoAppService
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly IMapper _mapper;

        public AlunoAppService(IAlunoRepository alunoRepository,
                            IMapper mapper)
        {
            _mapper = mapper;
            _alunoRepository = alunoRepository;
        }

        public async Task<Guid> CadastrarAlunoAsync(AlunoViewModel dto)
        {
            if (await _alunoRepository.ExisteEmailAsync(dto.Email)) throw new DomainException("Email ja Existente");
            var aluno = _mapper.Map<Domain.Entities.Aluno>(dto);
            aluno.CriarData();
            await _alunoRepository.AdicionarAsync(aluno);

            await _alunoRepository.UnitOfWork.Commit();
            return aluno.Id;
        }

        public async Task AtualizarAlunoAsync(Guid alunoId, AtualizarAlunoViewModel dto)
        {
            var temAluno = await _alunoRepository.ObterPorIdAsync(alunoId) ?? throw new DomainException("Aluno não encontrado");
            var alunoEmail = await _alunoRepository.ObterPorEmailAsync(dto.Email);
            if (!alunoEmail.Id.Equals(alunoId)) throw new DomainException("Esse Email pertence a outro aluno");



            var aluno = _mapper.Map<Domain.Entities.Aluno>(dto);
            aluno.CriarDataDeixaAMesma(temAluno.DataCriacao);

            await _alunoRepository.AtualizarAsync(aluno);

            await _alunoRepository.UnitOfWork.Commit();
        }

        public async Task<AlunoDto> DesativarAlunoAsync(Guid AlunoId)
        {
            var aluno = await _alunoRepository.ObterPorIdAsync(AlunoId) ?? throw new DomainException("Aluno não encontrado");

            aluno.Desativar();
            await _alunoRepository.AtualizarAsync(aluno);
            await _alunoRepository.UnitOfWork.Commit();

            return _mapper.Map<AlunoDto>(aluno);
        }

        public async Task<AlunoDto> AtivarAlunoAsync(Guid alunoId)
        {
            var aluno = await _alunoRepository.ObterPorIdAsync(alunoId) ?? throw new DomainException("Curso não encontrado");


            aluno.Ativar();
            await _alunoRepository.AtualizarAsync(aluno);
            await _alunoRepository.UnitOfWork.Commit();

            return _mapper.Map<AlunoDto>(aluno);
        }

        public async Task<AlunoDto> ObterPorIdAsync(Guid alunoId)
        {
            var aluno = await _alunoRepository.ObterPorIdAsync(alunoId)
                         ?? throw new DomainException("Curso não encontrado");

            return _mapper.Map<AlunoDto>(aluno);
        }









    }
}
