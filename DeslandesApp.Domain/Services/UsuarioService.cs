using AutoMapper;
using DeslandesApp.Domain.Contracts.Security;
using DeslandesApp.Domain.Helpers;
using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.Usuarios;
using DeslandesApp.Domain.Models.Dtos.Responses.Foto;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoNiveis;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoSetores;
using DeslandesApp.Domain.Models.Dtos.Responses.Nivel;
using DeslandesApp.Domain.Models.Dtos.Responses.Setor;
using DeslandesApp.Domain.Models.Dtos.Responses.Usuarios;
using DeslandesApp.Domain.Models.Dtos.Responses.Vara;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Utils;
using DeslandesApp.Domain.Validators;
using DeslandesApp.Domain.ValueObjects;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Services
{
    public class UsuarioService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IJwtTokenService _jwtTokenService, IHistoricoGeralService historicoGeralService)
        :  BaseService(httpContextAccessor), IUsuarioService
    {

        public async Task<UsuariosResponse> AdicionarAsync(UsuariosRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================
                // MAPEAR
                // =========================
                var usuario = mapper.Map<Usuario>(request);

                // =========================
                // EMAIL
                // =========================
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    usuario.ValorEmail =
                        new ValorEmail(request.Email.Trim());
                }

                // =========================
                // VALIDAÇÕES
                // =========================
                if (string.IsNullOrWhiteSpace(usuario.NomeUsuario))
                    throw new ApplicationException("Nome do usuário é obrigatório.");

                if (string.IsNullOrWhiteSpace(usuario.Login))
                    throw new ApplicationException("Login é obrigatório.");

                if (string.IsNullOrWhiteSpace(usuario.Senha))
                    throw new ApplicationException("Senha é obrigatória.");

                // =========================
                // NORMALIZAÇÃO
                // =========================
                usuario.NomeUsuario =
                    usuario.NomeUsuario.Trim();

                usuario.Login =
                    usuario.Login.Trim().ToLower();

                usuario.DataCadastro =
                    DateTime.Now;

                usuario.Status =
                    Status.Ativo;

                // =========================
                // VALIDATOR
                // =========================
                var validator = new UsuarioValidator();

                var result = validator.Validate(usuario);

                if (!result.IsValid)
                    throw new ValidationException(result.Errors);

                // =========================
                // CRIPTOGRAFIA
                // =========================
                usuario.Senha =
                    CryptoHelper.SHA256Encrypt(usuario.Senha);

                // =========================
                // VERIFICAR LOGIN
                // =========================
                var existenteLogin =
                    await unitOfWork.UsuarioRepository.GetByAsync(
                        u => u.Login == usuario.Login);

                if (existenteLogin != null)
                    throw new InvalidOperationException("Login já utilizado.");

                // =========================
                // VERIFICAR NOME
                // =========================
                var existenteNome =
                    await unitOfWork.UsuarioRepository.GetByAsync(
                        u => u.NomeUsuario == usuario.NomeUsuario);

                if (existenteNome != null)
                    throw new InvalidOperationException("Nome de usuário já utilizado.");

                // =========================
                // VERIFICAR EMAIL
                // =========================
                if (usuario.ValorEmail != null)
                {
                    var usuarios =
                        await unitOfWork.UsuarioRepository.GetAllAsync();

                    var emailExistente =
                        usuarios.FirstOrDefault(u =>

                            u.ValorEmail != null &&

                            u.ValorEmail.EnderecoEmail.ToLower()
                            ==
                            usuario.ValorEmail.EnderecoEmail.ToLower()
                        );

                    if (emailExistente != null)
                        throw new InvalidOperationException("E-mail já utilizado.");
                }

                // =========================
                // SALVAR USUÁRIO
                // =========================
                await unitOfWork.UsuarioRepository.AddAsync(usuario);

                // =========================
                // SALVAR PARA GERAR ID
                // =========================
          

                // =========================
                // GRUPOS SETOR
                // =========================
                if (request.GrupoSetor != null)
                {
                    foreach (var grupos in request.GrupoSetor)
                    {
                        var grupoSetores = new GrupoSetores
                        {
                            IdUsuario = usuario.Id,
                            IdSetor = grupos.IdSetor
                        };

                        await unitOfWork
                            .GrupoSetoresRepository
                            .AddAsync(grupoSetores);
                    }
                }

                // =========================
                // GRUPOS NÍVEL
                // =========================
                if (request.GrupoNivel != null)
                {
                    foreach (var grupos in request.GrupoNivel)
                    {
                        var grupoNiveis = new GrupoNiveis
                        {
                            IdUsuario = usuario.Id,
                            IdNivel = grupos.IdNivel
                        };

                        await unitOfWork
                            .GrupoNiveisRepository
                            .AddAsync(grupoNiveis);
                    }
                }

                // =========================
                // COMMIT FINAL
                // =========================
                await unitOfWork.CommitAsync();

                // =========================
                // RETORNO
                // =========================
                return mapper.Map<UsuariosResponse>(usuario);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task<UsuariosResponse> ModificarAsync(
     Guid id,
     UsuarioUpdateRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================
                // BUSCAR USUÁRIO
                // =========================
                var usuario = await unitOfWork.UsuarioRepository.GetByIdAsync(id);

                if (usuario == null)
                    throw new ApplicationException("Usuário não encontrado.");

                var usuarioId = ObterUsuarioId();

                // =========================
                // SNAPSHOT ANTES
                // =========================

                var usuarioAntes =
                    await unitOfWork.UsuarioRepository
                        .ConsultarUsuarioCompletoAsync(id);

                var dadosAntes = new
                {
                    usuarioAntes.NomeUsuario,
                    usuarioAntes.Login,

                    Email =
                        usuarioAntes.ValorEmail != null
                            ? usuarioAntes.ValorEmail.EnderecoEmail
                            : null,

                    usuarioAntes.Status,

                    Setores =
                        usuarioAntes.GrupoSetores?
                            .Select(x => x.Setor?.NomeSetor)
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToList(),

                    Niveis =
                        usuarioAntes.GrupoNiveis?
                            .Select(x => x.Niveis?.NomeNivel)
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToList()
                };

                // =========================
                // CAMPOS BÁSICOS
                // =========================

                usuario.NomeUsuario = request.NomeUsuario;

                usuario.ValorEmail =
                    new ValorEmail(request.Email);

                usuario.Login = request.Login;

                usuario.DataAtualizacao = DateTime.Now;

                usuario.Status = Status.Ativo;

                // =========================
                // SENHA
                // =========================

                bool senhaAlterada = false;

                if (!string.IsNullOrWhiteSpace(request.Senha))
                {
                    usuario.Senha =
                        CryptoHelper.SHA256Encrypt(request.Senha);

                    senhaAlterada = true;
                }

                // =========================
                // RESET GRUPOS SETORES
                // =========================

                await unitOfWork
                    .GrupoSetoresRepository
                    .RemoverPorUsuarioId(id);

                if (request.GrupoSetores?.Any() == true)
                {
                    foreach (var item in request.GrupoSetores)
                    {
                        await unitOfWork
                            .GrupoSetoresRepository
                            .AddAsync(new GrupoSetores
                            {
                                IdUsuario = id,
                                IdSetor = item.IdSetor
                            });
                    }
                }

                // =========================
                // RESET GRUPOS NÍVEIS
                // =========================

                await unitOfWork
                    .GrupoNiveisRepository
                    .RemoverPorUsuarioId(id);

                if (request.GrupoNiveis?.Any() == true)
                {
                    foreach (var item in request.GrupoNiveis)
                    {
                        await unitOfWork
                            .GrupoNiveisRepository
                            .AddAsync(new GrupoNiveis
                            {
                                IdUsuario = id,
                                IdNivel = item.IdNivel
                            });
                    }
                }

                // =========================
                // UPDATE
                // =========================

                await unitOfWork
                    .UsuarioRepository
                    .UpdateAsync(usuario);

                // =========================
                // SNAPSHOT DEPOIS
                // =========================

                var usuarioDepois =
                    await unitOfWork.UsuarioRepository
                        .ConsultarUsuarioCompletoAsync(id);

                var dadosDepois = new
                {
                    usuarioDepois.NomeUsuario,
                    usuarioDepois.Login,

                    Email =
                        usuarioDepois.ValorEmail != null
                            ? usuarioDepois.ValorEmail.EnderecoEmail
                            : null,

                    usuarioDepois.Status,

                    SenhaAlterada = senhaAlterada,

                    Setores =
                        usuarioDepois.GrupoSetores?
                            .Select(x => x.Setor?.NomeSetor)
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToList(),

                    Niveis =
                        usuarioDepois.GrupoNiveis?
                            .Select(x => x.Niveis?.NomeNivel)
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToList()
                };

                // =========================
                // HISTÓRICO
                // =========================

                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.Usuario,
                    id,
                    usuarioId,
                    dadosAntes,
                    dadosDepois,
                    request.Observacao
                );

                // =========================
                // COMMIT
                // =========================

                await unitOfWork.CommitAsync();

                return mapper.Map<UsuariosResponse>(usuarioDepois);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task<PageResult<UsuariosResponse>> ConsultarAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0 || pageSize > 25) pageSize = 25;

            var pageResult = await unitOfWork.UsuarioRepository.GetAllAsync(pageNumber, pageSize);

            var response = new PageResult<UsuariosResponse>
            {
                Items = mapper.Map<List<UsuariosResponse>>(pageResult.Items),
                PageNumber = pageResult.PageNumber,
                PageSize = pageResult.PageSize,
                TotalCount = pageResult.TotalCount
            };
            return response;
        }
        public async Task<UsuariosResponse?> ObterPorIdAsync(Guid id)
        {
            var usuario = await unitOfWork.UsuarioRepository.ObterCompletoPorIdAsync(id);

            if (usuario == null)
                return null;

            return mapper.Map<UsuariosResponse>(usuario);
        }
        public async Task<UsuariosResponse> ExcluirAsync(Guid id)
        {
            var usuario = await unitOfWork.UsuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado, tente outro");
            await unitOfWork.UsuarioRepository.DeleteAsync(usuario);
            return mapper.Map<UsuariosResponse>(usuario);
        }
        public async Task<AutenticarUsuarioResponse> AutenticarUsuarioAsync(AutenticarUsuarioRequest request, string ip,string userAgent)
        {
            if (request == null)
                throw new ApplicationException("Requisição inválida.");

            // Delay aleatório para reduzir brute force
            var rnd = new Random();
            await Task.Delay(rnd.Next(500, 1001));

            await unitOfWork.BeginTransactionAsync();

            try
            {
                // 1️⃣ Buscar usuário
                var usuario = await unitOfWork.UsuarioRepository
                    .GetUsuarioByLoginAsync(request.Login?.Trim());

                // 2️⃣ Usuário não encontrado
                if (usuario == null)
                {
                    await unitOfWork.FailedLoginAttemptRepository.AddAsync(new FailedLoginAttempt
                    {
                        Id = Guid.NewGuid(),
                        Login = request.Login ?? string.Empty,
                        IpAcesso = ip,
                        UserAgent = userAgent,
                        DataHora = DateTime.UtcNow,
                        Mensagem = "Usuário não encontrado."
                    });

                    await unitOfWork.CommitAsync();

                    throw new ApplicationException("Credenciais inválidas.");
                }

                // 3️⃣ Conta bloqueada
                if (usuario.Status == Status.Bloqueado)
                {
                    await unitOfWork.FailedLoginAttemptRepository.AddAsync(new FailedLoginAttempt
                    {
                        Id = Guid.NewGuid(),
                        IdUsuario = usuario.Id,
                        Login = usuario.Login ?? "",
                        IpAcesso = ip,
                        UserAgent = userAgent,
                        DataHora = DateTime.UtcNow,
                        Mensagem = "Tentativa de login em conta bloqueada."
                    });

                    await unitOfWork.CommitAsync();

                    throw new ApplicationException("Conta bloqueada. Contate o administrador.");
                }

                // 4️⃣ Validar senha
                var senhaCriptografada = CryptoHelper.SHA256Encrypt(request.Senha);
                var senhaValida = usuario.Senha == senhaCriptografada;

                if (!senhaValida)
                {
                    await unitOfWork.LoginHistoryRepository.AddAsync(new LoginHistory
                    {
                        Id = Guid.NewGuid(),
                        IdUsuario = usuario.Id,
                        DataHoraAcesso = DateTime.UtcNow,
                        IpAcesso = ip,
                        UserAgent = userAgent,
                        Sucesso = false,
                        Mensagem = "Falha ao autenticar: senha incorreta."
                    });

                    await unitOfWork.FailedLoginAttemptRepository.AddAsync(new FailedLoginAttempt
                    {
                        Id = Guid.NewGuid(),
                        IdUsuario = usuario.Id,
                        Login = usuario.Login ?? "",
                        IpAcesso = ip,
                        UserAgent = userAgent,
                        DataHora = DateTime.UtcNow,
                        Mensagem = "Senha incorreta."
                    });

                    await unitOfWork.CommitAsync();

                    // 5️⃣ Verificar tentativas
                    var totalFailed = await unitOfWork
                        .FailedLoginAttemptRepository
                        .CountRecentFailedAttemptsByUserAsync(usuario.Id);

                    if (totalFailed >= 3)
                    {
                        usuario.Status = Status.Bloqueado;
                        usuario.DataAtualizacao = DateTime.UtcNow;

                        await unitOfWork.UsuarioRepository.UpdateAsync(usuario);

                        await unitOfWork.LoginHistoryRepository.AddAsync(new LoginHistory
                        {
                            Id = Guid.NewGuid(),
                            IdUsuario = usuario.Id,
                            DataHoraAcesso = DateTime.UtcNow,
                            IpAcesso = ip,
                            UserAgent = userAgent,
                            Sucesso = false,
                            Mensagem = "Usuário bloqueado após múltiplas tentativas inválidas."
                        });

                        await unitOfWork.CommitAsync();
                    }

                    throw new ApplicationException("Credenciais inválidas.");
                }

                // 6️⃣ Login sucesso

                var niveis = (usuario.GrupoNiveis ?? Enumerable.Empty<GrupoNiveis>())
                    .Select(gn => new NivelResponse(
                        gn.Niveis?.Id ?? Guid.Empty,
                        gn.Niveis?.NomeNivel ?? string.Empty))
                    .ToList();

                var setores = (usuario.GrupoSetores ?? Enumerable.Empty<GrupoSetores>())
                    .Select(gs => new SetorResponse(
                        gs.Setor?.Id ?? Guid.Empty,
                        gs.Setor?.NomeSetor ?? string.Empty))
                    .ToList();

                var response = new AutenticarUsuarioResponse(
                    usuario.Id,
                    usuario.Login ?? "",
                    niveis,
                    setores,
                    DateTime.UtcNow,
                    _jwtTokenService.GenerateExpirationDate(),
                    _jwtTokenService.GenerateToken(usuario),
                    usuario.NomeUsuario ?? "",
                    usuario.Fotos?.FotoNome ?? "",
                    ip
                );

                // 7️⃣ Registrar login sucesso
                await unitOfWork.LoginHistoryRepository.AddAsync(new LoginHistory
                {
                    Id = Guid.NewGuid(),
                    IdUsuario = usuario.Id,
                    DataHoraAcesso = DateTime.UtcNow,
                    IpAcesso = ip,
                    UserAgent = userAgent,
                    Sucesso = true,
                    Mensagem = "Login efetuado com sucesso."
                });

                // 8️⃣ Limpar tentativas falhas
                await unitOfWork.FailedLoginAttemptRepository
                    .ClearFailedAttemptsForUserAsync(usuario.Id);

                await unitOfWork.CommitAsync();

                return response;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task<UsuariosResponse> ConsultarUsuariosPerfil(Guid id)
        {
            var u = await unitOfWork.UsuarioRepository
                .GetUsuariosComRelacionamentosPerfilAsync(id);

            if (u == null)
                throw new ApplicationException("Usuário não encontrado.");

            var foto = u.Fotos != null
                ? new FotoResponse
                {
                    IdFoto = u.Fotos.Id,
                    IdUsuario = u.Fotos.Id,
                    FotoNome = u.Fotos.FotoNome,
                    FileUrl = u.Fotos.FileUrl
                }
                : null;

            var setores = u.GrupoSetores
                .Select(gs => new GrupoSetorResponse(
                    gs.Setor.Id,
                    gs.Setor.NomeSetor
                ))
                .ToList();

            var niveis = u.GrupoNiveis
                .Select(gn => new GrupoNivelResponse(
                    gn.Niveis.Id,
                    gn.Niveis.NomeNivel
                ))
                .ToList();

            return new UsuariosResponse(
                u.Id,
                u.NomeUsuario,
                u.Login,
                u.DataCadastro!.Value,
                u.Status,
                u.ValorEmail != null ? u.ValorEmail.EnderecoEmail : string.Empty,
                foto,
                setores,
                niveis
            );
        }

        public void Dispose()
        {
            unitOfWork.Dispose();
        }
        public async Task<List<UsuariosResponse>> ConsultarAsync()
        {
            var acao = await unitOfWork.UsuarioRepository.GetAllAsync();

            var response = mapper.Map<List<UsuariosResponse>>(acao);

            return response;
        }
        public async Task<PageResult<UsuarioPaginacaoResponse>> ConsultarUsuariosComPaginacaoAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null)
        {
            var paged = await unitOfWork.UsuarioRepository
                .GetUsuariosComPaginadoAsync(pageNumber, pageSize, searchTerm);

            if (paged == null || !paged.Items.Any())
            {
                return new PageResult<UsuarioPaginacaoResponse>
                {
                    Items = new List<UsuarioPaginacaoResponse>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            return paged;
        }

        public async Task DesbloquearUsuario(Guid id)
        {
            if (id == Guid.Empty)
                throw new ApplicationException("Id do usuário inválido.");

            await unitOfWork.BeginTransactionAsync();

            var usuario = await unitOfWork.UsuarioRepository.GetByIdAsync(id);

            if (usuario == null)
                throw new ApplicationException("Usuário não encontrado.");

            // Remove tentativas
            await unitOfWork.FailedLoginAttemptRepository
                .RemoveUserAsync(id);

            // Atualiza status
            usuario.Status = Status.Ativo;

            await unitOfWork.UsuarioRepository
                .UpdateAsync(usuario);

            await unitOfWork.CommitAsync();
        }
        
    }
}
