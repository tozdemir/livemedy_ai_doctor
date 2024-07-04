namespace LiveMedyAIProject.Models
{
    public class OpenAiResponse
    {
        public Choice[] Choices { get; set; }
    }

    public class Choice
    {
        public string Text { get; set; }
    }
}
