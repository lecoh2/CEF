using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        #region Operações de transação
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();

        #endregion

        #region Acessos aos repositórios
        IUsuarioRepository UsuarioRepository { get; }
        IPessoaRepository PessoaRepository { get; }
        INivelRepository NivelRepository { get; }
        ISetorRepository SetorRepository { get; }
        IGrupoSetoresRepository GrupoSetoresRepository { get; } 
        IGrupoNiveisRepository GrupoNiveisRepository { get; }
        IFailedLoginAttemptRepository FailedLoginAttemptRepository { get; }
        ILoginHistoryRepository LoginHistoryRepository { get; }
        IEnderecoRepository EnderecoRepository { get; }
        IInformacoesComplementaresRepository InformacoesComplementaresRepository { get;  }
        IFotoRepository FotoRepository { get; }
        IProcessoRepository ProcessoRepository { get; }
        IGrupoClientesRepository GrupoClientesRepository { get; }
        IGrupoClientesProcessosRepository GrupoClientesProcessosRepository { get; }
        IGrupoEnvolvidosRepository GrupoEnvolvidosRepository { get; }
        IVaraRepository VaraRepository { get; }
        ITarefaRepository TarefaRepository { get; }
        IListaTarefaRepository ListaTarefaRepository { get; }
        IGrupoTarefaResponsaveisRepository GrupoTarefaResponsaveisRepository { get; }
        IAtendimentoRepository AtendimentoRepository { get; }
        ICasoRepository CasoRepository { get; }
        IGrupoCasoClienteRepository GrupoCasoClienteRepository { get; }
        IGrupoCasoEnvolvidosRepository GrupoCasoEnvolvidosRepository { get; }
        IQualificacaoRepository QualificacaoRepository { get; }
        IGrupoAtendimentoClienteRepository GrupoAtendimentoClienteRepository { get; }       
        IEtiquetaRepository EtiquetaRepository { get; }
        IGrupoTarefasEtiquetasRepository GrupoTarefasEtiquetasRepository { get; }
        IGrupoEventoResponsavelRepository GrupoEventoResponsavelRepository { get; }
        IEventoRepository EventoRepository { get; }
            
        IGrupoEnvolvidosProcessoRepository GrupoEnvolvidosProcessosRepository { get; }
        IGrupoEtiquetasProcessosRepository GrupoEtiquetasProcessosRepository { get; }
        IGrupoEtiquetasAtendimentoRepository GrupoEtiquetasAtendimentoRepository { get;  }
        IGrupoEtiquetaCasoRepository GrupoEtiquetaCasoRepository { get; }
        IAtendimentoHistoricoRepository AtendimentoHistoricoRepository { get; }
       
        IGrupoPessoasEtiquetasRepository GrupoPessoasEtiquetasRepository { get; }
        IContaBancariaRepository ContaBancariaRepository { get; }
        IAcaoRepository AcaoRepository { get; }
        IGrupoEventoEtiquetasRepository GrupoEventoEtiquetasRepository { get; }
        IComentarioRepository ComentarioRepository { get; }
        IHistoricoGeralRepository HistoricoGeralRepository { get; }
        INotificacaoRepository NotificacaoRepository { get; }
        IIntimacaoRepository IntimacaoRepository { get; }

        IPecaCabivelRepository PecaCabivelRepository { get; }

        ILoteTrabalhoRepository LoteTrabalhoRepository { get; }
        #endregion
    }
}
