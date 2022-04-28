using Microsoft.Data.SqlClient;
using Dapper.Contrib.Extensions;
using WorkerEventHubQuestao.EventHubs;

namespace WorkerEventHubQuestao.Data;

public class VotacaoRepository
{
    private readonly IConfiguration _configuration;

    public VotacaoRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SaveHistoricoProcessamento(QuestaoEventData questaoEventData)
    {
        using var conexao = new SqlConnection(
            _configuration.GetConnectionString("BaseVotacao"));
        conexao.Insert<HistoricoProcessamento>(new()
        {
            IdVoto = questaoEventData.IdVoto,
            Horario = Convert.ToDateTime(questaoEventData.Horario),
            Producer = questaoEventData.Instancia,
            Consumer = Environment.MachineName
        });
    }

    public void SaveVotoTecnologia(QuestaoEventData questaoEventData)
    {
        using var conexao = new SqlConnection(
            _configuration.GetConnectionString("BaseVotacao"));
        conexao.Insert<VotoTecnologia>(new()
        {
            IdVoto = questaoEventData.IdVoto,
            Horario = Convert.ToDateTime(questaoEventData.Horario),
            Tecnologia = questaoEventData.Tecnologia,
            Consumer = Environment.MachineName
        });
    }
}