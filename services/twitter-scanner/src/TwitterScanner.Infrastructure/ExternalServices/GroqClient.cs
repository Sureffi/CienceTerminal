using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using TwitterScanner.Application.Interfaces;
using TwitterScanner.Domain.Groq;

namespace TwitterScanner.Infrastructure.ExternalServices;

// TODO: Make generic client for using groq

public class GroqClient : IGroqClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    private readonly string _apiKey;

    public GroqClient(IConfiguration configuration)
    {
        _httpClient = new HttpClient();

        _baseUrl = configuration["Endpoints:Groq"];
        _apiKey = configuration["ApiKeys:Groq"];
    }

    public async Task<GroqResponse?> GenerateAsync(GroqPrompt prompt)
    {
        try
        {
            // Create request message to avoid header conflicts
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, _baseUrl);

            // Set authorization header on the request message (not on the client)
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            // Serialize request
            var json = JsonSerializer.Serialize(prompt);
            requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send request
            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            // Parse Groq response (not Ollama format)
            var groqResponse = JsonSerializer.Deserialize<GroqResponse>(responseJson);

            return groqResponse;
        }
        catch (HttpRequestException)
        {
            // Http request failed
            return null;
        }
        catch (JsonException)
        {
            // Error deserializing json
            return null;
        }


    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
