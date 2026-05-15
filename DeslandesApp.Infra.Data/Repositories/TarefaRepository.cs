using AutoMapper;
using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Models.Dtos.Responses.Etiquetas;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoTarefaResponsaveis;
using DeslandesApp.Domain.Models.Dtos.Responses.ListaTarefas;
using DeslandesApp.Domain.Models.Dtos.Responses.Processo;
using DeslandesApp.Domain.Models.Dtos.Responses.Tarefa;
using DeslandesApp.Domain.Models.Dtos.Responses.Usuarios;
using DeslandesApp.Domain.Models.Entities;
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
    public class TarefaRepository(DataContext dataContext) : BaseRepository<Tarefa, Guid>(dataContext), ITarefaRepository
    {
        public async Task<PageResult<TarefaPaginacaoResponse>> GetTarefaPaginacaoAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {

            var query = dataContext.Tarefas
       .AsNoTracking()

       .AsQueryable();

            // --- filtro ---
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(p =>
                       p.Descricao.ToLower().Contains(term) ||

                       // (p.Responsavel != null &&
                       //  p.Responsavel.NomeUsuario.ToLower().Contains(term)) ||
                       p.StatusGeralKanban.ToString().ToLower().Contains(term)

                               );
            }

            // --- total ---
            var totalCount = await query.CountAsync();

            var items = await query
         .OrderBy(u => u.DataTarefa)
         .Skip((pageNumber - 1) * pageSize)
         .Take(pageSize)
         .Select(u => new TarefaPaginacaoResponse
         {
             Id = u.Id,
             Descricao = u.Descricao,
             DataTarefa = u.DataTarefa,

             // 🔥 CRIADOR
             UsuarioCriacao = u.UsuarioCriacao == null ? null : new UsuarioResumoResponse(
                 u.UsuarioCriacao.Id,
                 u.UsuarioCriacao.NomeUsuario
             ),

             // 🔥 RESPONSÁVEIS (MUITO IMPORTANTE)
             GrupoTarefaResponsaveis = u.GrupoTarefaResponsaveis
    .Select(r => new GrupoTarefaResponsaveisResponse(
        r.UsuarioId,
        r.Usuario.Id,
        r.Usuario.NomeUsuario
    ))
    .ToList(),

             Prioridade = u.Prioridade,
             TipoVinculo = u.TipoVinculoId,
             StatusGeralKanban = u.StatusGeralKanban
         })
         .ToListAsync();


            return new PageResult<TarefaPaginacaoResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<Tarefa?> ConsultarComRelacionamentosAsync(Guid id)
        {
            return await dataContext.Tarefas
                .Include(t => t.UsuarioCriacao)
                .Include(t => t.GrupoTarefaResponsaveis)
                    .ThenInclude(r => r.Usuario)
                .Include(t => t.GrupoTarefasEtiquetas)
                    .ThenInclude(e => e.Etiqueta)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<List<Tarefa>> GetKanbanAsync()
        {
            return await dataContext.Tarefas
                .Include(t => t.UsuarioCriacao)
                .Include(t => t.GrupoTarefaResponsaveis)

                // 🔗 VÍNCULOS
                .Include(t => t.Processo)
                .Include(t => t.Caso)
                .Include(t => t.Atendimento)

                .Where(t => t.StatusGeralKanban != StatusGeralKanban.Cancelado)
                .OrderBy(t => t.StatusGeralKanban)
                .ThenBy(t => t.DataTarefa)
                .Take(200)
                .ToListAsync();
        }
        public async Task<Tarefa?> ObterCompletoPorIdAsync(Guid id)
        {
            return await dataContext.Tarefas
                .AsNoTracking()

                // 🔥 ADICIONAR ISSO
                .Include(t => t.Processo)
                .Include(t => t.Caso)
                .Include(t => t.Atendimento)

                .Include(t => t.ListasTarefa)

                .Include(t => t.GrupoTarefaResponsaveis)
                    .ThenInclude(x => x.Usuario)

                .Include(t => t.GrupoTarefasEtiquetas)
                    .ThenInclude(x => x.Etiqueta)

                .FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<List<Tarefa>> ConsultarUltimosAsync(int quantidade)
        {
            return await dataContext.Tarefas
                .AsNoTracking()
                .Include(t => t.UsuarioCriacao)
                .Include(t => t.GrupoTarefaResponsaveis)
                .Include(t => t.GrupoTarefasEtiquetas)
                    .ThenInclude(e => e.Etiqueta)
                .OrderByDescending(x => x.DataCadastro)
                .Take(quantidade)
                .ToListAsync();
        }
        public async Task<List<Tarefa>> ObterTarefasLembreteAsync(
      Guid? usuarioId,
      bool ehAdministrador
  )
        {
            var hoje = DateTime.Today;

            var query = dataContext.Tarefas
                .Include(t => t.GrupoTarefaResponsaveis)
                .AsQueryable();

            // ADMIN vê tudo
            if (!ehAdministrador)
            {
                query = query.Where(t =>

                    // criador
                    t.UsuarioCriacaoId == usuarioId

                    ||

                    // responsável
                    t.GrupoTarefaResponsaveis
                        .Any(r => r.UsuarioId == usuarioId)
                );
            }

            return await query
                .Where(t =>

                    t.DataTarefa.HasValue &&

                    t.DataTarefa.Value.Date >= hoje

                    &&

                    t.StatusGeralKanban != StatusGeralKanban.Concluido
                )
                .OrderBy(t => t.DataTarefa)
                .ToListAsync();
        }

        
    }
}
