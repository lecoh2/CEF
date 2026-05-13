using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoNiveis;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoSetores;
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
    public class UsuarioRepository(DataContext dataContext): BaseRepository<Usuario, Guid>(dataContext), IUsuarioRepository
    {
        public async Task<PageResult<UsuarioPaginacaoResponse>> GetUsuariosComPaginadoAsync(
          int pageNumber,
          int pageSize,
          string? searchTerm = null)
        {
            var query = dataContext.Usuario
       .AsNoTracking()
       .Include(u => u.GrupoSetores)
           .ThenInclude(gs => gs.Setor)
       .Include(u => u.GrupoNiveis)
           .ThenInclude(gn => gn.Niveis)
       .AsQueryable();

            // --- filtro ---
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();

                query = query.Where(u =>
                    u.Login.ToLower().Contains(term) ||
                    u.NomeUsuario.ToLower().Contains(term) ||
                    u.GrupoSetores.Any(gs => gs.Setor != null && gs.Setor.NomeSetor.ToLower().Contains(term)) ||
                    u.GrupoNiveis.Any(gn => gn.Niveis != null && gn.Niveis.NomeNivel.ToLower().Contains(term))
                );
            }

            // --- total ---
            var totalCount = await query.CountAsync();

            var items = await query
       .OrderBy(u => u.NomeUsuario)
       .Skip((pageNumber - 1) * pageSize)
       .Take(pageSize)
       .Select(u => new UsuarioPaginacaoResponse(
           u.Id,
           u.NomeUsuario,
           u.Login,
           u.Status,
           u.DataCadastro ?? DateTime.MinValue,

           u.GrupoSetores
               .Where(gs => gs.Setor != null)
               .Select(gs => new GrupoSetorPaginacaoResponse(
                   gs.Setor.Id,
                   gs.Setor.NomeSetor
               ))
               .ToList(),

           u.GrupoNiveis
               .Where(gn => gn.Niveis != null)
               .Select(gn => new GrupoNivelPaginacaoResponse(
                   gn.Niveis.Id,
                   gn.Niveis.NomeNivel
               ))
               .ToList()
       ))
       .ToListAsync();


            return new PageResult<UsuarioPaginacaoResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        // Buscar usuário apenas pelo login
        public async Task<Usuario?> GetUsuarioByLoginAsync(string login)
        {
            return await dataContext.Set<Usuario>()
                .AsNoTracking()
                .Include(u => u.GrupoNiveis)
                    .ThenInclude(gn => gn.Niveis)
                .Include(u => u.GrupoSetores)
                    .ThenInclude(gs => gs.Setor)
                .Include(u => u.Fotos)
                .FirstOrDefaultAsync(u => u.Login == login);
        }
        public async Task<Usuario?> GetUsuariosComRelacionamentosPerfilAsync(Guid id)
        {
            return await dataContext.Usuario
                .Include(u => u.Pessoa)
                    .ThenInclude(p => p.Endereco) // ✅ Inclui o endereço da pessoa
                .Include(u => u.Pessoa)
                    //.ThenInclude(p => p.Sexo) // ✅ Inclui o sexo da pessoa
                .Include(u => u.GrupoSetores)
                    .ThenInclude(gs => gs.Setor)
                .Include(u => u.GrupoNiveis)
                    .ThenInclude(gn => gn.Niveis)
                .Include(u => u.Fotos) // ✅ traz a foto se houver
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<Usuario?> ObterCompletoPorIdAsync(Guid id)
        {
            return await dataContext.Usuario
                .AsNoTracking()
                .Where(x => x.Id == id)

                // FOTO
                .Include(x => x.Fotos)

                // SETORES
                .Include(x => x.GrupoSetores)
                    .ThenInclude(x => x.Setor)

                // NÍVEIS
                .Include(x => x.GrupoNiveis)
                    .ThenInclude(x => x.Niveis)

                // PESSOA
                .Include(x => x.Pessoa)

                .FirstOrDefaultAsync();
        }
        public async Task<Usuario> ConsultarUsuarioCompletoAsync(Guid idUsuario)
        {
            return await dataContext.Usuario

                // SETORES
                .Include(u => u.GrupoSetores)
                    .ThenInclude(gs => gs.Setor)

                // NÍVEIS
                .Include(u => u.GrupoNiveis)
                    .ThenInclude(gn => gn.Niveis)

                .FirstOrDefaultAsync(u => u.Id == idUsuario);
        }

    }
}
