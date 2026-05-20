using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Infra.Data.Contexts
{
    public class DataContext : DbContext
    {
        //Método construtor para receber por meio de injeção de dependência 
        //as configurações do banco de dados, como conexão, tipo etc. 
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<LoginHistory> LoginHistory { get; set; }
        public DbSet<FailedLoginAttempt> FailedLoginAttempt { get; set; }
        public DbSet<Niveis> Niveis { get; set; }
        public DbSet<GrupoNiveis> GrupoNiveis { get; set; }
        public DbSet<Setor> Setor { get; set; }
        public DbSet<GrupoSetores> GrupoSetores { get; set; }
        public DbSet<Endereco> Endereco { get; set; }
        public DbSet<InformacoesComplementaresPessoaFisica> InformacoesComplementaresPessoaFisicas { get; set; }
        public DbSet<PessoaFisica> PessoasFisicas { get; set; }
        public DbSet<PessoaJuridica> PessoaJuridica { get; set; }
        public DbSet<Qualificacao> Qualificacao { get; set; }
        
        public DbSet<AtendimentoHistorico> AtendimentoHistorico { get; set; }
        public DbSet<InformacoesComplementaresPessoaJuridica> InformacoesComplementaresPessoaJuridicas { get; set; }
        public DbSet<Processo> Processos { get; set; }
        public DbSet<GrupoPessoaClientes> GrupoPessoaClientes { get; set; }
        public DbSet<GrupoEnvolvidos> GrupoEnvolvidos { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; }
        public DbSet<ListaTarefa> ListasTarefa { get; set; }
        public DbSet<GrupoTarefaResponsaveis> GrupoTarefaResponsveis { get; set; }
        public DbSet<Atendimento> Atendimento { get; set; }
        public DbSet<Caso> Caso { get; set; }
        public DbSet<GrupoCasoCliente> GrupoCasoCliente { get; set; }
        public DbSet<GrupoCasoEnvolvido> GrupoCasoEnvolvido { get; set; }
        public DbSet<GrupoAtendimentoCliente> GrupoAtendimentoCliente { get; set; }
        public DbSet<GrupoEventoResponsavel> GrupoEventoResponsavel { get; set; }
        public DbSet<Etiqueta> Etiqueta { get; set; }
        public DbSet<Evento> Evento { get; set; }
        public DbSet<GrupoClienteProcesso> GrupoClienteProcesso { get; set; }
        public DbSet<GrupoEnvolvidosProcesso> GrupoEnvolvidosProcesso { get; set; }
        public DbSet<GrupoEtiquetasProcessos> GrupoEtiquetasProcessos { get; set; }
        public DbSet<GrupoEtiquetaCasos> GrupoEtiquetaCasos { get; set; }
        public DbSet<GrupoTarefaResponsaveis> GrupoTarefaResponsaveis { get; set; }
        public DbSet<GrupoEtiquetasAtendimentos> GrupoEtiquetasAtendimentos { get; set; }      
      
        public DbSet<GrupoPessoasEtiquetas> GrupoPessoasEtiquetas { get; set; }
        public DbSet<ContaBancaria> ContaBancaria { get; set; }
        public DbSet<Vara> Varas { get; set; }
        public DbSet<GrupoTarefasEtiquetas> GrupoTarefasEtiquetas { get; set; }
        public DbSet<GrupoClienteProcesso> GrupoClientesProcesso { get; set; }
        public DbSet<ListaTarefa> ListaTarefa { get; set; }
        public DbSet<GrupoEventoEtiquetas> GrupoEventoEtiquetas { get; set; }
        public DbSet<Comentario> Comentario { get; set; }
        public DbSet<HistoricoGeral> HistoricoGeral { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<PecaCabivel> PecaCabivel { get; set; }
        public DbSet<Intimacao> Intimacao { get; set; }
        public DbSet<LoteTrabalho> LoteTrabalho { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔥 Aplica todos os mappings do Fluent API
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // 🔥 PADRONIZAÇÃO GLOBAL
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // ✅ Nome da tabela em UPPERCASE
                entity.SetTableName(entity.GetTableName()?.ToUpper());

                // ✅ Nome das colunas + regras padrão
                foreach (var property in entity.GetProperties())
                {
                    // 🔥 Nome da coluna em UPPERCASE
                    property.SetColumnName(property.GetColumnBaseName().ToUpper());

                    // 🔤 STRING → varchar(250) default
                    if (property.ClrType == typeof(string))
                    {
                        property.SetIsUnicode(false);

                        if (!property.GetMaxLength().HasValue)
                            property.SetMaxLength(250);
                    }

                    // 💰 DECIMAL → decimal(18,2)
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        property.SetColumnType("decimal(18,2)");
                    }
                }

                // 🔑 PRIMARY KEY NAME
                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName()?.ToUpper());
                }

                // 🔗 FOREIGN KEY NAME
                foreach (var fk in entity.GetForeignKeys())
                {
                    fk.SetConstraintName(fk.GetConstraintName()?.ToUpper());
                }

                // 📊 INDEX NAME
                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(index.GetDatabaseName()?.ToUpper());
                }
            }
        }
    }
}
