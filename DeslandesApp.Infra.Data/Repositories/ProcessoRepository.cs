using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Models.Dtos.Responses.Atendimento;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoClienteProcesso;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoEnvolvidosProcesso;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoNiveis;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoSetores;
using DeslandesApp.Domain.Models.Dtos.Responses.Processo;
using DeslandesApp.Domain.Models.Dtos.Responses.Usuarios;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Utils;
using DeslandesApp.Infra.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Infra.Data.Repositories
{
    public class ProcessoRepository(DataContext dataContext) : BaseRepository<Processo, Guid>(dataContext), IProcessoRepository
    {

        public async Task<Processo?> ConsultarProcessoComRelacionamentosAsync(Guid idProcesso)
        {
            return await dataContext.Processos
    .Include(p => p.Vara)
        .ThenInclude(v => v.Foro) // 🔥 ESSENCIAL
    .Include(p => p.Acao)
    .Include(p => p.UsuarioResponsavel)
    .Include(p => p.GrupoClienteProcesso)
        .ThenInclude(c => c.Pessoa)
    .Include(p => p.GrupoEnvolvidosProcesso)
        .ThenInclude(e => e.Pessoa)
    .Include(p => p.GrupoEtiquetasProcessos)
        .ThenInclude(e => e.Etiqueta)
    .FirstOrDefaultAsync(p => p.Id == idProcesso);
        }
        public async Task<PageResult<ProcessoPaginacaoResponse>> GetProcessoPaginacaoAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {

            var query = dataContext.Processos
       .AsNoTracking()

       .AsQueryable();

            // --- filtro ---
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();

                query = query.Where(p =>
                    p.Pasta.ToLower().Contains(term) ||
                    p.Titulo.ToLower().Contains(term) ||
                    p.NumeroProcesso.Contains(term)

                );
            }

            // --- total ---
            var totalCount = await query.CountAsync();

            var items = await query
       .OrderBy(u => u.Pasta)
       .Skip((pageNumber - 1) * pageSize)
       .Take(pageSize)
       .Select(u => new ProcessoPaginacaoResponse(
           u.Id,
           u.Pasta,
           u.Titulo,
           u.NumeroProcesso
       ))
       .ToListAsync();


            return new PageResult<ProcessoPaginacaoResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<List<ProcessoAutoComplete>> ConsultarProcessoAutoCompleteAsync(string? termo = null)
        {
            var query = dataContext.Set<Processo>()
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(termo))
            {
                termo = termo.Trim();

                query = query.Where(p =>
                 
                    p.Pasta.Contains(termo)
                );
            }

            return await query
                .Select(p => new ProcessoAutoComplete
                {
                    Id = p.Id,
                   
                    Pasta = p.Pasta,
     
                })
                .OrderBy(p => p.Pasta)
                .Take(20)
                .ToListAsync();
        }
        public async Task<Processo?> ObterCompletoPorIdAsync(Guid id)
        {
            return await dataContext.Processos
                .AsNoTracking()
                .Where(x => x.Id == id)

                // CLIENTES
                .Include(x => x.GrupoClienteProcesso)
                    .ThenInclude(gc => gc.Pessoa)

                .Include(x => x.GrupoClienteProcesso)
                    .ThenInclude(gc => gc.QualificacaoCliente)

                // ENVOLVIDOS
                .Include(x => x.GrupoEnvolvidosProcesso)
                    .ThenInclude(ge => ge.Pessoa)

                .Include(x => x.GrupoEnvolvidosProcesso)
                    .ThenInclude(ge => ge.Qualificacao)

                // ETIQUETAS
                .Include(x => x.GrupoEtiquetasProcessos)
                    .ThenInclude(ge => ge.Etiqueta)

                // RELACIONAMENTOS
                .Include(x => x.UsuarioResponsavel)
                .Include(x => x.Acao)
                .Include(x => x.Vara)
                 .ThenInclude(v => v.Foro)

                .FirstOrDefaultAsync();
        }
        public async Task<List<ProcessoResumoResponse>> ConsultarUltimosAsync(int quantidade)
        {
            return await dataContext.Processos
     .AsNoTracking()
     .OrderByDescending(p => p.DataCadastro)
     .Take(quantidade)
     .Select(p => new ProcessoResumoResponse
     {
         Id = p.Id,
         Pasta = p.Pasta,
         NumeroProcesso = p.NumeroProcesso,
         Titulo = p.Titulo,

         GrupoClientesProcesso = p.GrupoClienteProcesso
             .Select(c => new GrupoClienteProcessoResponse
             {
                 IdPessoa = c.PessoaId,
                 Nome = c.Pessoa.Nome
             })
             .ToList(),

         GrupoEnvolvidosProcesso = p.GrupoEnvolvidosProcesso
             .Select(e => new GrupoEnvolvidosProcessoResponse
             {
                 IdPessoa = e.PessoaId,
                 Nome = e.Pessoa.Nome
             })
             .ToList(),

         DataCadastro = p.DataCadastro
     })
     .ToListAsync();
        }

        public async Task<List<ProcessoAgrupado>> GetGraficoProcessoAsync()
        {
            var anoAtual = DateTime.Now.Year;

            return await dataContext.Processos
                .Where(r => r.DataCadastro.Year == anoAtual)
                .GroupBy(r => new
                {
                    Mes = r.DataCadastro.Month,
                   
                })
                .Select(g => new ProcessoAgrupado
                {
                    Mes = g.Key.Mes,                   
                    Quantidade = g.Count()
                })
                .OrderBy(g => g.Mes)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<int> ContarProcessoAnoAtual()
        {
            var inicioAno = new DateTime(DateTime.Now.Year, 1, 1);
            var fimAno = new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59);

            return await dataContext.Processos
                .Where(p => p.DataCadastro >= inicioAno && p.DataCadastro <= fimAno)
                .CountAsync();
        }
        public Task<int> ContarTotal()
        {
            return dataContext.Processos.CountAsync();
        }
        public async Task<Processo?> ObterPorNumeroAsync(string numeroProcesso)
        {
            return await dataContext.Processos
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.NumeroProcesso == numeroProcesso);
        }


    }
}
