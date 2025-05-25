using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq; // Required for FirstOrDefault and others if used
using System.Text.Json.Serialization;

namespace AITodoApp.Services
{
    public class TaskSuggestion
    {
        public string Title { get; set; } = "";
        public string Category { get; set; } = "";
        public string Priority { get; set; } = "";
    }

    // Request payload for Gemini API
    public class GeminiRequest
    {
        [JsonPropertyName("contents")]
        public Content[]? Contents { get; set; } // Made nullable

        // Optional: Add generationConfig if you need to control temperature, max tokens etc.
        // [JsonPropertyName("generationConfig")]
        // public GenerationConfig GenerationConfig { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("parts")]
        public Part[]? Parts { get; set; } // Made nullable
    }

    public class Part
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; } // Made nullable
    }

    // Response structure (simplified for this use case)
    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public Candidate[]? Candidates { get; set; } // Made nullable

        // You might also want to handle PromptFeedback if present
        // [JsonPropertyName("promptFeedback")]
        // public PromptFeedback PromptFeedback { get; set; }
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public Content? Content { get; set; } // Made nullable
        // Other properties like finishReason, index, safetyRatings can be added if needed
    }


    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _modelName; // e.g., "gemini-1.5-flash-latest" or "gemini-1.0-pro"

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "Gemini:ApiKey config missing");
            _modelName = configuration["Gemini:ModelName"] ?? "gemini-1.5-flash-latest"; // Default to a recent model
        }

        public async Task<TaskSuggestion> GetDetailedSuggestionAsync()
        {
            // Use v1beta for broader model access, or v1 if your model is GA on v1
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_modelName}:generateContent?key={_apiKey}";

            // --- PROMPT TRANSLATED TO ENGLISH ---
            var promptText = @"
Suggest a productive task for today.
Return it only in JSON format with these properties: Title, Category, Priority.
Ensure the response is solely the JSON object, without any additional text or markdown.

Example of required JSON output format:
{""Title"": ""Finish sales presentation"", ""Category"": ""Work"", ""Priority"": ""High""}";
            // --- END OF TRANSLATED PROMPT ---

            var requestPayload = new GeminiRequest
            {
                Contents = new[]
                {
                    new Content
                    {
                        Parts = new[]
                        {
                            new Part { Text = promptText }
                        }
                    }
                }
            };

            var jsonRequest = JsonSerializer.Serialize(requestPayload, new JsonSerializerOptions
            {
                // Ensure nulls are not included if that's the API preference, though usually not an issue for this structure
                // DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // Log the full response body for debugging
                Console.WriteLine($"Error from Gemini API: {response.StatusCode}");
                Console.WriteLine($"Response Body: {responseBody}");
                throw new Exception($"Gemini API Error: {response.StatusCode} - {responseBody}"); // Slightly rephrased error message
            }

            string outputText = "";
            try
            {
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseBody);
                // Check if Candidates, the first Candidate, its Content, its Parts, and the first Part's Text are all non-null
                if (geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text != null)
                {
                    // Ensure that Candidates is not null and has at least one element before accessing index 0
                    // Same for Parts within Content.
                    outputText = geminiResponse.Candidates[0].Content.Parts[0].Text;
                }
            }
            catch (JsonException jsonEx)
            {
                // If the overall response isn't the expected GeminiResponse structure
                Console.WriteLine($"Error deserializing Gemini response: {jsonEx.Message}");
                Console.WriteLine($"Raw response body: {responseBody}");
                // Fallback or throw, depending on how critical this is.
                // For now, we'll let it proceed to the next block and try to parse outputText if it got populated some other way
                // or if it remains empty, it will return a default message.
            }


            if (string.IsNullOrEmpty(outputText))
            {
                return new TaskSuggestion { Title = "No suggestion available from Gemini." };
            }

            // The model might wrap the JSON in markdown backticks.
            string cleanedJson = outputText.Trim();
            if (cleanedJson.StartsWith("```json"))
            {
                cleanedJson = cleanedJson.Substring(7); // Remove ```json
                if (cleanedJson.EndsWith("```"))
                {
                    cleanedJson = cleanedJson.Substring(0, cleanedJson.Length - 3);
                }
            }
            else if (cleanedJson.StartsWith("```")) // Just ```
            {
                 cleanedJson = cleanedJson.Substring(3);
                 if (cleanedJson.EndsWith("```"))
                 {
                    cleanedJson = cleanedJson.Substring(0, cleanedJson.Length - 3);
                 }
            }
            cleanedJson = cleanedJson.Trim();


            try
            {
                // Attempt to deserialize the JSON content from the model's text part
                var suggestion = JsonSerializer.Deserialize<TaskSuggestion>(cleanedJson);
                if (suggestion == null)
                {
                    return new TaskSuggestion { Title = "Could not deserialize suggestion (result was null): " + cleanedJson };
                }
                // Ensure default values if properties are missing after deserialization
                suggestion.Title ??= "Deserialized title was null";
                suggestion.Category ??= "Uncategorized";
                suggestion.Priority ??= "Medium";
                return suggestion;
            }
            catch (JsonException ex)
            {
                // If the text part is not valid JSON, return the text in Title
                Console.WriteLine($"Error deserializing suggestion JSON: {ex.Message}");
                Console.WriteLine($"Cleaned JSON string that failed: {cleanedJson}");
                return new TaskSuggestion { Title = "Received non-JSON suggestion: " + outputText };
            }
        }
    }
}