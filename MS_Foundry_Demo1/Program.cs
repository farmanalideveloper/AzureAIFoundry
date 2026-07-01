using Azure;
using Azure.AI.OpenAI;
using DotNetEnv;
using OpenAI.Chat;

Env.Load();

string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")!;
string apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY")!;
string deployment = Environment.GetEnvironmentVariable("MODEL_DEPLOYMENT")!;

var client = new AzureOpenAIClient(
    new Uri(endpoint),
    new AzureKeyCredential(apiKey));

var chatClient = client.GetChatClient(deployment);

Console.WriteLine("Connected to Azure AI Foundry");
Console.WriteLine("-------------------------------------");

while (true)
{
    Console.Write("\nYou: ");
    string? userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput))
        continue;

    if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    var messages = new List<ChatMessage>
    {
        new SystemChatMessage("You are a helpful AI assistant."),
        new UserChatMessage(userInput)
    };

    ChatCompletion completion = chatClient.CompleteChat(messages);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\nAssistant:");
    Console.ResetColor();

    Console.WriteLine(completion.Content[0].Text);
}