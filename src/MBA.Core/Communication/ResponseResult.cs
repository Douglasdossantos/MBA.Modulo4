namespace MBA.Core.Communication
{
    public class ResponseResult
    {
        public ResponseResult()
        {
            Title = string.Empty;
            Errors = new ResponseErrorMessages();
        }

        public string Title { get; set; }
        public int Status { get; set; }
        public ResponseErrorMessages Errors { get; set; }
    }

    public class ResponseErrorMessages
    {
        public ResponseErrorMessages()
        {
            Mensagens = [];
        }

        public List<string> Mensagens { get; set; }
    }
}