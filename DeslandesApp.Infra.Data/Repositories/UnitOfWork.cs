using DeslandesApp.Domain.Interfaces.Repositories;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Infra.Data.Contexts;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Infra.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork //(DataContext dataContext) : IUnitOfWork
    {
        //atributo para armazenar o contexto 
        private readonly DataContext dataContext;

        //construtor para injeção de dependência 
        public UnitOfWork(DataContext _dataContext)
        {
            dataContext = _dataContext;
        }

        #region Repositórios
        private IUsuarioRepository? _usuarioRepository;
        private INivelRepository? _nivelRepository;
        private ISetorRepository? _setorRepository;
        private IGrupoNiveisRepository? _grupoNiveisRepository;
        private IGrupoSetoresRepository? _grupoSetoresRepository;
        private IFailedLoginAttemptRepository? _failedLoginAttemptRepository;
        private ILoginHistoryRepository? _loginHistoryRepository;
        private IPessoaRepository? _pessoaRepository;
        private IEnderecoRepository? _enderecoRepository;
        private IInformacoesComplementaresRepository? _informacoesComplementaresRepository;
       
        private IProcessoRepository? _processoRepository;
        private IGrupoClientesRepository? _grupoClientesRepository;
        private IGrupoEnvolvidosRepository? _grupoEnvolvidosRepository;
        private IVaraRepository? _varaRepository;
        private ITarefaRepository? _tarefaRepository;
        private IListaTarefaRepository? _listaTarefaRepository;
        private IGrupoTarefaResponsaveisRepository? _grupoTarefaResponsaveisRepository;
        private ICasoRepository? _casoRepository;
        private IAtendimentoRepository? _atendimentoRepository;
        private IGrupoCasoClienteRepository? _grupoCasoClienteRepository;
        private IGrupoCasoEnvolvidosRepository? _grupoCasoEnvolvidosRepository;
        private IEtiquetaRepository? _etiquetaRepository;
        private IGrupoAtendimentoClienteRepository? _atendimentoClienteRepository;       
        private IEventoRepository? _eventoRepository;
        private IGrupoEventoResponsavelRepository? _grupoEventoResponsavelRepository;
         
        private IGrupoClientesProcessosRepository? _grupoClientesProcessosRepository;
        private IGrupoEnvolvidosProcessoRepository? _grupoEnvolvidosProcessoRepository;
        private IGrupoEtiquetasProcessosRepository? _grupoEtiquetasProcessosRepository;
        private IGrupoEtiquetasAtendimentoRepository? _grupoEtiquetasAtendimentoRepository;
        private IGrupoEtiquetaCasoRepository? _grupoEtiquetaCasoRepository;
        private IAtendimentoHistoricoRepository? _atendimentoHistoricoRepository;
        private INotificacaoRepository? _notificacaoRepository;
        private IIntimacaoRepository? _intimacaoRepository;
        private IPecaCabivelRepository? _pecaCabivelRepository;
        private ILoteTrabalhoRepository? _loteTrabalhoRepository;
        public ILoteTrabalhoRepository LoteTrabalhoRepository
        {
            get
            {
                if (_loteTrabalhoRepository == null)
                    _loteTrabalhoRepository = new LoteTrabalhoRepository(dataContext);

                return _loteTrabalhoRepository;
            }
        }
        public IPecaCabivelRepository PecaCabivelRepository
        {
            get
            {
                if (_pecaCabivelRepository == null)
                    _pecaCabivelRepository = new PecaCabivelRepository(dataContext);

                return _pecaCabivelRepository;
            }
        }
        public IIntimacaoRepository IntimacaoRepository
        {
            get
            {
                if (_intimacaoRepository == null)
                    _intimacaoRepository = new IntimacaoRepository(dataContext);

                return _intimacaoRepository;
            }
        }
        public INotificacaoRepository NotificacaoRepository
        {
            get
            {
                if (_notificacaoRepository == null)
                    _notificacaoRepository = new NotificacaoRepository(dataContext);

                return _notificacaoRepository;
            }
        }
        private IFotoRepository? _fotosRepository;
        public IFotoRepository FotoRepository
        {
            get
            {
                if (_fotosRepository == null)
                    _fotosRepository = new FotoRepository(dataContext);

                return _fotosRepository;
            }
        }

        private IHistoricoGeralRepository? _geralRepository;
        public IHistoricoGeralRepository HistoricoGeralRepository
        {
            get
            {
                if (_geralRepository == null)
                    _geralRepository = new HistoricoGeralRepository(dataContext);

                return _geralRepository;
            }
        }
      
        public IGrupoPessoasEtiquetasRepository? _grupoPessoasEtiquetasRepository;

        public IGrupoPessoasEtiquetasRepository GrupoPessoasEtiquetasRepository
        {
            get
            {
                if (_grupoPessoasEtiquetasRepository == null)
                    _grupoPessoasEtiquetasRepository = new GrupoPessoasEtiquetasRepository(dataContext);

                return _grupoPessoasEtiquetasRepository;
            }
        }
       
      
        private IGrupoEventoEtiquetasRepository? _grupoEventoEtiquetasRepository;
        private IComentarioRepository? _comentarioRepository;
        public IComentarioRepository ComentarioRepository
        {
            get
            {
                if (_comentarioRepository == null)
                    _comentarioRepository = new ComentarioRepository(dataContext);

                return _comentarioRepository;
            }
        }
        public IGrupoEventoEtiquetasRepository GrupoEventoEtiquetasRepository
        {
            get
            {
                if (_grupoEventoEtiquetasRepository == null)
                    _grupoEventoEtiquetasRepository = new GrupoEventoEtiquetasRepository(dataContext);

                return _grupoEventoEtiquetasRepository;
            }
        }
        public IAtendimentoHistoricoRepository AtendimentoHistoricoRepository
        {
            get
            {
                if (_atendimentoHistoricoRepository == null)
                    _atendimentoHistoricoRepository = new AtendimentoHistoricoRepository(dataContext);

                return _atendimentoHistoricoRepository;
            }
        }       
        public IGrupoEtiquetaCasoRepository GrupoEtiquetaCasoRepository
        {
            get
            {
                if (_grupoEtiquetaCasoRepository == null)
                    _grupoEtiquetaCasoRepository = new GrupoEtiquetasCasosRepository(dataContext);

                return _grupoEtiquetaCasoRepository;
            }
        }
        private IQualificacaoRepository? _qualificacaoRepository;
        private IContaBancariaRepository? _contaBancariaRepository;
        private IAcaoRepository? _acaoRepository;
        public IAcaoRepository AcaoRepository
        {
            get
            {
                if (_acaoRepository == null)
                    _acaoRepository = new AcaoRepository(dataContext);

                return _acaoRepository;
            }
        }
        public IContaBancariaRepository ContaBancariaRepository
        {
            get
            {
                if (_contaBancariaRepository == null)
                    _contaBancariaRepository = new ContaBancariaRepository(dataContext);

                return _contaBancariaRepository;
            }
        }
        public IQualificacaoRepository QualificacaoRepository
        {
            get
            {
                if (_qualificacaoRepository == null)
                    _qualificacaoRepository = new QualificacaoRepository(dataContext);

                return _qualificacaoRepository;
            }
        }
        public IGrupoEtiquetasAtendimentoRepository GrupoEtiquetasAtendimentoRepository
        {
            get
            {
                if (_grupoEtiquetasAtendimentoRepository == null)
                    _grupoEtiquetasAtendimentoRepository = new GrupoEtiquetasAtendimentoRepository(dataContext);

                return _grupoEtiquetasAtendimentoRepository;
            }
        }
        public IGrupoEtiquetasProcessosRepository GrupoEtiquetasProcessosRepository
        {
            get
            {
                if (_grupoEtiquetasProcessosRepository == null)
                    _grupoEtiquetasProcessosRepository = new GrupoEtiquetasProcessosRepository(dataContext);

                return _grupoEtiquetasProcessosRepository;
            }
        }
        public IGrupoEnvolvidosProcessoRepository GrupoEnvolvidosProcessosRepository
        {
            get
            {
                if (_grupoEnvolvidosProcessoRepository == null)
                    _grupoEnvolvidosProcessoRepository = new GrupoEnvolvidosProcessosRepository(dataContext);

                return _grupoEnvolvidosProcessoRepository;
            }
        }
        public IGrupoClientesProcessosRepository GrupoClientesProcessosRepository
        {
            get
            {
                if (_grupoClientesProcessosRepository == null)
                    _grupoClientesProcessosRepository = new GrupoClientesProcessoRepository(dataContext);

                return _grupoClientesProcessosRepository;
            }
        }       
       
        public IGrupoEventoResponsavelRepository GrupoEventoResponsavelRepository
        {
            get
            {
                if (_grupoEventoResponsavelRepository == null)
                    _grupoEventoResponsavelRepository = new GrupoEventoResponsavelRepository(dataContext);

                return _grupoEventoResponsavelRepository;
            }
        }
        public IEventoRepository EventoRepository
        {
            get
            {
                if (_eventoRepository == null)
                    _eventoRepository = new EventoRepository(dataContext);

                return _eventoRepository;
            }
        }
         private IGrupoTarefasEtiquetasRepository? _grupoTarefasEtiquetasRepository;
        public IGrupoTarefasEtiquetasRepository GrupoTarefasEtiquetasRepository
        {
            get
            {
                if (_grupoTarefasEtiquetasRepository == null)
                    _grupoTarefasEtiquetasRepository = new GrupoTarefasEtiquetasRepository(dataContext);

                return _grupoTarefasEtiquetasRepository;
            }
        }

        public IGrupoAtendimentoClienteRepository GrupoAtendimentoClienteRepository
        {
            get
            {
                if (_atendimentoClienteRepository == null)
                    _atendimentoClienteRepository = new GrupoAtendimentoClienteRepository(dataContext);

                return _atendimentoClienteRepository;
            }
        }
        public IEtiquetaRepository EtiquetaRepository
        {
            get
            {
                if (_etiquetaRepository == null)
                    _etiquetaRepository = new EtiquetaRepository(dataContext);

                return _etiquetaRepository;
            }
        }
        public IGrupoCasoEnvolvidosRepository GrupoCasoEnvolvidosRepository
        {
            get
            {
                if (_grupoCasoEnvolvidosRepository == null)
                    _grupoCasoEnvolvidosRepository = new GrupoCasoEnvolvidosRepository(dataContext);

                return _grupoCasoEnvolvidosRepository;
            }
        }
        public IGrupoCasoClienteRepository GrupoCasoClienteRepository
        {
            get
            {
                if (_grupoCasoClienteRepository == null)
                    _grupoCasoClienteRepository = new GrupoCasoClienteRepository(dataContext);

                return _grupoCasoClienteRepository;
            }
        }
        public IAtendimentoRepository AtendimentoRepository
        {
            get
            {
                if (_atendimentoRepository == null)
                    _atendimentoRepository = new AtendimentoRepository(dataContext);

                return _atendimentoRepository;
            }
        }
        public ICasoRepository CasoRepository
        {
            get
            {
                if (_casoRepository == null)
                    _casoRepository = new CasoRepository(dataContext);

                return _casoRepository;
            }
        }
        public IGrupoTarefaResponsaveisRepository GrupoTarefaResponsaveisRepository
        {
            get
            {
                if (_grupoTarefaResponsaveisRepository == null)
                    _grupoTarefaResponsaveisRepository = new GrupoTarefaResponsaveisRepository(dataContext);

                return _grupoTarefaResponsaveisRepository;
            }
        }
        public IListaTarefaRepository ListaTarefaRepository
        {
            get
            {
                if (_listaTarefaRepository == null)
                    _listaTarefaRepository = new ListaTarefaRepository(dataContext);

                return _listaTarefaRepository;
            }
        }
        public ITarefaRepository TarefaRepository
        {
            get
            {
                if (_tarefaRepository == null)
                    _tarefaRepository = new TarefaRepository(dataContext);

                return _tarefaRepository;
            }
        }
        public IVaraRepository VaraRepository
        {
            get
            {
                if (_varaRepository == null)
                    _varaRepository = new VaraRepository(dataContext);

                return _varaRepository;
            }
        }
        public IGrupoEnvolvidosRepository GrupoEnvolvidosRepository
        {
            get
            {
                if (_grupoEnvolvidosRepository == null)
                    _grupoEnvolvidosRepository = new GrupoEnvolvidosRepository(dataContext);

                return _grupoEnvolvidosRepository;
            }
        }
        public IGrupoClientesRepository GrupoClientesRepository

        {
            get
            {
                if (_grupoClientesRepository == null)
                    _grupoClientesRepository = new GrupoClientesRepository(dataContext);

                return _grupoClientesRepository;
            }
        }
        public IProcessoRepository ProcessoRepository

        {
            get
            {
                if (_processoRepository == null)
                    _processoRepository = new ProcessoRepository(dataContext);

                return _processoRepository;
            }
        }
        
        public IInformacoesComplementaresRepository InformacoesComplementaresRepository

        {
            get
            {
                if (_informacoesComplementaresRepository == null)
                    _informacoesComplementaresRepository = new InformacoesComplementaresRepository(dataContext);

                return _informacoesComplementaresRepository;
            }
        }
        public IEnderecoRepository EnderecoRepository

        {
            get
            {
                if (_enderecoRepository == null)
                    _enderecoRepository = new EnderecoRepository(dataContext);

                return _enderecoRepository;
            }
        }
        public IPessoaRepository PessoaRepository

        {
            get
            {
                if (_pessoaRepository == null)
                    _pessoaRepository = new PessoaRepository(dataContext);

                return _pessoaRepository;
            }
        }
        public ILoginHistoryRepository LoginHistoryRepository
        {
            get
            {
                if (_loginHistoryRepository == null)
                    _loginHistoryRepository = new LoginHistoryRepository(dataContext);

                return _loginHistoryRepository;
            }
        }
        public IUsuarioRepository UsuarioRepository
        {
            get
            {
                if (_usuarioRepository == null)
                    _usuarioRepository = new UsuarioRepository(dataContext);

                return _usuarioRepository;
            }
        }
        public INivelRepository NivelRepository
        {
            get
            {
                if (_nivelRepository == null)
                    _nivelRepository = new NivelRepository(dataContext);

                return _nivelRepository;
            }
        }
        public ISetorRepository SetorRepository
        {
            get
            {
                if (_setorRepository == null)
                    _setorRepository = new SetorRepository(dataContext);

                return _setorRepository;
            }
        }
        public IGrupoSetoresRepository GrupoSetoresRepository
        {
            get
            {
                if (_grupoSetoresRepository == null)
                    _grupoSetoresRepository = new GrupoSetorRepository(dataContext);

                return _grupoSetoresRepository;
            }
        }
        public IFailedLoginAttemptRepository FailedLoginAttemptRepository
        {
            get
            {
                if (_failedLoginAttemptRepository == null)
                    _failedLoginAttemptRepository = new FailedLoginAttemptRepository(dataContext);

                return _failedLoginAttemptRepository;
            }
        }
        public IGrupoNiveisRepository GrupoNiveisRepository
        {
            get
            {
                if (_grupoNiveisRepository == null)
                    _grupoNiveisRepository = new GrupoNivelRepository(dataContext);

                return _grupoNiveisRepository;
            }
        }

        #endregion
        #region Transaçoes
        //construtor para injeção de dependência 
        private IDbContextTransaction? transaction;
        public async Task BeginTransactionAsync()
        {
            if (transaction == null)

                transaction = await dataContext.Database.BeginTransactionAsync();
        }
        public async Task CommitAsync()
        {
            await dataContext.SaveChangesAsync();

            if (transaction != null)
            {
                await transaction.CommitAsync();
                await transaction.DisposeAsync();
                transaction = null;
            }

            // 🔥 ESSENCIAL (resolve 80% dos seus bugs)
            dataContext.ChangeTracker.Clear();
        }
        public async Task RollbackAsync()
        {
            if (transaction == null)
                return;

            var currentTransaction = transaction;
            transaction = null;

            try
            {
                await currentTransaction.RollbackAsync();
            }
            catch
            {
                // ignorar porque pode já ter sido finalizada
            }
            finally
            {
                await currentTransaction.DisposeAsync();
            }

            dataContext.ChangeTracker.Clear();
        }
        public void Dispose()
        {
            dataContext.Dispose();
            if (transaction != null)
                transaction.Dispose();

        }
        #endregion

    }
}
