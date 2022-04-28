using System.Text;
using System.Text.Json;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using WorkerEventHubQuestao.Data;
using WorkerEventHubQuestao.EventHubs;

namespace WorkerEventHubQuestao;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly VotacaoRepository _repository;
    private readonly string _eventHub;
    private readonly EventProcessorClient _processor;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public Worker(ILogger<Worker> logger,
        IConfiguration configuration,
        VotacaoRepository repository)
    {
        _logger = logger;
        _repository = repository;

        _eventHub = configuration["AzureEventHubs:EventHub"];
        var consumerGroup = configuration["AzureEventHubs:ConsumerGroup"];
        var blobContainer = configuration["AzureEventHubs:BlobContainer"];
        
        _processor = new EventProcessorClient(
            new BlobContainerClient(
                configuration["AzureEventHubs:BlobStorageConnectionString"],
                blobContainer),
            consumerGroup,
            configuration["AzureEventHubs:EventHubsConnectionString"],
            _eventHub);
        _processor.ProcessEventAsync += ProcessEventHandler;
        _processor.ProcessErrorAsync += ProcessErrorHandler;

        _logger.LogInformation($"Event Hub = {_eventHub}");
        _logger.LogInformation($"Consumer Group = {consumerGroup}");
        _logger.LogInformation($"Blob Container = {blobContainer}");

        _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Run(() =>
            {
                _processor.StartProcessing();
            });

            _logger.LogInformation(
                $"Worker ativo em: {DateTime.Now:yyyy-MM-dd HH:mm}");
            await Task.Delay(60000, stoppingToken);
        }
    }
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executando Stop...");
        _processor.StopProcessing();
        return base.StopAsync(cancellationToken);
    }

    private async Task ProcessEventHandler(ProcessEventArgs eventArgs)
    {
        var eventData = Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray());
        _logger.LogInformation($"[{DateTime.Now:HH:mm:ss} Evento] " + eventData);

        var dataProcessed = false;
        QuestaoEventData? questaoEventData = null;
        try
        {
            questaoEventData = JsonSerializer.Deserialize<QuestaoEventData>(
                eventData, _jsonSerializerOptions);
        }
        catch
        {
            _logger.LogError(
                "Erro durante a deserializacao dos dados recebidos!");
        }

        if (questaoEventData is not null)
        {
            if (!String.IsNullOrWhiteSpace(questaoEventData.IdVoto) &
                !String.IsNullOrWhiteSpace(questaoEventData.Horario))
            {
                if (!String.IsNullOrWhiteSpace(questaoEventData.Tecnologia))
                {
                    _repository.SaveVotoTecnologia(questaoEventData);
                    _logger.LogInformation($"Voto = {questaoEventData.IdVoto} | " +
                        $"Tecnologia = {questaoEventData.Tecnologia} | " +
                        "Evento computado com sucesso!");
                    dataProcessed = true;
                }
                else if (!String.IsNullOrWhiteSpace(questaoEventData.Instancia))
                {
                    _repository.SaveHistoricoProcessamento(questaoEventData);
                    _logger.LogInformation($"Voto = {questaoEventData.IdVoto} | " +
                        $"Instância = {questaoEventData.Instancia} | " +
                        "Evento computado com sucesso!");
                    dataProcessed = true;
                }
            }
        }

        if (!dataProcessed)
            _logger.LogError("Formato dos dados do evento inválido!");

        await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
    }

    private Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
    {
        _logger.LogError($"Error Handler Exception: {eventArgs.Exception.Message}");
        return Task.CompletedTask;
    }
}