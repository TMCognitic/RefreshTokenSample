using Consumer.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Consumer;

public partial class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        User? currentUser = null;

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Call api for getting user at : {time}", DateTimeOffset.Now);
        }
        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://localhost:7085/");

            //Récupération de l'utilisateur
            using (HttpResponseMessage responseMessage = await client.PostAsJsonAsync("api/auth/login", new { Email = "john.doe@test.be", Passwd = "Test1234=" }, stoppingToken))
            {
                if (!responseMessage.IsSuccessStatusCode)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("No User Found!!");
                    }
                    return;
                }

                currentUser = await responseMessage.Content.ReadFromJsonAsync<User>();
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation(JsonSerializer.Serialize(currentUser, new JsonSerializerOptions() { WriteIndented = true }));
                }
            }
        }

        if (currentUser is null)
            return;

        while (!stoppingToken.IsCancellationRequested)
        {
            bool wait = true;
            string token = currentUser.Token!;
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Call api for getting values at : {time}", DateTimeOffset.Now);
            }

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7085/");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                using (HttpResponseMessage responseMessage = await client.GetAsync("api/WeatherForecast", stoppingToken))
                {
                    switch (responseMessage.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            if (_logger.IsEnabled(LogLevel.Information))
                            {
                                _logger.LogInformation($"Getting data succeded");
                            }
                            break;
                        case HttpStatusCode.Unauthorized:
                            if (_logger.IsEnabled(LogLevel.Warning))
                            {
                                _logger.LogWarning("Try to refresh the tokens at {time}", DateTimeOffset.Now);
                            }

                            TokenPair tokenPair = await RefreshToken(token, currentUser.RefreshToken!, stoppingToken);
                            currentUser.Token = tokenPair!.Token;
                            currentUser.RefreshToken = tokenPair.RefreshToken;

                            if (_logger.IsEnabled(LogLevel.Warning | LogLevel.Information))
                            {
                                _logger.LogWarning("Tokens are refresh at {time}", DateTimeOffset.Now);
                                _logger.LogInformation(JsonSerializer.Serialize(currentUser, new JsonSerializerOptions() { WriteIndented = true }));
                            }
                            wait = false;
                            break;
                        default:
                            if (_logger.IsEnabled(LogLevel.Error))
                            {
                                _logger.LogError($"Call api don't work : Status Code {responseMessage.StatusCode}");
                            }
                            break;

                    }                    
                }
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Waiting 10 seconds");
            }

            if(wait)
                await Task.Delay(10000, stoppingToken);
        }        
    }

    private async Task<TokenPair> RefreshToken(string oldToken, string oldRefreshToken, CancellationToken stoppingToken)
    {
        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://localhost:7085/");

            //Récupération les nouveaux token
            using (HttpResponseMessage responseMessage = await client.PostAsJsonAsync("api/auth/refresh", new { Token = oldToken, RefreshToken = oldRefreshToken }, stoppingToken))
            {
                responseMessage.EnsureSuccessStatusCode();

                return await responseMessage.Content.ReadFromJsonAsync<TokenPair>();
            }
        }
    }
}
