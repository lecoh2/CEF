using DeslandesApp.Domain.Commons;
using DeslandesApp.Domain.Models.Entities;
using DeslandesApp.Domain.Models.Enum;
public class Processo : BaseEntity
{
    public Guid VaraId { get; set; }
    public Vara Vara { get; set; } = null!;

    public Guid? AcaoId { get; set; }

    public Guid? UsuarioResponsavelId { get; set; }
    public Usuario? UsuarioResponsavel { get; set; }
    public Guid? UsuarioCadastroId { get; set; }
    public Usuario? UsuarioCadastro { get; set; }
    public string? Pasta { get; set; }
    public string? Titulo { get; set; }
    public string? NumeroProcesso { get; set; }
    public string? LinkTribunal { get; set; }
    public string? Objeto { get; set; }
    public decimal? ValorCausa { get; set; }
    public DateOnly? Distribuido { get; set; }
    public decimal? ValorCondenacao { get; set; }

    public string? Observacao { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public Acao? Acao { get; set; }
   // public List<ProcessoEtiqueta> ProcessoEtiquetas { get; set; } = new();
    public ICollection<GrupoEnvolvidos> GrupoEnvolvidos { get; set; }
    public ICollection<GrupoClienteProcesso> GrupoClienteProcesso { get; set; }
    public ICollection<GrupoEtiquetasProcessos> GrupoEtiquetasProcessos { get; set; }
    public ICollection<GrupoEnvolvidosProcesso> GrupoEnvolvidosProcesso { get; set; }
    public Instancia? Instancia { get; set; }
    public Acesso? Acesso { get; set; }
    public Guid? ParteAdversaId { get; set; }

    public virtual Pessoa? ParteAdversa { get; set; }

}