using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

// Minimal, SDK-free implementation that uses the agent HTTP endpoint directly.
// Reads ProjectEndpoint and AgentId from appsettings.json (if present) and falls back to
// sensible defaults used in other demos.

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .Build();

//string projectEndpoint = configuration["ProjectEndpoint"] ?? "https://project63147600-resource.services.ai.azure.com/api/projects/Project63147600";
//string agentId = configuration["AgentId"] ?? "ConsoleChatAgent";
//string apiVersion = "2024-10-21";

//string agentResponsesEndpoint = $"{projectEndpoint.TrimEnd('/')}/agents/{agentId}/endpoint/protocols/openai/responses?api-version={apiVersion}";
//string projectEndpoint = configuration["ProjectEndpoint"] ?? "...";
//string agentId = configuration["AgentId"] ?? "A1";

// 1. Update this to the GA version
//string apiVersion = "2024-10-21";

// 2. This line in your original code will now build the perfect URL!   
// Replace your dynamic string builder with this hardcoded absolute URI
//string agentResponsesEndpoint = "https://project63147600-resource.services.ai.azure.com/api/projects/Project63147600/agents/A1/endpoint/protocols/openai/responses?api-version=v1";
string agentResponsesEndpoint = "https://Project63149150-resource.services.ai.azure.com/api/projects/Project63149150/agents/A1/endpoint/protocols/openai/responses?api-version=v1";
Console.WriteLine("Opening browser to authenticate (InteractiveBrowserCredential)...");

try
{
    // var credential = new InteractiveBrowserCredential();
    //var credential = new DeviceCodeCredential();
    //var tokenRequestContext = new TokenRequestContext(new[] { "https://cognitiveservices.azure.com/.default" });
    //var accessToken = await credential.GetTokenAsync(tokenRequestContext);

    using HttpClient http = new HttpClient();

    // Add your copied key directly to the headers
    http.DefaultRequestHeaders.Add("api-key", "9d2Zy2DNM08N2woThrFcozY24LdLKkOpOiQZp4I5OGvr3iUPg5BxJQQJ99CGACYeBjFXJ3w3AAAAACOGb4wN");

    Console.WriteLine("\n--- Chat Started (type 'exit' to quit) ---");

    //using HttpClient http = new HttpClient();
    //http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);

    Console.WriteLine("\n--- Chat Started (type 'exit' to quit) ---");

    while (true)
    {
        Console.Write("\nYou: ");
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) continue;
        if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

        //var payload = new
        //{
        //    messages = new[] { new { role = "user", content = input } }
        //};
        // ✅ New Responses API format
        var payload = new
        {
            input = new[] { new { role = "user", content = input } }
        };

        string json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await http.PostAsync(agentResponsesEndpoint, content);
        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"[HTTP {response.StatusCode}] Request failed.\n{responseBody}");
            continue;
        }

        try
        {
            var parsed = JsonSerializer.Deserialize<JsonElement>(responseBody);
            Console.WriteLine("\n--- Agent Response ---");
            Console.WriteLine(JsonSerializer.Serialize(parsed, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch
        {
            Console.WriteLine("Received response but failed to parse JSON:");
            Console.WriteLine(responseBody);
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[Error] {ex.Message}");
}
