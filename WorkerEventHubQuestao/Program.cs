using WorkerEventHubQuestao;
using WorkerEventHubQuestao.Data;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<VotacaoRepository>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();