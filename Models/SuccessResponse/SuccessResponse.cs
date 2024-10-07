using Newtonsoft.Json;

namespace OnboardingSystem.Models.SuccessResponse;

public class SuccessResponse
{
    [JsonProperty("message")]
    public String Message { get; set; }
}