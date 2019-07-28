namespace BundleToolUI.Models
{
    public readonly struct ExecuteResult
    {
        
        public readonly int ExitCode;

        public ExecuteResult(int exitCode)
        {
            ExitCode = exitCode;
        }
        
    }
        
}