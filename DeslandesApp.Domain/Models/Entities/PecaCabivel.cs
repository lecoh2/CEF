using DeslandesApp.Domain.Commons;
using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Entities
{
    public class PecaCabivel : BaseEntity
    {
      

        public string NomePeca { get; set; }

        public int PrazoDias { get; set; }

        public ComplexidadeEnum SugestaoComplexidadePadrao { get; set; }
        // =========================
        // EXCLUSÃO LÓGICA
        // =========================
        public bool Excluido { get; set; }

        public DateTime? DataExclusao { get; set; }

        public Guid? UsuarioExclusaoId { get; set; }
    }
}
