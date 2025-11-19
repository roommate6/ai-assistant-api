using Microsoft.AspNetCore.Mvc;

using HCI.AIAssistant.API.Models.DTOs.AIAssistantController;
using HCI.AIAssistant.API.Services;
using HCI.AIAssistant.API.Models.DTOs;

namespace HCI.AIAssistant.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIAssistantController : ControllerBase
{
    private readonly ISecretsService _secretsService;
    private readonly IAppConfigurationsService _appConfigurationsService;
    private readonly IAIAssistantService _aIAssistantService;
    private readonly IParametricFunctions _parametricFunctions;

    public AIAssistantController(
        ISecretsService secretsService,
        IAppConfigurationsService appConfigurationsService,
        IAIAssistantService aIAssistantService,
        IParametricFunctions parametricFunctions
    )
    {
        _secretsService = secretsService;
        _appConfigurationsService = appConfigurationsService;
        _aIAssistantService = aIAssistantService;
        _parametricFunctions = parametricFunctions;
    }

    [HttpPost("/message")]
    [ProducesResponseType(typeof(AIAssistantControllerPostMessageResponseDTO), 200)]
    [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
    public async Task<ActionResult> PostMessage([FromBody] AIAssistantControllerPostMessageRequestDTO request)
    {
        Console.WriteLine(_secretsService?.AIAssistantSecrets?.EndPoint);
        Console.WriteLine(_secretsService?.AIAssistantSecrets?.Key);
        Console.WriteLine(_secretsService?.AIAssistantSecrets?.Id);
        Console.WriteLine(_secretsService?.IoTHubSecrets?.ConnectionString);
        Console.WriteLine(_appConfigurationsService?.KeyVaultName);
        Console.WriteLine(_appConfigurationsService?.SecretsPrefix);
        Console.WriteLine(_appConfigurationsService?.IoTDeviceName);

        if (!_parametricFunctions.ObjectExistsAndHasNoNullPublicProperties(request))
        {
            return BadRequest(
                new ErrorResponseDTO()
                {
                    TextErrorTitle = "AtLeastOneNullParameter",
                    TextErrorMessage = "Some parameters are null/missing.",
                    TextErrorTrace = _parametricFunctions.GetCallerTrace()
                }
            );
        }

#pragma warning disable CS8604
        string textMessageResponse = await _aIAssistantService.SendMessageAndGetResponseAsync(request.TextMessage);
#pragma warning restore CS8604

        AIAssistantControllerPostMessageResponseDTO response = new()
        {
            TextMessage = textMessageResponse
        };

        return Ok(response);
    }
}
