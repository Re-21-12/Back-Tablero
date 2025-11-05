using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;


public interface ISocketService
{
    Task<bool> SendEventAsync(string typeEvent, object data);
}

public class SocketServiceConfig
{
    public string BaseUrl { get; set; } = "http://localhost:3000";
}

public class SocketService : ISocketService
{
    private readonly HttpClient _httpClient;
    private readonly SocketServiceConfig _config;

    public SocketService(HttpClient httpClient, IOptions<SocketServiceConfig> config)
    {
        _httpClient = httpClient;
        _config = config.Value;
    }


    public async Task<bool> SendEventAsync(string typeEvent, object data)
    {
        var url = $"{_config.BaseUrl}/emit/";

        var jsonPayload = System.Text.Json.JsonSerializer.Serialize(new
        {
            data
        });

        try
        {

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"SocketService Response: {responseContent}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SocketService Error: {ex.Message}");
            return false;
        }

        return true;
    }
}