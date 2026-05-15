using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Models.Dtos.Responses.Evento;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoEventoResponsavel;
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
    public class EventoRepository(DataContext dataContext) : BaseRepository<Evento, Guid>(dataContext), IEventoRepository
    {
        public async Task<Evento?> ConsultarEventoComRelacionamentosAsync(Guid idEvento)
        {
            return await dataContext.Evento
                .Include(e => e.UsuarioCriacao)

                .Include(e => e.GrupoEventoResponsaveis)
                    .ThenInclude(r => r.Usuario)

                .Include(e => e.GrupoEventoEtiquetas)
                    .ThenInclude(x => x.Etiqueta)

                .FirstOrDefaultAsync(e => e.Id == idEvento);
        }

        public async Task<List<Evento>> GetKanbanAsync()
        {
            return await dataContext.Evento
                .Include(e => e.UsuarioCriacao)
                .Where(e => e.StatusGeralKanban != StatusGeralKanban.Cancelado)
                .OrderBy(e => e.DataInicial)
                .Take(200) // 🔥 limite
                .ToListAsync();
        }
        public async Task<PageResult<EventoPaginacaoResponse>> GetEventoPaginacaoAsync(
       int pageNumber,
       int pageSize,
       string? searchTerm = null)
        {
            // 1️⃣ Base query (🔥 CORREÇÃO AQUI)
            var query = dataContext.Evento
                .AsNoTracking()
                .Include(e => e.UsuarioCriacao) // ✅ resolve o null
                .AsQueryable();

            // 2️⃣ Filtro
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();

                query = query.Where(e =>
                    e.Titulo.ToLower().Contains(term) ||

                    (e.Endereco != null && e.Endereco.ToLower().Contains(term)) ||

                    (e.UsuarioCriacao != null &&
                     e.UsuarioCriacao.NomeUsuario.ToLower().Contains(term)) ||
                    dataContext.GrupoEventoResponsavel
                        .Where(gr => gr.EventoId == e.Id)
                        .Any(gr => gr.Usuario.NomeUsuario.ToLower().Contains(term))
                );
            }

            // 3️⃣ Total
            var totalCount = await query.CountAsync();

            // 4️⃣ Paginação
            var eventosPaged = await query
                .OrderBy(e => e.DataInicial)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var eventosIds = eventosPaged.Select(e => e.Id).ToList();

            // 5️⃣ Buscar responsáveis
            var responsaveisDb = await dataContext.GrupoEventoResponsavel
                .Where(gr => eventosIds.Contains(gr.EventoId))
                .Select(gr => new GrupoEventoResponsavelTemp
                {
                    EventoId = gr.EventoId,
                    UsuarioId = gr.UsuarioId,
                    NomeUsuario = gr.Usuario.NomeUsuario
                })
                .ToListAsync();

            // 6️⃣ Agrupar
            var responsaveisPorEvento = responsaveisDb
                .GroupBy(r => r.EventoId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 7️⃣ Montagem final
            var items = eventosPaged.Select(e =>
            {
                var responsaveis = responsaveisPorEvento.ContainsKey(e.Id)
                    ? responsaveisPorEvento[e.Id]
                    : new List<GrupoEventoResponsavelTemp>();

                return new EventoPaginacaoResponse
                {
                    Id = e.Id,
                    Titulo = e.Titulo,
                    DataInicial = e.DataInicial,
                    HoraInicial = e.HoraInicial,
                    DataFinal = e.DataFinal,
                    HoraFinal = e.HoraFinal,
                    DiaInteiro = e.DiaInteiro,
                    Endereco = e.Endereco,
                    Modalidade = e.Modalidade,
                    StatusGeralKanban = e.StatusGeralKanban,

                    // ✅ AGORA NÃO VEM MAIS NULL
                    UsuarioCriacao = e.UsuarioCriacao == null
                        ? null
                        : new UsuarioResumoResponse(
                            e.UsuarioCriacao.Id,
                            e.UsuarioCriacao.NomeUsuario
                        ),

                    GrupoEventoResponsavel = responsaveis
                         .Select(r => new GrupoEventoResponsavelResponse
                         {
                             UsuarioId = r.UsuarioId,
                             NomeUsuario = r.NomeUsuario
                         })
                        .ToList()
                };
            }).ToList();

            // 8️⃣ Retorno
            return new PageResult<EventoPaginacaoResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Evento?> ObterCompletoPorIdAsync(Guid id)
        {
            return await dataContext.Evento
                .AsNoTracking()

                // 🔥 ADICIONAR ISSO
                .Include(t => t.Processo)
                .Include(t => t.Caso)
                .Include(t => t.Atendimento)               

                .Include(t => t.GrupoEventoResponsaveis)
                    .ThenInclude(x => x.Usuario)

                .Include(t => t.GrupoEventoEtiquetas)
                    .ThenInclude(x => x.Etiqueta)

                .FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<List<Evento>> ConsultarUltimosAsync(int quantidade)
        {
            return await dataContext.Evento
                .OrderByDescending(x => x.DataCadastro)
                .Take(quantidade)
                .ToListAsync();
        }
        public async Task<List<Evento>> ObterEventosLembreteAsync()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Today);

            return await dataContext.Evento
                .Where(e =>

                    // evento futuro
                    e.DataInicial >= hoje

                    ||

                    // evento recorrente ainda válido
                    (
                        e.DataFimRecorrencia.HasValue &&
                        e.DataFimRecorrencia >= hoje
                    )
                )
                .OrderBy(e => e.DataInicial)
                .ThenBy(e => e.HoraInicial)
                .ToListAsync();
        }
    }
}
