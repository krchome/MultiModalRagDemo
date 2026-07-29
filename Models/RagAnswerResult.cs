namespace MultiModalRagDemo.Models
{
    public class RagAnswerResult
    {
        public bool Succeeded { get; init; }
        public string Answer { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;

        public static RagAnswerResult Success(string answer) =>
            new()
            {
                Succeeded = true,
                Answer = answer,
                Message = "The answer was generated from the retrieved document context."
            };

        public static RagAnswerResult Failure(string message) =>
            new()
            {
                Succeeded = false,
                Message = message
            };

    }
}
