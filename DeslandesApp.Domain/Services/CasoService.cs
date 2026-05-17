using AutoMapper;
using DeslandesApp.Domain.Helpers;
using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.Caso;
using DeslandesApp.Domain.Models.Dtos.Responses.Atendimento;
using DeslandesApp.Domain.Models.Dtos.Responses.Caso;
using DeslandesApp.Domain.Models.Dtos.Responses.Pessoas;
using DeslandesApp.Domain.Models.Dtos.Responses.Processo;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Utils;
using DeslandesApp.Domain.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Services
{
    public class CasoService(IUnitOfWork unitOfWork,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor,
    IHistoricoGeralService historicoGeralService, INotificacaoService notificacaoService,
          FunctionsHelper functionsHelper) : BaseService(httpContextAccessor), ICasoService
    {
        public async Task<CriarCasoResponse> AdicionarAsync(CriarCasoRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================
                // DTO -> ENTIDADE
                // =========================
                var caso = mapper.Map<Caso>(request);

                // =========================
                // 🧹 NORMALIZAÇÃO
                // =========================
                caso.Pasta = caso.Pasta?.Trim().ToUpper();
                caso.Titulo = caso.Titulo?.Trim().ToUpper();
                caso.Descricao = caso.Descricao?.Trim();
                caso.Observacao = caso.Observacao?.Trim();

                caso.DataCadastro = DateTime.Now;
                caso.UsuarioCadastroId = ObterUsuarioId();

                // =========================
                // 👤 RESPONSÁVEL
                // =========================
                caso.ResponsavelId = request.ResponsavelId;

                // =========================
                // ✅ VALIDAÇÃO FLUENT
                // =========================
                var validator = new CasoValidator();
                var result = validator.Validate(caso);

                if (!result.IsValid)
                    throw new ValidationException(result.Errors);

                // =========================
                // 🔍 VALIDA RESPONSÁVEL
                // =========================
                if (caso.ResponsavelId.HasValue)
                {
                    var usuario = await unitOfWork.UsuarioRepository
                        .GetByIdAsync(caso.ResponsavelId.Value);

                    if (usuario == null)
                        throw new InvalidOperationException("Responsável não encontrado.");
                }

                // =========================
                // 🔍 DUPLICIDADE
                // =========================
                var existente = await unitOfWork.CasoRepository
                    .GetByAsync(c => c.Pasta == caso.Pasta);

                if (existente != null)
                    throw new InvalidOperationException(
                        "Nome de pasta já utilizado por outro caso."
                    );

                // =========================
                // 💾 SALVAR CASO
                // =========================
                await unitOfWork.CasoRepository.AddAsync(caso);

                // =========================
                // 👥 CLIENTES
                // =========================
                if (request.GrupoCasoClientes != null &&
                    request.GrupoCasoClientes.Any())
                {
                    foreach (var grupos in request.GrupoCasoClientes)
                    {
                        await unitOfWork
                            .GrupoCasoClienteRepository
                            .AddAsync(new GrupoCasoCliente
                            {
                                CasoId = caso.Id,
                                PessoaId = grupos.IdPessoa
                            });
                    }
                }

                // =========================
                // 👥 ENVOLVIDOS
                // =========================
                if (request.GrupoCasoEnvolvidos != null &&
                    request.GrupoCasoEnvolvidos.Any())
                {
                    foreach (var grupos in request.GrupoCasoEnvolvidos)
                    {
                        await unitOfWork
                            .GrupoCasoEnvolvidosRepository
                            .AddAsync(new GrupoCasoEnvolvido
                            {
                                CasoId = caso.Id,
                                QualificacaoId = grupos.IdQualificacao,
                                PessoaId = grupos.IdPessoa
                            });
                    }
                }

                // =========================
                // 🏷️ ETIQUETAS
                // =========================
                if (request.GrupoEtiquetaCasos != null &&
                    request.GrupoEtiquetaCasos.Any())
                {
                    foreach (var item in request.GrupoEtiquetaCasos)
                    {
                        if (item.EtiquetaId == Guid.Empty)
                            continue;

                        var etiqueta = await unitOfWork
                            .EtiquetaRepository
                            .GetByIdAsync(item.EtiquetaId);

                        if (etiqueta == null)
                            continue;

                        await unitOfWork
                            .GrupoEtiquetaCasoRepository
                            .AddAsync(new GrupoEtiquetaCasos
                            {
                                CasoId = caso.Id,
                                EtiquetaId = item.EtiquetaId
                            });
                    }
                }

                // =========================
                // ✅ COMMIT
                // =========================
                await unitOfWork.CommitAsync();

                // =========================
                // 🔔 NOTIFICAÇÃO
                // =========================
                if (caso.ResponsavelId.HasValue)
                {
                    try
                    {
                        await notificacaoService.CriarNotificacaoAsync(
                            caso.ResponsavelId.Value,
                            "Novo caso criado",
                            caso.Titulo,
                            TipoEntidade.Caso,
                            caso.Id
                        );
                    }
                    catch
                    {
                        // não interrompe fluxo caso notificação falhe
                    }
                }

                // =========================
                // 📤 RETORNO
                // =========================
                return mapper.Map<CriarCasoResponse>(caso);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public Task<PageResult<CriarCasoResponse>> ConsultarAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }
        public async Task<PageResult<CasoPaginacaoResponse>> ConsultarCasoPaginacaoAsync(
    int pageNumber,
    int pageSize,
    string? searchTerm = null)
        {
            var paged = await unitOfWork.CasoRepository
                .GetCasoPaginacaoAsync(pageNumber, pageSize, searchTerm);

            if (paged == null || !paged.Items.Any())
            {
                return new PageResult<CasoPaginacaoResponse>
                {
                    Items = new List<CasoPaginacaoResponse>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            return paged;
        }
        public void Dispose()
        {
            unitOfWork.Dispose();
        }

        public Task<CriarCasoResponse> ExcluirAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<CriarCasoResponse> ModificarAsync(
            Guid id,
            CasoUpdateRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================
                // BUSCA CASO
                // =========================
                var caso = await unitOfWork
                    .CasoRepository
                    .GetByIdAsync(id)
                    ?? throw new ApplicationException(
                        "Caso não encontrado."
                    );

                var usuarioId = ObterUsuarioId();

                // =========================
                // SNAPSHOT ANTES
                // =========================
                var casoAntes = await unitOfWork
                    .CasoRepository
                    .ConsultarCasoComRelacionamentosAsync(id);

                if (casoAntes == null)
                {
                    throw new ApplicationException(
                        "Caso para histórico não encontrado."
                    );
                }

                var dadosAntes = new
                {
                    casoAntes.Pasta,
                    casoAntes.Titulo,
                    casoAntes.Descricao,
                    casoAntes.Observacao,

                    Acesso = casoAntes.Acesso.ToString(),

                    Responsavel = casoAntes.Responsavel != null
                        ? casoAntes.Responsavel.NomeUsuario
                        : null,

                    Clientes = casoAntes.GrupoCasoClientes?
                        .Select(x => x.Pessoa?.Nome)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList(),

                    Envolvidos = casoAntes.GrupoCasoEnvolvidos?
                        .Select(x => x.Pessoa?.Nome)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList(),

                    Etiquetas = casoAntes.GrupoEtiquetaCasos?
                        .Select(x => x.Etiqueta?.Nome)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList()
                };

                // =========================
                // NORMALIZAÇÃO / CAMPOS
                // =========================
                caso.Pasta = request.Pasta?.Trim()?.ToUpper();

                caso.Titulo = request.Titulo?.Trim()?.ToUpper();

                caso.Descricao = request.Descricao?.Trim();

                caso.Observacao = request.Observacao?.Trim();

                caso.Acesso = request.Acesso;

                caso.ResponsavelId = request.ResponsavelId;

                caso.DataAtualizacao = DateTime.Now;

                // =========================
                // ✅ VALIDA RESPONSÁVEL
                // =========================
                if (caso.ResponsavelId.HasValue)
                {
                    var usuario = await unitOfWork
                        .UsuarioRepository
                        .GetByIdAsync(caso.ResponsavelId.Value);

                    if (usuario == null)
                    {
                        throw new InvalidOperationException(
                            "Responsável não encontrado."
                        );
                    }
                }

                // =========================
                // 👥 CLIENTES (RESET)
                // =========================
                await unitOfWork
                    .GrupoCasoClienteRepository
                    .RemoverPorCasoId(id);

                if (request.GrupoCasoCliente?.Any() == true)
                {
                    foreach (var item in request.GrupoCasoCliente)
                    {
                        var pessoa = await unitOfWork
                            .PessoaRepository
                            .GetByIdAsync(item.IdPessoa);

                        if (pessoa == null)
                        {
                            throw new InvalidOperationException(
                                "Cliente não encontrado."
                            );
                        }

                        await unitOfWork
                            .GrupoCasoClienteRepository
                            .AddAsync(new GrupoCasoCliente
                            {
                                CasoId = id,
                                PessoaId = item.IdPessoa
                            });
                    }
                }

                // =========================
                // 👤 ENVOLVIDOS (RESET)
                // =========================
                await unitOfWork
                    .GrupoCasoEnvolvidosRepository
                    .RemoverPorCasoId(id);

                if (request.GrupoCasoEnvolvidos?.Any() == true)
                {
                    foreach (var item in request.GrupoCasoEnvolvidos)
                    {
                        var pessoa = await unitOfWork
                            .PessoaRepository
                            .GetByIdAsync(item.IdPessoa);

                        if (pessoa == null)
                        {
                            throw new InvalidOperationException(
                                "Envolvido não encontrado."
                            );
                        }

                        await unitOfWork
                            .GrupoCasoEnvolvidosRepository
                            .AddAsync(new GrupoCasoEnvolvido
                            {
                                CasoId = id,
                                PessoaId = item.IdPessoa,
                                QualificacaoId = item.IdQualificacao
                            });
                    }
                }

                // =========================
                // 🏷️ ETIQUETAS (RESET)
                // =========================
                await unitOfWork
                    .GrupoEtiquetaCasoRepository
                    .RemoverPorCasoId(id);

                if (request.GrupoEtiquetaCaso?.Any() == true)
                {
                    foreach (var item in request.GrupoEtiquetaCaso)
                    {
                        var etiqueta = await unitOfWork
                            .EtiquetaRepository
                            .GetByIdAsync(item.EtiquetaId);

                        if (etiqueta == null)
                        {
                            throw new InvalidOperationException(
                                "Etiqueta não encontrada."
                            );
                        }

                        await unitOfWork
                            .GrupoEtiquetaCasoRepository
                            .AddAsync(new GrupoEtiquetaCasos
                            {
                                CasoId = id,
                                EtiquetaId = item.EtiquetaId
                            });
                    }
                }

                // =========================
                // 💾 UPDATE ENTIDADE
                // =========================
                await unitOfWork
                    .CasoRepository
                    .UpdateAsync(caso);

                // =========================
                // SNAPSHOT DEPOIS
                // =========================
                var casoDepois = await unitOfWork
                    .CasoRepository
                    .ConsultarCasoComRelacionamentosAsync(id);

                if (casoDepois == null)
                {
                    throw new ApplicationException(
                        "Caso atualizado não encontrado."
                    );
                }

                var dadosDepois = new
                {
                    casoDepois.Pasta,
                    casoDepois.Titulo,
                    casoDepois.Descricao,
                    casoDepois.Observacao,

                    Acesso = casoDepois.Acesso.ToString(),

                    Responsavel = casoDepois.Responsavel != null
                        ? casoDepois.Responsavel.NomeUsuario
                        : null,

                    Clientes = casoDepois.GrupoCasoClientes?
                        .Select(x => x.Pessoa?.Nome)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList(),

                    Envolvidos = casoDepois.GrupoCasoEnvolvidos?
                        .Select(x => x.Pessoa?.Nome)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList(),

                    Etiquetas = casoDepois.GrupoEtiquetaCasos?
                        .Select(x => x.Etiqueta?.Nome)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList()
                };

                // =========================
                // 🕘 HISTÓRICO
                // =========================
                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.Caso,
                    id,
                    usuarioId,
                    dadosAntes,
                    dadosDepois,
                    request.Observacao
                );

                // =========================
                // ✅ COMMIT
                // =========================
                await unitOfWork.CommitAsync();

                // =========================
                // 🔔 NOTIFICAÇÃO
                // =========================
                if (caso.ResponsavelId.HasValue)
                {
                    await notificacaoService.CriarNotificacaoAsync(
                        caso.ResponsavelId.Value,
                        "Caso atualizado",
                        caso.Titulo,
                        TipoEntidade.Caso,
                        caso.Id
                    );
                }

                // =========================
                // RETORNO
                // =========================
                return mapper.Map<CriarCasoResponse>(casoDepois);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<ObterCasoResponse?> ObterPorIdAsync(Guid id)
        {
            var caso = await unitOfWork.CasoRepository.ObterCompletoPorIdAsync(id);

            if (caso == null)
                return null;

            return mapper.Map<ObterCasoResponse>(caso);
        }
        public async Task<List<CasoAutoComplete>> ConsultarCasoAutoCompleteAsync(string? termo = null)
        {
            return await unitOfWork.CasoRepository.ConsultarCasoAutoCompleteAsync(termo);
        }
        public async Task<List<ObterCasoResponse>> ConsultarUltimosAsync(int quantidade)
        {
            var dados = await unitOfWork.CasoRepository
                .ConsultarUltimosAsync(quantidade);

            return mapper.Map<List<ObterCasoResponse>>(dados);
        }
        public async Task<List<GraficoCasoResponse>> ConsultarGraficoCaso()
        {
            var dados = await unitOfWork.CasoRepository.GetGraficoCasoAsync();

            var meses = Enumerable.Range(1, 12);

            var resultado = new List<GraficoCasoResponse>();

            foreach (var mes in meses)
            {
                var item = dados.FirstOrDefault(d => d.Mes == mes);

                resultado.Add(new GraficoCasoResponse
                {
                    Mes = mes,
                    Quantidade = item?.Quantidade ?? 0
                });
            }

            return resultado;
        }

        public async Task<int> ContarCasoAnoAtual()
        {
            return await unitOfWork.CasoRepository.ContarCasoAnoAtual();
        }
        public async Task<int> ContarTotal()
        {
            return await unitOfWork.CasoRepository.ContarTotal();
        }

    }
}
