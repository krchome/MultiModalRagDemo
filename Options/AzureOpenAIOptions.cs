namespace MultiModalRagDemo.Options
{
    public class AzureOpenAIOptions
    {
        public const string SectionName = "AzureOpenAI";

        public string Endpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string DeploymentName { get; set; } = "gpt-5.4-mini";
        public int MaxOutputTokens { get; set; } = 1000;
    }

}
