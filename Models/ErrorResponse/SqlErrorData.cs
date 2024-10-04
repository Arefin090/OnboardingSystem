namespace OnboardingSystem.Models.ErrorResponse;

public class SqlErrorData
{
    public int ServerErrorCode { get; set; }
    public string SqlState { get; set; }
}