using DeslandesApp.Domain.Contracts.Security;
using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Domain.Mappings;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Extensions
{
    public static class DomainServiceExtension
    {
        public static IServiceCollection AddDomainService(this IServiceCollection services)
        {
            services.AddAutoMapper(map => map.AddProfile
(typeof(ProfileMap)));

            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IPessoaFisicaService, PessoaFisicaService>();
            services.AddScoped<IPessoaJuridicaService, PessoaJuridicaService>();
            services.AddScoped<ISetorService, SetorService>();
            services.AddScoped<INivelServices, NiveisService>();
            services.AddScoped<IProcessoService, ProcessoService>();
            services.AddScoped<ITarefaService, TarefaService>();
            services.AddScoped<ICasoService, CasoService>();
            services.AddScoped<IAtendimentoService, AtendiemntoService>();
            services.AddScoped<IEventoService, EventoService>();
            services.AddScoped<IGrupoClienteProcessoService, GrupoClienteProcessoService>();
            services.AddScoped<IGrupoEnvolvidosProcessoService, GrupoEnvolvidosProcessoService>();
            services.AddScoped<IGrupoEtiquetaProcessoService, GrupoEtiquetasProcessosService>();
            services.AddScoped<IGrupoTarefaResponsaveisService, GrupoTarefaResponsaveisServices>();
            services.AddScoped<IGrupoEtiquetaAtendimentoServices, GrupoEtiquetaAtendimentoService>();
            services.AddScoped<IGrupoEtiquetaCasoService, GrupoEtiquetaCasoService>();
            services.AddScoped<IGrupoCasoClienteService, GrupoCasoClientesServices>();
            services.AddScoped<IGrupoCasoEnvovidoService, GrupoCasoEnvolvidoService>();
            services.AddScoped<IGrupoEventoResponsaveisService, GrupoEventoResponsavelService>();
            services.AddScoped<IEtiquetasService, EtiquetasService>();
            services.AddScoped<IVaraService, VaraService>();
            services.AddScoped<IAcaoService, AcaoService>();
            services.AddScoped<IPessoaService, PessoaService>();
            services.AddScoped<IQualidicacaoService, QualificacaoService>();
            services.AddScoped<IKanbanService, KanbanService>();
            services.AddScoped<IDetalheAtividadeService, DetalheAtividadeService>();
            services.AddScoped<IComentarioService, ComentarioService>();
            services.AddScoped<IHistoricoGeralService, HistoricoGeralService>();
            services.AddScoped<IFotoServices, FotoService>();
            services.AddScoped<ILembreteService, LembreteService>();
            //services.AddTransient<IGrupoNiveisServices, GrupoNiveisService>();
            //services.AddTransient<IGrupoSetoresService, GrupoSetoresService>()


            return services;
        }
    }
}
