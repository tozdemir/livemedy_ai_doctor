namespace LiveMedyAIProject.Models
{
    public class OpenAiChatResponse
    {
        public Choice[] Choices { get; set; }
        public OpenAiError Error { get; set; } 

        public class Choice
        {
            public Message Message { get; set; }
        }

        public class Message
        {
            public string Role { get; set; }
            public string Content { get; set; }
        }

        public class OpenAiError
        {
            public string Message { get; set; }
            public string Type { get; set; }
            public string Param { get; set; }
            public string Code { get; set; }
        }
    }
}