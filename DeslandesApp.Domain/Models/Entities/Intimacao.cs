using DeslandesApp.Domain.Commons;
using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Entities
{
    public class Intimacao : BaseEntity
    {
  

        public Guid ProcessoId { get; set; }
        public virtual Processo Processo { get; set; }

        public DateTime DataImportacao { get; set; }

        public DateTime DataIntimacao { get; set; }

        public string TextoIntimacao { get; set; }

        public StatusTriagem StatusTriagem { get; set; }

        public Guid? PecaCabivelId { get; set; }
        public virtual PecaCabivel? PecaCabivel { get; set; }

        public DateTime? PrazoIndividualCpc { get; set; }

        public ComplexidadeEnum? ComplexidadeReal { get; set; }

        public string? ObservacaoTriagem { get; set; }

        public Guid? AdvogadoId { get; set; }
        public virtual Usuario? Advogado { get; set; }

        public Guid? LoteId { get; set; }
        public virtual LoteTrabalho? Lote { get; set; }

        public StatusCumprimento StatusCumprimento { get; set; }
        public DateTime? DataTriagem { get; set; }
    }
}
