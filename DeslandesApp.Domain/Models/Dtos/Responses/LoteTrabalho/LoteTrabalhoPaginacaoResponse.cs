using DeslandesApp.Domain.Models.Enum.DeslandesApp.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Responses.LoteTrabalho
{
    public class LoteTrabalhoPaginacaoResponse
   (
         Guid Id,

         string NumeroLote,

         Guid ResponsavelId,
         string ResponsavelNome,

         Guid CoordenadorId,

         string CoordenadorNome,
         DateTime DataCriacao,

         DateTime DataPrazoLote,

         StatusLote Status,

         int QuantidadeIntimacoes
    );
}
