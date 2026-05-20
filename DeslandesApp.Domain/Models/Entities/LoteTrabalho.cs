using DeslandesApp.Domain.Commons;
using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Entities
{
    public class LoteTrabalho : BaseEntity
    {      

        public string NumeroLote { get; set; }

        public Guid ResponsavelId { get; set; }

        public Guid CoordenadorId { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime DataPrazoLote { get; set; }

        public StatusLote Status { get; set; }
        // Exclusão lógica
        public bool Excluido { get; set; }

        public DateTime? DataExclusao { get; set; }

        public Guid? UsuarioExclusaoId { get; set; }

        // Navegação
        public ICollection<Intimacao> Intimacoes { get; set; }

        public Usuario Responsavel { get; set; } 

        public Usuario Coordenador { get; set; }
    
    }
}
