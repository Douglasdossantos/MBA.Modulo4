using AutoMapper;
using MBA.Aluno.Appplication.Interfaces;
using MBA.Aluno.Domain.Interface;
using MBA.Core.SharedDto.Aluno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Aluno.Appplication.Queries
{
    public class AlunoQueryService : IAlunoQuery
    {
        private readonly IAlunoRepository _alunoRepository;
        //private readonly ICursoRepository _cursoRepository;
        private readonly IMatriculaRepository _matriculaRepository;
        private readonly IMapper _mapper;

        public AlunoQueryService(IAlunoRepository alunoRepository,
            IMatriculaRepository matriculaRepository,
            //ICursoRepository cursoRepository,
                            IMapper mapper)
        {
            //_cursoRepository = cursoRepository;
            _matriculaRepository = matriculaRepository;
            _mapper = mapper;
            _alunoRepository = alunoRepository;
        }


        public async Task<MatriculaDto> EvolucaoCursoPorMatriculaAsync(Guid matriculaId)
        {
            var matricula = await _matriculaRepository.ObterPorIdAsync(matriculaId);
            if (matricula == null) return null;

            var aulasAssistidas = await _alunoRepository.AulasAssistidasPorMatricula(matriculaId);
            //var curso = await _cursoRepository.ObterPorIdAsync(matricula.CursoId);

            MatriculaDto matriculaDto = _mapper.Map<MatriculaDto>(matricula);

            //int totalAulas = (curso.Aulas != null) ? curso.Aulas.Count() : 0;
            //int totalAssistidas = (aulasAssistidas != null) ? aulasAssistidas.Count() : 0;

            //matriculaDto.TotalAulas = totalAulas;
            //matriculaDto.AulasAssistidas = totalAssistidas;
            //matriculaDto.AulasFaltantes = Math.Max(0, totalAulas - totalAssistidas);

            decimal calculo = 0;

            //if (totalAulas > 0)
                //calculo = ((decimal)totalAssistidas / totalAulas) * 100;

            matriculaDto.Porcentagem = Math.Round(calculo, 2);


            return matriculaDto;
        }



    }
}
