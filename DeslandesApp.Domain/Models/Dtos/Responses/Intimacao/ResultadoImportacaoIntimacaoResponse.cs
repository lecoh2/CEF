using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeslandesApp.Domain.Models.Dtos.Responses.Intimacao
{
    public record ResultadoImportacaoIntimacaoResponse
    {
        // ================== RESUMO ==================

        public int TotalLinhas { get; init; }

        public int TotalImportadas { get; init; }

        public int TotalDuplicadas { get; init; }

        public int TotalProcessosNaoEncontrados { get; init; }

        public int TotalErros { get; init; }

        // ================== DETALHES ==================

        public List<IntimacaoImportadaResponse> Importadas { get; init; } = [];

        public List<IntimacaoDuplicadaResponse> Duplicadas { get; init; } = [];

        public List<ProcessoNaoEncontradoResponse> ProcessosNaoEncontrados { get; init; } = [];

        public List<string> Erros { get; init; } = [];
    }
}
