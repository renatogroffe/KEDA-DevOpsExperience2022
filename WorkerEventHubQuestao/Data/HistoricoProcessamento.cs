using Dapper.Contrib.Extensions;

namespace WorkerEventHubQuestao.Data;

[Table("dbo.HistoricoProcessamento")]
public class HistoricoProcessamento
{
    [Key]
    public int Id { get; set; }
    public string? IdVoto { get; set; }
    public DateTime? Horario { get; set; }
    public string? Producer { get; set; }
    public string? Consumer { get; set; }
}