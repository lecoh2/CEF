using DeslandesApp.Domain.Commons;
using DeslandesApp.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Entities
{
    public class Atendimento : BaseEntity
    {
        public string Assunto { get; set; } = string.Empty;
        public string Registro { get; set; } = string.Empty;

        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }       

        // 🔹 Relacionamentos possíveis
        public Guid? ProcessoId { get; set; }
        public Processo? Processo { get; set; }

        public Guid? CasoId { get; set; }
        public Caso? Caso { get; set; }
        public Guid? ResponsavelId { get; set; }
        public Usuario? Responsavel { get; set; }
        public Guid? AtendimentoPaiId { get; set; }
        public Atendimento? AtendimentoPai { get; set; }

        // 🔹 Clientes
        public List<GrupoAtendimentoCliente> GrupoClientes { get; set; } = new();
        public List<GrupoEtiquetasAtendimentos> GrupoEtiquetasAtendimentos { get; set; } = new();
        public TipoVinculo? TipoVinculoId { get; set; }
        public void ValidarVinculo()
        {
            int count = 0;

            if (ProcessoId.HasValue) count++;
            if (CasoId.HasValue) count++;
            if (AtendimentoPaiId.HasValue) count++;

            if (count > 1)
                throw new Exception("O atendimento não pode ter mais de um vínculo.");

            if (TipoVinculoId == TipoVinculo.Processo && !ProcessoId.HasValue)
                throw new Exception("TipoVinculo Processo inválido.");

            if (TipoVinculoId == TipoVinculo.Caso && !CasoId.HasValue)
                throw new Exception("TipoVinculo Caso inválido.");

            if (TipoVinculoId == TipoVinculo.Atendimento && !AtendimentoPaiId.HasValue)
                throw new Exception("TipoVinculo Atendimento inválido.");
        }
        public void DefinirVinculo(Guid? processoId, Guid? casoId, Guid? atendimentoPaiId)
        {
            // ✅ valida múltiplos vínculos
            int total = 0;

            if (processoId.HasValue) total++;
            if (casoId.HasValue) total++;
            if (atendimentoPaiId.HasValue) total++;

            if (total > 1)
                throw new Exception("O atendimento só pode possuir um vínculo.");

            // ✅ limpa tudo antes
            ProcessoId = null;
            Processo = null;

            CasoId = null;
            Caso = null;

            AtendimentoPaiId = null;
            AtendimentoPai = null;

            // ✅ define vínculo
            if (processoId.HasValue)
            {
                ProcessoId = processoId;
                TipoVinculoId = TipoVinculo.Processo;
                return;
            }

            if (casoId.HasValue)
            {
                CasoId = casoId;
                TipoVinculoId = TipoVinculo.Caso;
                return;
            }

            if (atendimentoPaiId.HasValue)
            {
                AtendimentoPaiId = atendimentoPaiId;
                TipoVinculoId = TipoVinculo.Atendimento;
                return;
            }

            // ✅ sem vínculo
            TipoVinculoId = null;
        }
    }

}

