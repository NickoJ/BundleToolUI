namespace BundleToolUI.Models
{
    public readonly struct ExecuteResult
    {
        
        public readonly int ExitCode;
        public readonly string Message;
        public readonly string ErrorMessage;

        public ExecuteResult(int exitCode, string message, string errorMessage)
        {
            ExitCode = exitCode;
            Message = message;
            ErrorMessage = errorMessage;
        }
        
    }
        
}