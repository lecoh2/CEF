using AutoMapper;
using DeslandesApp.Domain.Models.Dtos.Requests.Atendimento;
using DeslandesApp.Domain.Models.Dtos.Requests.Caso;
using DeslandesApp.Domain.Models.Dtos.Requests.Comentarios;
using DeslandesApp.Domain.Models.Dtos.Requests.ContaBancaria;
using DeslandesApp.Domain.Models.Dtos.Requests.EnderecoPessoa;
using DeslandesApp.Domain.Models.Dtos.Requests.Evento;
using DeslandesApp.Domain.Models.Dtos.Requests.GrupoCasoCliente;
using DeslandesApp.Domain.Models.Dtos.Requests.GrupoCasoEnvolvidos;
using DeslandesApp.Domain.Models.Dtos.Requests.GrupoClienteProceso;
using DeslandesApp.Domain.Models.Dtos.Requests.GrupoEnvolvidosProcesso;
using DeslandesApp.Domain.Models.Dtos.Requests.GrupoEtiquetaProcesso;
using DeslandesApp.Domain.Models.Dtos.Requests.GrupoEventoEtiquetas;
using DeslandesApp.Domain.Models.Dtos.Requests.GrupoNiveis;
using DeslandesApp.Domain.Models.Dtos.Requests.GrupoSetores;
using DeslandesApp.Domain.Models.Dtos.Requests.InformacoesComplementares;
using DeslandesApp.Domain.Models.Dtos.Requests.Nivel;
using DeslandesApp.Domain.Models.Dtos.Requests.Pessoas;
using DeslandesApp.Domain.Models.Dtos.Requests.Processo;
using DeslandesApp.Domain.Models.Dtos.Requests.Setor;
using DeslandesApp.Domain.Models.Dtos.Requests.Tarefa;
using DeslandesApp.Domain.Models.Dtos.Requests.Usuarios;
using DeslandesApp.Domain.Models.Dtos.Requests.Usuarios;
using DeslandesApp.Domain.Models.Dtos.Responses.Agenda;
using DeslandesApp.Domain.Models.Dtos.Responses.Atendimento;
using DeslandesApp.Domain.Models.Dtos.Responses.Caso;
using DeslandesApp.Domain.Models.Dtos.Responses.EnderecoEndereco;
using DeslandesApp.Domain.Models.Dtos.Responses.Etiquetas;
using DeslandesApp.Domain.Models.Dtos.Responses.Evento;
using DeslandesApp.Domain.Models.Dtos.Responses.Foto;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoAtendimentoCliente;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoCasoCliente;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoCasoEnvolvidos;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoClienteProcesso;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoEnvolvidosProcesso;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoEtiquetaAtendimento;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoEtiquetaCaso;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoEtiquetasProcessos;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoEventoEtiquetas;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoEventoResponsavel;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoNiveis;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoPessoasEtiquetas;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoSetores;
using DeslandesApp.Domain.Models.Dtos.Responses.GrupoTarefaResponsaveis;
using DeslandesApp.Domain.Models.Dtos.Responses.InformacoesComplementares;
using DeslandesApp.Domain.Models.Dtos.Responses.ListaTarefas;
using DeslandesApp.Domain.Models.Dtos.Responses.Nivel;
using DeslandesApp.Domain.Models.Dtos.Responses.Pessoas;
using DeslandesApp.Domain.Models.Dtos.Responses.Processo;
using DeslandesApp.Domain.Models.Dtos.Responses.Qualificacao;
using DeslandesApp.Domain.Models.Dtos.Responses.Setor;
using DeslandesApp.Domain.Models.Dtos.Responses.Tarefa;
using DeslandesApp.Domain.Models.Dtos.Responses.Usuarios;
using DeslandesApp.Domain.Models.Dtos.Responses.Usuarios.DeslandesApp.Domain.Models.Dtos.Responses.Usuarios;
using DeslandesApp.Domain.Models.Dtos.Responses.Vara;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum;
using DeslandesApp.Domain.ValueObjects;
using System.Runtime.ConstrainedExecution;
using ObterTarefaResponse = DeslandesApp.Domain.Models.Dtos.Responses.Tarefa.ObterTarefaResponse;

namespace DeslandesApp.Domain.Mappings
{
    public class ProfileMap : Profile
    {
        public ProfileMap()
        {

            #region USUARIOS

            CreateMap<UsuariosRequest, Usuario>()
              .ForMember(dest => dest.GrupoSetores, opt => opt.Ignore())
              .ForMember(dest => dest.GrupoNiveis, opt => opt.Ignore())
              .ForMember(dest => dest.ProcessosResponsaveis, opt => opt.Ignore())
              .ForMember(dest => dest.Pessoa, opt => opt.Ignore())
              .ForMember(dest => dest.Fotos, opt => opt.Ignore())
              .ForMember(dest => dest.ValorEmail, opt => opt.Ignore());
            CreateMap<UsuarioUpdateRequest, Usuario>()
                .ForMember(dest => dest.ValorEmail,
                    opt => opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.Email)
                            ? null
                            : new ValorEmail(src.Email)));

            CreateMap<Usuario, UsuariosResponse>()
       .ForCtorParam("Email",
           opt => opt.MapFrom(src =>
               src.ValorEmail != null
                   ? src.ValorEmail.EnderecoEmail
                   : null))
       .ForCtorParam("Foto",
    opt => opt.MapFrom(src =>
        src.Fotos != null
            ? new FotoResponse
            {
                IdFoto = src.Fotos.Id,
                IdUsuario = src.Fotos.Id,
                FotoNome = src.Fotos.FotoNome,
                FileUrl = src.Fotos.FileUrl
            }
            : null))
       .ForCtorParam("GrupoSetores",
           opt => opt.MapFrom(src =>
               src.GrupoSetores.Select(gs =>
                   new GrupoSetorResponse(
                       gs.Setor.Id,
                       gs.Setor.NomeSetor
                   )).ToList()))

       .ForCtorParam("GrupoNiveis",
           opt => opt.MapFrom(src =>
               src.GrupoNiveis.Select(gn =>
                   new GrupoNivelResponse(
                       gn.Niveis.Id,
                       gn.Niveis.NomeNivel
                   )).ToList()));

            #endregion


            #region SETOR

            CreateMap<SetorRequest, Setor>();

            CreateMap<Setor, SetorResponse>()
                .ForCtorParam("IdSetor", opt => opt.MapFrom(src => src.Id))
                .ForCtorParam("NomeSetor", opt => opt.MapFrom(src => src.NomeSetor));

            #endregion


            #region NIVEL

            CreateMap<NivelRequest, Niveis>();

            CreateMap<Niveis, NivelResponse>()
                .ForCtorParam("IdNivel", opt => opt.MapFrom(src => src.Id))
                .ForCtorParam("NomeNivel", opt => opt.MapFrom(src => src.NomeNivel));

            #endregion


            #region GRUPO NIVEIS

            CreateMap<GrupoNivelRequest, GrupoNiveis>();
            CreateMap<GrupoNiveis, GrupoNivelResponse>();

            #endregion


            #region GRUPO SETORES

            CreateMap<GrupoSetorRequest, GrupoSetores>();
            CreateMap<GrupoSetores, GrupoSetorResponse>();

            #endregion


            #region PESSOA JURIDICA

            CreateMap<PessoaJuridicaRequest, PessoaJuridica>()
       .ForMember(dest => dest.ValorEmail,
           opt => opt.MapFrom(src =>
               string.IsNullOrEmpty(src.Email)
                   ? null
                   : ValorEmail.Create(src.Email)))
       .ForMember(dest => dest.InformacoesComplementares, opt => opt.Ignore())
       .ForMember(dest => dest.GrupoPessoasEtiquetas, opt => opt.Ignore()); // 🔥 PADRÃO

            CreateMap<PessoaJuridicaUpdateRequest, PessoaJuridica>()
                .ForMember(dest => dest.ValorEmail,
                    opt => opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.Email)
                            ? null
                            : ValorEmail.Create(src.Email)))
                .ForMember(dest => dest.InformacoesComplementares, opt => opt.Ignore())
                .ForMember(dest => dest.Endereco, opt => opt.Ignore())
                .ForMember(dest => dest.GrupoPessoasEtiquetas, opt => opt.Ignore()); // 🔥 PADRÃO

            CreateMap<PessoaJuridica, PessoaJuridicaResponse>()
                .ForCtorParam("Email",
                    opt => opt.MapFrom(src =>
                        src.ValorEmail != null
                            ? src.ValorEmail.EnderecoEmail
                            : null));

            #endregion

            #region pessoas pf pj
            CreateMap<PessoaFisica, PessoaResumoResponse>()
    .ForMember(dest => dest.Documento, opt => opt.MapFrom(src => src.CPF))
    .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => "Fisica"));

            CreateMap<PessoaJuridica, PessoaResumoResponse>()
                .ForMember(dest => dest.Documento, opt => opt.MapFrom(src => src.CNPJ))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => "Juridica"));
            #endregion
            #region PESSOA FISICA

            CreateMap<PessoaFisicaRequest, PessoaFisica>()
                .ForMember(dest => dest.ValorEmail,
                    opt => opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.Email)
                            ? null
                            : new ValorEmail(src.Email)))
                .ForMember(dest => dest.InformacoesComplementares, opt => opt.Ignore())
                .ForMember(dest => dest.GrupoPessoasEtiquetas, opt => opt.Ignore()); // 🔥 AQUI

            CreateMap<PessoaFisicaUpdateRequest, PessoaFisica>()
                 .ForMember(dest => dest.ValorEmail,
        opt => opt.MapFrom(src =>
            string.IsNullOrEmpty(src.Email)
                ? null
                : ValorEmail.Create(src.Email))) // 🔥 FALTAVA ISSO
                .ForMember(dest => dest.InformacoesComplementares, opt => opt.Ignore())
                .ForMember(dest => dest.Endereco, opt => opt.Ignore())
                .ForMember(dest => dest.GrupoPessoasEtiquetas, opt => opt.Ignore()); // 🔥 BOA PRÁTICA

            CreateMap<PessoaFisica, PessoaFisicaResponse>()
                .ForCtorParam("Email",
                    opt => opt.MapFrom(src =>
                        src.ValorEmail != null
                            ? src.ValorEmail.EnderecoEmail
                            : null));

            #endregion


            #region INFORMACOES COMPLEMENTARES

            CreateMap<InformacoesComplementaresRequest,
                InformacoesComplementaresPessoaFisica>();

            CreateMap<InformacoesComplementaresPessoaFisica,
                InformacoesComplementaresResponse>();

            CreateMap<InformacoesComplementaresJuridicaRequest,
                InformacoesComplementaresPessoaJuridica>();

            CreateMap<InformacoesComplementaresPessoaJuridica,
                InformacoesComplementaresResponse>();

            #endregion


            #region ENDERECO

            CreateMap<EnderecoRequest, Endereco>();
            CreateMap<Endereco, EnderecoResponse>();

            #endregion

            #region Processo

            // =========================
            // CREATE (REQUEST -> ENTITY)
            // =========================
            CreateMap<ProcessoRequest, Processo>()
                .ForMember(d => d.Vara, o => o.Ignore())
                .ForMember(d => d.UsuarioResponsavel, o => o.Ignore())
                .ForMember(d => d.Acao, o => o.Ignore())
                .ForMember(d => d.GrupoClienteProcesso, o => o.Ignore())
                .ForMember(d => d.GrupoEnvolvidosProcesso, o => o.Ignore())
                .ForMember(d => d.GrupoEtiquetasProcessos, o => o.Ignore());


            // =========================
            // UPDATE
            // =========================
            CreateMap<ProcessoUpdateRequest, Processo>()
                .ForMember(d => d.GrupoClienteProcesso, o => o.Ignore())
                .ForMember(d => d.GrupoEnvolvidosProcesso, o => o.Ignore())
                .ForMember(d => d.GrupoEtiquetasProcessos, o => o.Ignore());


            // =========================
            // ENTITY -> RESPONSE SIMPLES
            // =========================
            CreateMap<Processo, ProcessoResponse>()
                .ForMember(d => d.AcaoId, o => o.MapFrom(s => s.AcaoId))
                .ForMember(d => d.VaraId, o => o.MapFrom(s => s.VaraId))
                .ForMember(d => d.UsuarioResponsavelId, o => o.MapFrom(s => s.UsuarioResponsavelId))

                .ForMember(d => d.Pasta, o => o.MapFrom(s => s.Pasta))
                .ForMember(d => d.Titulo, o => o.MapFrom(s => s.Titulo))
                .ForMember(d => d.NumeroProcesso, o => o.MapFrom(s => s.NumeroProcesso))
                .ForMember(d => d.LinkTribunal, o => o.MapFrom(s => s.LinkTribunal))
                .ForMember(d => d.Objeto, o => o.MapFrom(s => s.Objeto))
                .ForMember(d => d.ValorCausa, o => o.MapFrom(s => s.ValorCausa))
                .ForMember(d => d.Distribuido, o => o.MapFrom(s => s.Distribuido))
                .ForMember(d => d.ValorCondenacao, o => o.MapFrom(s => s.ValorCondenacao))
                .ForMember(d => d.Observacao, o => o.MapFrom(s => s.Observacao))
                .ForMember(d => d.Instancia, o => o.MapFrom(s => s.Instancia))
                .ForMember(d => d.Acesso, o => o.MapFrom(s => s.Acesso));


            // =========================
            // ENTITY -> RESPONSE COMPLETO
            // =========================
            CreateMap<Processo, ObterProcessoResponse>()
                //.IncludeBase<Processo, ProcessoResponse>()

                .ForMember(d => d.GrupoClienteProcesso,
                    o => o.MapFrom(s => s.GrupoClienteProcesso))

                .ForMember(d => d.GrupoEnvolvidosProcesso,
                    o => o.MapFrom(s => s.GrupoEnvolvidosProcesso))

                .ForMember(d => d.GrupoEtiquetasProcesso,
                    o => o.MapFrom(s => s.GrupoEtiquetasProcessos));


            // =========================
            // CLIENTES (ENTITY -> RESPONSE)
            // =========================
            CreateMap<GrupoClienteProcesso, GrupoClienteProcessoResponse>()
                .ForMember(d => d.IdPessoa, o => o.MapFrom(s => s.PessoaId))
                .ForMember(d => d.IdProcesso, o => o.MapFrom(s => s.ProcessoId))
                .ForMember(d => d.Nome,
                    o => o.MapFrom(s => s.Pessoa != null ? s.Pessoa.Nome : null))
                 .ForMember(d => d.QualificacaoId, o => o.MapFrom(s => s.QualificacaoId))
                .ForMember(d => d.NomeQualificacao,
                    o => o.MapFrom(s =>
                        s.QualificacaoCliente != null ? s.QualificacaoCliente.NomeQualificacao : null));


            // =========================
            // ENVOLVIDOS (ENTITY -> RESPONSE)
            // =========================
            CreateMap<GrupoEnvolvidosProcesso, GrupoEnvolvidosProcessoResponse>()
                .ForMember(d => d.IdPessoa, o => o.MapFrom(s => s.PessoaId))
                .ForMember(d => d.IdProcesso, o => o.MapFrom(s => s.ProcessoId))
                .ForMember(d => d.Nome,
                    o => o.MapFrom(s => s.Pessoa != null ? s.Pessoa.Nome : null))
                .ForMember(d => d.QualificacaoId, o => o.MapFrom(s => s.QualificacaoId))
                .ForMember(d => d.NomeQualificacao,
                    o => o.MapFrom(s =>
                        s.Qualificacao != null ? s.Qualificacao.NomeQualificacao : null));


            // =========================
            // ETIQUETAS (ENTITY -> RESPONSE)
            // =========================
            CreateMap<GrupoEtiquetasProcessos, GrupoEtiquetasProcessosResponse>()
                .ForMember(d => d.IdEtiqueta, o => o.MapFrom(s => s.EtiquetaId))
                .ForMember(d => d.IdProcesso, o => o.MapFrom(s => s.ProcessoId))
                .ForMember(d => d.Nome,
                    o => o.MapFrom(s => s.Etiqueta != null ? s.Etiqueta.Nome : null))
                .ForMember(d => d.Cor,
                    o => o.MapFrom(s => s.Etiqueta != null ? s.Etiqueta.Cor : null));


            // =========================
            // REQUEST -> ENTITY (N:N)
            // =========================
            CreateMap<GrupoClienteProcessoRequest, GrupoClienteProcesso>()
                .ForMember(d => d.PessoaId, o => o.MapFrom(s => s.IdPessoa))
                .ForMember(d => d.QualificacaoId, o => o.MapFrom(s => s.IdQualificacao))
                .ForMember(d => d.ProcessoId, o => o.Ignore());


            CreateMap<GrupoEnvolvidosProcessoRequest, GrupoEnvolvidosProcesso>()
                .ForMember(d => d.PessoaId, o => o.MapFrom(s => s.IdPessoa))
                .ForMember(d => d.QualificacaoId, o => o.MapFrom(s => s.IdQualificacao))
                .ForMember(d => d.ProcessoId, o => o.Ignore());


            CreateMap<GrupoEtiquetaProcessoRequest, GrupoEtiquetasProcessos>()
                .ForMember(d => d.EtiquetaId, o => o.MapFrom(s => s.EtiquetaId))
                .ForMember(d => d.ProcessoId, o => o.Ignore());
            CreateMap<Processo, ObterProcessoResponse>()
    .ForMember(d => d.AcaoId, o => o.MapFrom(s => s.AcaoId))
    .ForMember(d => d.VaraId, o => o.MapFrom(s => s.VaraId))
    .ForMember(d => d.UsuarioResponsavelId, o => o.MapFrom(s => s.UsuarioResponsavelId))

    // 🔥 DADOS DA VARA
    .ForMember(d => d.NomeVara,
        o => o.MapFrom(s => s.Vara != null ? s.Vara.NomeVara : null))

    .ForMember(d => d.NumeroVara,
        o => o.MapFrom(s => s.Vara != null ? s.Vara.Numero : (int?)null))

    // 🔥 DADOS DO FORO (VIA VARA)
    .ForMember(d => d.ForoId,
        o => o.MapFrom(s => s.Vara != null ? s.Vara.ForoId : (Guid?)null))

    .ForMember(d => d.NomeForo,
        o => o.MapFrom(s =>
            s.Vara != null && s.Vara.Foro != null
                ? s.Vara.Foro.NomeForo
                : null))
     .ForMember(d => d.Juizo,
        o => o.MapFrom(s =>
            s.Vara.NomeVara + " - " + s.Vara.Foro.NomeForo
        ))
    // RELAÇÕES
    .ForMember(d => d.GrupoClienteProcesso,
        o => o.MapFrom(s => s.GrupoClienteProcesso))

    .ForMember(d => d.GrupoEnvolvidosProcesso,
        o => o.MapFrom(s => s.GrupoEnvolvidosProcesso))

    .ForMember(d => d.GrupoEtiquetasProcesso,
        o => o.MapFrom(s => s.GrupoEtiquetasProcessos));
            #endregion




            #region TAREFAS

            // =========================
            // CREATE
            // =========================
            CreateMap<CriarTarefaRequest, Tarefa>()
                .ForMember(dest => dest.ListasTarefa, opt => opt.Ignore())
                .ForMember(dest => dest.GrupoTarefasEtiquetas, opt => opt.Ignore())
                .ForMember(dest => dest.GrupoTarefaResponsaveis, opt => opt.Ignore());

            CreateMap<Tarefa, CriarTarefaResponse>();

            // =========================
            // LISTA TAREFA
            // =========================
            CreateMap<CriarListaTarefaRequest, ListaTarefa>();

            CreateMap<ListaTarefa, ListaTarefasResponse>()
                .ForMember(dest => dest.Descricao,
                    opt => opt.MapFrom(src => src.Descricao));


            // =========================
            // RESPONSE PRINCIPAL (OBTER TAREFA)
            // =========================
            CreateMap<Tarefa, ObterTarefaResponse>()

                // 🔹 BÁSICOS
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))

                .ForMember(dest => dest.Descricao,
                    opt => opt.MapFrom(src => src.Descricao))

                .ForMember(dest => dest.DataTarefa,
                    opt => opt.MapFrom(src => src.DataTarefa))

                .ForMember(dest => dest.Prioridade,
                    opt => opt.MapFrom(src => src.Prioridade))

                .ForMember(dest => dest.StatusGeralKanban,
                    opt => opt.MapFrom(src => src.StatusGeralKanban))
                .ForMember(dest => dest.TipoVinculo, opt => opt.MapFrom(src =>
    src.Processo != null ? TipoVinculo.Processo :
    src.Atendimento != null ? TipoVinculo.Atendimento :
    src.Caso != null ? TipoVinculo.Caso :
    (TipoVinculo?)null
))

.ForMember(dest => dest.ProcessoPasta, opt => opt.MapFrom(src =>
    src.Processo != null ? $"{src.Processo.Pasta} - {src.Processo.NumeroProcesso}" : null
))

.ForMember(dest => dest.AtendimentoAssunto, opt => opt.MapFrom(src =>
    src.Atendimento != null ? $"{src.Atendimento.Assunto} - {src.Atendimento.Registro}" : null
))

.ForMember(dest => dest.CasoPasta, opt => opt.MapFrom(src =>
    src.Caso != null ? $"{src.Caso.Pasta} - {src.Caso.Titulo}" : null
))


                // =========================
                // 🔗 VÍNCULOS (CORRIGIDO)
                // =========================
                .ForMember(dest => dest.TipoVinculo,
                    opt => opt.MapFrom(src => src.TipoVinculoId != null ? (int?)src.TipoVinculoId : null))

                .ForMember(dest => dest.ProcessoId,
                    opt => opt.MapFrom(src => src.ProcessoId))

                .ForMember(dest => dest.CasoId,
                    opt => opt.MapFrom(src => src.CasoId))

                .ForMember(dest => dest.AtendimentoId,
                    opt => opt.MapFrom(src => src.AtendimentoId))


                // =========================
                // 🔥 DESCRIÇÕES DOS VÍNCULOS (SEGURAS)
                // =========================
                .ForMember(dest => dest.ProcessoPasta,
                    opt => opt.MapFrom(src => src.Processo != null ? src.Processo.Pasta : null))

                .ForMember(dest => dest.CasoPasta,
                    opt => opt.MapFrom(src => src.Caso != null ? src.Caso.Pasta : null))

                .ForMember(dest => dest.AtendimentoAssunto,
                    opt => opt.MapFrom(src => src.Atendimento != null ? src.Atendimento.Assunto : null))


                // =========================
                // 📋 LISTA
                // =========================
                .ForMember(dest => dest.ListasTarefa,
                    opt => opt.MapFrom(src => src.ListasTarefa))


                // =========================
                // 👥 RESPONSÁVEIS
                // =========================
                .ForMember(dest => dest.GrupoTarefaResponsaveis,
                    opt => opt.MapFrom(src => src.GrupoTarefaResponsaveis))


                // =========================
                // 🏷️ ETIQUETAS
                // =========================
                .ForMember(dest => dest.GrupoTarefasEtiquetas,
                    opt => opt.MapFrom(src => src.GrupoTarefasEtiquetas));


            // =========================
            // 👤 RESPONSÁVEIS (DETALHE)
            // =========================
            CreateMap<GrupoTarefaResponsaveis, TarefaResponsavelResponse>()
                .ForMember(d => d.UsuarioId,
                    o => o.MapFrom(s => s.UsuarioId))
                .ForMember(d => d.Nome,
                    o => o.MapFrom(s => s.Usuario != null ? s.Usuario.NomeUsuario : null));


            // =========================
            // 👤 RESPONSÁVEIS (SELECT ANGULAR)
            // =========================
            CreateMap<GrupoTarefaResponsaveis, ConsultarUsuarioResponse>()
                .ForMember(d => d.Id,
                    o => o.MapFrom(s => s.UsuarioId)) // 🔥 CORRIGIDO (era Usuario.Id)
                .ForMember(d => d.NomeUsuario,
                    o => o.MapFrom(s => s.Usuario != null ? s.Usuario.NomeUsuario : null));


            // =========================
            // 🏷️ ETIQUETAS
            // =========================
            CreateMap<GrupoTarefasEtiquetas, EtiquetaResponse>()
                .ForMember(d => d.Id,
                    o => o.MapFrom(s => s.EtiquetaId))
                .ForMember(d => d.Nome,
                    o => o.MapFrom(s => s.Etiqueta != null ? s.Etiqueta.Nome : null))
                .ForMember(d => d.Cor,
                    o => o.MapFrom(s => s.Etiqueta != null ? s.Etiqueta.Cor : null));

            #endregion
            #region Caso

            // =========================
            // CREATE (Cadastro)
            // =========================
            CreateMap<Caso, CriarCasoResponse>();
            CreateMap<CriarCasoRequest, Caso>()
                .ForMember(dest => dest.GrupoCasoClientes, opt => opt.Ignore())
                .ForMember(dest => dest.GrupoCasoEnvolvidos, opt => opt.Ignore())
                .ForMember(dest => dest.GrupoEtiquetaCasos, opt => opt.Ignore());

            // =========================
            // UPDATE (Edição)
            // =========================
            CreateMap<CasoUpdateRequest, Caso>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCadastro, opt => opt.Ignore())
                .ForMember(dest => dest.GrupoCasoClientes, opt => opt.Ignore())
                .ForMember(dest => dest.GrupoCasoEnvolvidos, opt => opt.Ignore())
                .ForMember(dest => dest.GrupoEtiquetaCasos, opt => opt.Ignore());

            // =========================
            // RESPONSE (CASO PRINCIPAL)
            // =========================
            CreateMap<Caso, ObterCasoResponse>()
                .ForMember(dest => dest.GrupoCasoClientes,
                    opt => opt.MapFrom(src => src.GrupoCasoClientes))

                .ForMember(dest => dest.GrupoCasoEnvolvidos,
                    opt => opt.MapFrom(src => src.GrupoCasoEnvolvidos))

                .ForMember(dest => dest.GrupoEtiquetaCaso,
                    opt => opt.MapFrom(src => src.GrupoEtiquetaCasos))

                .ForMember(dest => dest.ResponsavelNome,
                    opt => opt.MapFrom(src => src.Responsavel != null ? src.Responsavel.NomeUsuario : null))

                .ForMember(dest => dest.UsuarioCadastroNome,
                    opt => opt.MapFrom(src => src.UsuarioCadastro != null ? src.UsuarioCadastro.NomeUsuario : null));

            // =========================
            // CLIENTES DO CASO
            // =========================
            CreateMap<GrupoCasoCliente, GrupoCasoClienteResponse>()
                .ForMember(dest => dest.PessoaId,
                    opt => opt.MapFrom(src => src.PessoaId))

                .ForMember(dest => dest.CasoId,
                    opt => opt.MapFrom(src => src.CasoId))

                .ForMember(dest => dest.Nome,
                    opt => opt.MapFrom(src => src.Pessoa != null ? src.Pessoa.Nome : null));

            // =========================
            // ENVOLVIDOS DO CASO
            // =========================
            CreateMap<GrupoCasoEnvolvido, GrupoCasoEnvolvidosResponse>()
                .ForMember(dest => dest.PessoaId,
                    opt => opt.MapFrom(src => src.PessoaId))

                .ForMember(dest => dest.Nome,
                    opt => opt.MapFrom(src => src.Pessoa != null ? src.Pessoa.Nome : null))

                .ForMember(dest => dest.QualificacaoId,
                    opt => opt.MapFrom(src => src.QualificacaoId))

                .ForMember(dest => dest.NomeQualificacao,
                    opt => opt.MapFrom(src =>
                        src.Qualificacao != null ? src.Qualificacao.NomeQualificacao : null));

            // =========================
            // ETIQUETAS DO CASO
            // =========================
            CreateMap<GrupoEtiquetaCasos, GrupoEtiquetaCasoResponse>()
                .ForMember(dest => dest.EtiquetaId,
                    opt => opt.MapFrom(src => src.EtiquetaId))

                .ForMember(dest => dest.Nome,
                    opt => opt.MapFrom(src =>
                        src.Etiqueta != null ? src.Etiqueta.Nome : null))

                .ForMember(dest => dest.Cor,
                    opt => opt.MapFrom(src =>
                        src.Etiqueta != null ? src.Etiqueta.Cor : null));

            #endregion
            #region GrupoCasoCliente

            CreateMap<GrupoCasoClienteRequest, GrupoCasoCliente>();

            #endregion

            #region GrupoCasoEnvolvido

            CreateMap<GrupoCasoEnvolvidosRequest, GrupoCasoEnvolvido>();

            #endregion

            #region ATENDIMENTO

            // =============================
            // 🔹 REQUEST → ENTITY (CRIAR)
            // =============================
            CreateMap<CriarAtendimentoClienteRequest, Atendimento>()
          .ForMember(x => x.GrupoEtiquetasAtendimentos, opt => opt.Ignore())
          .ForMember(x => x.GrupoClientes, opt => opt.Ignore())
          .ForMember(x => x.Processo, opt => opt.Ignore())
          .ForMember(x => x.Caso, opt => opt.Ignore())
          .ForMember(x => x.AtendimentoPai, opt => opt.Ignore())
          .ForMember(x => x.Responsavel, opt => opt.Ignore());

            // =============================
            // 🔹 UPDATE → ENTITY
            // =============================
            CreateMap<AtendimentoClienteUpdateRequest, Atendimento>()
    .ForMember(x => x.GrupoEtiquetasAtendimentos, opt => opt.Ignore())
    .ForMember(x => x.GrupoClientes, opt => opt.Ignore())
    .ForMember(x => x.Processo, opt => opt.Ignore())
    .ForMember(x => x.Caso, opt => opt.Ignore())
    .ForMember(x => x.AtendimentoPai, opt => opt.Ignore())
    .ForMember(x => x.Responsavel, opt => opt.Ignore())
    .ForMember(x => x.DataCadastro, opt => opt.Ignore())
    .ForMember(x => x.ProcessoId, opt => opt.Ignore())
    .ForMember(x => x.CasoId, opt => opt.Ignore())
    .ForMember(x => x.AtendimentoPaiId, opt => opt.Ignore())
    .ForMember(x => x.ResponsavelId, opt => opt.Ignore());

            // =============================
            // 🔥 ENTITY → RESPONSE
            // =============================
            CreateMap<Atendimento, CriarAtendimentoClienteResponse>()
     .ForMember(d => d.GrupoAtendimentoCliente,
         o => o.MapFrom(s => s.GrupoClientes))

     .ForMember(d => d.GrupoAtendimentoEtiqueta,
         o => o.MapFrom(s => s.GrupoEtiquetasAtendimentos));
            CreateMap<GrupoAtendimentoCliente, GrupoAtendimentoClienteResponse>()
    .ForMember(d => d.Nome,
        o => o.MapFrom(s => s.Pessoa.Nome));
            CreateMap<GrupoEtiquetasAtendimentos, GrupoEtiquetaAtendimentoResponse>()
    .ForMember(d => d.EtiquetaId,
        o => o.MapFrom(s => s.EtiquetaId))
    .ForMember(d => d.Nome,
        o => o.MapFrom(s => s.Etiqueta.Nome))
    .ForMember(d => d.Cor,
        o => o.MapFrom(s => s.Etiqueta.Cor));
            CreateMap<Atendimento, ObterAtendimentoResponse>()
      // vínculos
      .ForMember(d => d.ProcessoPasta,
          opt => opt.MapFrom(s => s.Processo != null ? s.Processo.Pasta : null))

      .ForMember(d => d.CasoPasta,
          opt => opt.MapFrom(s => s.Caso != null ? s.Caso.Pasta : null))

      .ForMember(d => d.AtendimentoAssunto,
          opt => opt.MapFrom(s => s.AtendimentoPai != null ? s.AtendimentoPai.Assunto : null))

      // clientes
      .ForMember(d => d.GrupoAtendimentoCliente,
          opt => opt.MapFrom(s => s.GrupoClientes))

      // etiquetas
      .ForMember(d => d.GrupoAtendimentoEtiqueta,
          opt => opt.MapFrom(s => s.GrupoEtiquetasAtendimentos));
            CreateMap<GrupoAtendimentoCliente, GrupoAtendimentoClienteResponse>()
    .ForMember(d => d.PessoaId,
        opt => opt.MapFrom(s => s.PessoaId))
    .ForMember(d => d.Nome,
        opt => opt.MapFrom(s => s.Pessoa.Nome));

            CreateMap<GrupoEtiquetasAtendimentos, GrupoEtiquetaAtendimentoResponse>()
    .ForMember(d => d.EtiquetaId,
        opt => opt.MapFrom(s => s.EtiquetaId))
    .ForMember(d => d.Nome,
        opt => opt.MapFrom(s => s.Etiqueta != null ? s.Etiqueta.Nome : null))
    .ForMember(d => d.Cor,
        opt => opt.MapFrom(s => s.Etiqueta != null ? s.Etiqueta.Cor : null));

            #endregion


            #region Evento

            CreateMap<CriarEventoRequest, Evento>()
                .ForMember(dest => dest.GrupoEventoEtiquetas, opt => opt.Ignore())
                .ForMember(dest => dest.GrupoEventoResponsaveis, opt => opt.Ignore());



            CreateMap<UpdateEventoRequest, Evento>()
                .ForMember(dest => dest.GrupoEventoEtiquetas, opt => opt.Ignore())
                .ForMember(dest => dest.GrupoEventoResponsaveis, opt => opt.Ignore());
            CreateMap<Evento, CriarEventoResponse>()
          .ForCtorParam("Titulo",
              opt => opt.MapFrom(src => src.Titulo))
          .ForCtorParam("StatusGeralKanban",
              opt => opt.MapFrom(src => src.StatusGeralKanban
                  .ToString()
                  .Replace("_", " ")));
            CreateMap<Evento, ObterEventoResponse>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Titulo, opt => opt.MapFrom(src => src.Titulo))
               .ForMember(dest => dest.DataInicial, opt => opt.MapFrom(src => src.DataInicial))
               .ForMember(dest => dest.HoraInicial, opt => opt.MapFrom(src => src.HoraInicial))
               .ForMember(dest => dest.DataFinal, opt => opt.MapFrom(src => src.DataFinal))
               .ForMember(dest => dest.HoraFinal, opt => opt.MapFrom(src => src.HoraFinal))
               .ForMember(dest => dest.DiaInteiro, opt => opt.MapFrom(src => src.DiaInteiro))
               .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => src.Endereco))
               .ForMember(dest => dest.Modalidade, opt => opt.MapFrom(src => src.Modalidade))
               .ForMember(dest => dest.Observacao, opt => opt.MapFrom(src => src.Observacao))
               .ForMember(dest => dest.StatusGeralKanban, opt => opt.MapFrom(src => src.StatusGeralKanban))
               .ForMember(dest => dest.ProcessoId, opt => opt.MapFrom(src => src.ProcessoId))
               .ForMember(dest => dest.CasoId, opt => opt.MapFrom(src => src.CasoId))
               .ForMember(dest => dest.AtendimentoId, opt => opt.MapFrom(src => src.AtendimentoId))
               .ForMember(dest => dest.TipoVinculo, opt => opt.MapFrom(src => src.TipoVinculoId))

               // 🔥 AQUI ESTÁ A CORREÇÃO
               .ForMember(dest => dest.ProcessoPasta,
                   opt => opt.MapFrom(src =>
                       src.Processo != null ? src.Processo.Pasta : null))

               .ForMember(dest => dest.CasoPasta,
                   opt => opt.MapFrom(src =>
                       src.Caso != null ? src.Caso.Pasta : null))

               .ForMember(dest => dest.AtendimentoAssunto,
                   opt => opt.MapFrom(src =>
                       src.Atendimento != null ? src.Atendimento.Assunto : null));

            CreateMap<GrupoEventoResponsavel, GrupoEventoResponsavelResponse>()
    .ForMember(dest => dest.UsuarioId,
        opt => opt.MapFrom(src => src.UsuarioId))
    .ForMember(dest => dest.NomeUsuario,
        opt => opt.MapFrom(src => src.Usuario.NomeUsuario));

            CreateMap<GrupoEventoEtiquetas, GrupoEventoEtiquetasResponse>()
                .ForMember(dest => dest.EtiquetaId,
                    opt => opt.MapFrom(src => src.EtiquetaId))
                .ForMember(dest => dest.Nome,
                    opt => opt.MapFrom(src => src.Etiqueta.Nome))
                .ForMember(dest => dest.Cor,
                    opt => opt.MapFrom(src => src.Etiqueta.Cor));
            #endregion
            #region GrupoEvento

            CreateMap<GrupoEventoResponsavel, GrupoEventoResponsavel>();

            #endregion
            #region Agenda
            CreateMap<Tarefa, AgendaItemResponse>()
    .ForMember(dest => dest.Titulo, opt => opt.MapFrom(src => src.Descricao))
    .ForMember(dest => dest.Data, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.DataTarefa ?? src.DataCadastro)))
    .ForMember(dest => dest.Tipo, opt => opt.MapFrom(_ => "Tarefa"))
    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (StatusGeralKanban)src.StatusGeralKanban))
    .ForMember(dest => dest.HoraInicio, opt => opt.Ignore())
    .ForMember(dest => dest.HoraFim, opt => opt.Ignore());

            CreateMap<Evento, AgendaItemResponse>()
    .ForMember(dest => dest.Titulo, opt => opt.MapFrom(src => src.Titulo))
    .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.DataInicial))
    .ForMember(dest => dest.HoraInicio, opt => opt.MapFrom(src => src.HoraInicial))
    .ForMember(dest => dest.HoraFim, opt => opt.MapFrom(src => src.HoraFinal))
    .ForMember(dest => dest.Tipo, opt => opt.MapFrom(_ => "Evento"))
    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (StatusGeralKanban)src.StatusGeralKanban));
            #endregion
            #region etiquetas
            CreateMap<Etiqueta, EtiquetaResponse>();



            #endregion
            #region Conta bancaria
            CreateMap<ContaBancariaRequest, ContaBancaria>()
     .ForMember(dest => dest.TipoConta, opt => opt.MapFrom(src => src.TipoConta));
            #endregion
            #region Pesoas Etiquetas
            CreateMap<GrupoPessoasEtiquetas, GrupoPessoasEtiquetasResponse>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EtiquetaId))
    .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Etiqueta.Nome))
    .ForMember(dest => dest.Cor, opt => opt.MapFrom(src => src.Etiqueta.Cor));
            #endregion
            #region vara
            CreateMap<Vara, VaraResponse>()
         .ForMember(dest => dest.NomeForo,
             opt => opt.MapFrom(src => src.Foro.NomeForo));
            #endregion
            #region Acao
            CreateMap<Acao, AcaoResponse>()
     .ForCtorParam("IdAcao", opt => opt.MapFrom(src => src.Id))
     .ForCtorParam("NomeAcao", opt => opt.MapFrom(src => src.NomeAcao));
            #endregion
            #region Qualificacao
            CreateMap<Qualificacao, QualificacaoResponse>()
     .ForCtorParam("IdQualificacao", opt => opt.MapFrom(src => src.Id))
     .ForCtorParam("NomeQualificacao", opt => opt.MapFrom(src => src.NomeQualificacao));
            #endregion
            #region comentario


            CreateMap<CriarComentarioRequest, Comentario>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(_ => DateTime.Now))
            .ForMember(dest => dest.Texto, opt => opt.MapFrom(src => src.Texto.Trim()));


            #endregion

            #region Foto
            CreateMap<Fotos, FotoResponse>();
            #endregion
        }
    }
}