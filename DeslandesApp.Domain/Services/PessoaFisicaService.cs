using AutoMapper;
using DeslandesApp.Domain.Helpers;
using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Models.Dtos.Requests.ContaBancaria;
using DeslandesApp.Domain.Models.Dtos.Requests.InformacoesComplementares;
using DeslandesApp.Domain.Models.Dtos.Requests.Pessoas;
using DeslandesApp.Domain.Models.Dtos.Responses.Pessoas;
using DeslandesApp.Domain.Models.Dtos.Responses.Usuarios;
using DeslandesApp.Domain.Models.Dtos.Responses.Vara;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.Utils;
using DeslandesApp.Domain.Validators;
using DeslandesApp.Domain.ValueObjects;
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
    public class PessoaFisicaService(
   IUnitOfWork unitOfWork,
   IMapper mapper,
   IHttpContextAccessor httpContextAccessor,
   IHistoricoGeralService historicoGeralService
) : BaseService(httpContextAccessor), IPessoaFisicaService
    {

        public async Task<PessoaFisicaResponse> AdicionarAsync(PessoaFisicaRequest request)
        {
            await unitOfWork.BeginTransactionAsync();
            try
            {
                var cpf = FunctionsHelper.RemovePontosTracos(request.Cpf);
            var rg = FunctionsHelper.RemovePontosTracos(request.Rg);

            if (!FunctionsHelper.ValidadorCPF(cpf))
                throw new ApplicationException("CPF inválido.");

            if (await unitOfWork.PessoaRepository.CpfInUseAsync(cpf))
                throw new InvalidOperationException("CPF já cadastrado.");

            if (!string.IsNullOrWhiteSpace(rg) &&
                await unitOfWork.PessoaRepository.RgInUseAsync(rg))
                throw new InvalidOperationException("RG já cadastrado.");
            if (request.Perfil.HasValue &&
    !Enum.IsDefined(typeof(Perfil), request.Perfil.Value))

            {
                throw new ApplicationException("Perfil inválido.");
            }

            var pessoa = mapper.Map<PessoaFisica>(request);

            var validator = new PessoaFisicaValidator();
            var result = validator.Validate(pessoa);

            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            pessoa.CPF = cpf;
            pessoa.RG = rg;
            pessoa.Telefone = FunctionsHelper.RemovePontosTracosTelefone(pessoa.Telefone);
            pessoa.DataCadastro = DateTime.Now;

                //pessoa.IdSexo = request.IdSexo;

                if (!string.IsNullOrWhiteSpace(pessoa.ValorEmail?.EnderecoEmail))
                {
                    if (await unitOfWork.PessoaRepository
                        .EmailInUseAsync(pessoa.ValorEmail.EnderecoEmail))
                    {
                        throw new InvalidOperationException("Email já cadastrado.");
                    }
                }
                else
                {
                    pessoa.ValorEmail = null;
                }

                // ENDEREÇO
                if (request.Endereco != null)
            {
                pessoa.Endereco = mapper.Map<Endereco>(request.Endereco);
                pessoa.Endereco.Cep = FunctionsHelper.RemovePontosTracos(pessoa.Endereco.Cep);
                pessoa.Endereco.Complemento ??= "";
            }

            // INFORMAÇÕES COMPLEMENTARES
            if (TemAlgumValor(request.InformacoesComplementares))
            {
                pessoa.InformacoesComplementares =
                   mapper.Map<InformacoesComplementaresPessoaFisica>(request.InformacoesComplementares);
            }

            // SALVA PESSOA PRIMEIRO (IMPORTANTE!)
            await unitOfWork.PessoaRepository.AddAsync(pessoa);

            // N:N - ETIQUETAS (OPCIONAL)
            if (request.GrupoPessoasEtiquetas != null && request.GrupoPessoasEtiquetas.Any())
            {
                foreach (var item in request.GrupoPessoasEtiquetas)
                {
                    // 🔥 CORRETO: buscar na tabela de ETIQUETA
                    var etiqueta = await unitOfWork.EtiquetaRepository.GetByIdAsync(item.idEtiqueta);

                    if (etiqueta == null)
                        throw new InvalidOperationException("Etiqueta não encontrada.");

                    var grupoEtiqueta = new GrupoPessoasEtiquetas
                    {
                        EtiquetaId = etiqueta.Id, // 👈 aqui é Id da etiqueta
                        PessoaId = pessoa.Id
                    };

                    await unitOfWork.GrupoPessoasEtiquetasRepository.AddAsync(grupoEtiqueta);
                }
            }
            // CONTA BANCÁRIA (OPCIONAL)
            if (request.ContaBancaria != null && TemDadosConta(request.ContaBancaria))
            {
                var conta = mapper.Map<ContaBancaria>(request.ContaBancaria);

                conta.PessoaId = pessoa.Id; // vínculo com a pessoa

                await unitOfWork.ContaBancariaRepository.AddAsync(conta);
            }
            await unitOfWork.CommitAsync();

            return mapper.Map<PessoaFisicaResponse>(pessoa);
        }
            catch
    {
                await unitOfWork.RollbackAsync(); // 🔥 ESSENCIAL
                throw; // middleware trata
            }
        }


        public Task<PageResult<PessoaFisicaResponse>> ConsultarAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }


      
        public async Task<PageResult<PessoaFisicaPaginacaoResponse>> ConsultarPessoaFisicaPaginacaoAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var paged = await unitOfWork.PessoaRepository.PessoaFisicaComPaginacaoAsync
                (pageNumber, pageSize, searchTerm);

            if (paged == null || !paged.Items.Any())
            {
                return new PageResult<PessoaFisicaPaginacaoResponse>
                {
                    Items = new List<PessoaFisicaPaginacaoResponse>(),
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

        public Task<PessoaFisicaResponse> ExcluirAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<PessoaFisicaResponse> ModificarAsync(Guid id, PessoaFisicaUpdateRequest request)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                var pessoa = await unitOfWork.PessoaRepository.GetByIdAsync(id);

                if (pessoa == null)
                    throw new ApplicationException("Pessoa não encontrada para edição.");

                var pessoaAntes = await unitOfWork
                    .PessoaRepository
                    .ConsultarPessoasFisicasComIdRelacionamentosAsync(id);

                var infoAntes = pessoaAntes.InformacoesComplementares as InformacoesComplementaresPessoaFisica;

                var dadosAntes = new
                {
                    pessoaAntes.Nome,
                    pessoaAntes.Apelido,
                    pessoaAntes.Telefone,
                    pessoaAntes.ValorEmail,
                    pessoaAntes.Site,
                    pessoaAntes.CPF,
                    pessoaAntes.RG,

                    //Sexo = pessoaAntes.Sexo?.NomeSexo,

                    Usuario = pessoaAntes.Usuario != null ? new
                    {
                        pessoaAntes.Usuario.Id,
                        pessoaAntes.Usuario.Login,
                        pessoaAntes.Usuario.NomeUsuario
                    } : null,

                    Endereco = pessoaAntes.Endereco != null ? new
                    {
                        pessoaAntes.Endereco.Logradouro,
                        pessoaAntes.Endereco.Numero,
                        pessoaAntes.Endereco.Bairro,
                        pessoaAntes.Endereco.Localidade,
                        pessoaAntes.Endereco.Uf,
                        pessoaAntes.Endereco.Cep
                    } : null,

                    InformacoesComplementares = infoAntes != null ? new
                    {
                        pessoaAntes.InformacoesComplementares.Codigo,
                        pessoaAntes.InformacoesComplementares.Comentario,
                        infoAntes.DataNascimento,
                        infoAntes.NomeEmpresa,
                        infoAntes.Profissao,
                        infoAntes.AtividadeEconomica
                    } : null,

                    pessoaAntes.DataAtualizacao
                };

                pessoa.DataAtualizacao = DateTime.Now;

                mapper.Map(request, pessoa);
                await unitOfWork.PessoaRepository.UpdateAsync(pessoa);

                if (request.Endereco != null)
                {
                    var endereco = await unitOfWork.EnderecoRepository.GetByAsync(e => e.IdPessoa == pessoa.Id);

                    if (endereco != null)
                    {
                        mapper.Map(request.Endereco, endereco);
                        await unitOfWork.EnderecoRepository.UpdateAsync(endereco);
                    }
                }

                if (request.InformacoesComplementares != null)
                {
                    var info = await unitOfWork.InformacoesComplementaresRepository.GetByAsync(e => e.IdPessoa == pessoa.Id);

                    if (info != null)
                    {
                        mapper.Map(request.InformacoesComplementares, info);
                        await unitOfWork.InformacoesComplementaresRepository.UpdateAsync(info);
                    }
                }

                var pessoaDepois = await unitOfWork
                    .PessoaRepository
                    .ConsultarPessoasFisicasComIdRelacionamentosAsync(id);

                var dadosDepois = new
                {
                    pessoaDepois.Nome,
                    pessoaDepois.Apelido,
                    pessoaDepois.Telefone,
                    pessoaDepois.ValorEmail,
                    pessoaDepois.Site,
                    pessoaDepois.CPF,
                    pessoaDepois.RG,
                    // Sexo = pessoaDepois.Sexo?.NomeSexo,
                    pessoaDepois.DataAtualizacao
                };

                if (request.IdUsuario == null)
                    throw new ApplicationException("Id do usuário não informado.");


                var usuarioId = ObterUsuarioId();

                await historicoGeralService.RegistrarAsync(
                    TipoEntidade.Pessoa,
                    pessoa.Id,
                    usuarioId,
                    dadosAntes,
                    dadosDepois,
                    request.Observacoes
                );

                await unitOfWork.CommitAsync();

                return mapper.Map<PessoaFisicaResponse>(pessoaDepois);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public Task<PessoaFisicaResponse?> ObterPorIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        private bool TemDadosConta(ContaBancariaRequest conta)
        {
            return !string.IsNullOrWhiteSpace(conta.NomeBanco)
                || !string.IsNullOrWhiteSpace(conta.Agencia)
                || !string.IsNullOrWhiteSpace(conta.NumeroConta)
                || !string.IsNullOrWhiteSpace(conta.Pix)
               || conta.TipoConta.HasValue;
        }
        private bool TemAlgumValor(InformacoesComplementaresRequest info)
        {
            if (info == null) return false;

            return !string.IsNullOrWhiteSpace(info.DataNascimento)
                || !string.IsNullOrWhiteSpace(info.NomeEmpresa)
                || !string.IsNullOrWhiteSpace(info.Profissao)
                || !string.IsNullOrWhiteSpace(info.AtividadeEconomica)
                || !string.IsNullOrWhiteSpace(info.EstadoCivil)
                || !string.IsNullOrWhiteSpace(info.Codigo)
                || !string.IsNullOrWhiteSpace(info.NomePai)
                || !string.IsNullOrWhiteSpace(info.NomeMae)
                || !string.IsNullOrWhiteSpace(info.Naturalidade)
                || !string.IsNullOrWhiteSpace(info.Nacionalidade)
                || !string.IsNullOrWhiteSpace(info.Comentario);
        }
    }
}
