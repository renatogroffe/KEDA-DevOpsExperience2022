using Microsoft.AspNetCore.Mvc;
using SiteQuestaoEventHub.EventHubs;

namespace SiteQuestaoEventHub.Controllers;

public class VotacaoController : Controller
{
    private readonly ILogger<VotacaoController> _logger;
    private readonly VotacaoProducer _producer;

    public VotacaoController(ILogger<VotacaoController> logger,
        VotacaoProducer producer)
    {
        _logger = logger;
        _producer = producer;
    }

    public async Task<IActionResult> VotoKubernetes()
    {
        return await ProcessarVoto("Kubernetes");
    }

    public async Task<IActionResult> VotoLinux()
    {
        return await ProcessarVoto("Linux");
    }

    public async Task<IActionResult> VotoAzure()
    {
        return await ProcessarVoto("Azure");
    }

    public async Task<IActionResult> VotoPowerShell()
    {
        return await ProcessarVoto("PowerShell");
    }

    private async Task<IActionResult> ProcessarVoto(string tecnologia)
    {
        _logger.LogInformation($"Processando voto para a tecnologia: {tecnologia}");
        await _producer.Send(tecnologia);
        _logger.LogInformation($"Informações sobre o voto '{tecnologia}' enviadas para o Azure Event Hubs!");

        TempData["Voto"] = tecnologia;
        return RedirectToAction("Index", "Home");
    }
}