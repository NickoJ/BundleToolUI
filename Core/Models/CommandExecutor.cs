using System.Diagnostics;

namespace BundleToolUI.Models
{
    
    public abstract class CommandExecutor
    {

        private readonly CommandBuilder _builder;
        
        public CommandExecutor(CommandBuilder builder)
        {
            _builder = builder;
        }

        public ExecuteParams ExecuteParams { get; } = new ExecuteParams();
        
        public ExecuteResult Execute()
        {
            string command = _builder.Build("bundletool.jar", ExecuteParams);
            return Execute(command);
        }

        protected abstract Process CreateProcess(string command);
        
        private ExecuteResult Execute(string command)
        {
            var process = CreateProcess(command);
            
            process.Start();
            string message = process.StandardOutput.ReadToEnd();
            string errorMessage = process.StandardError.ReadToEnd();
            process.WaitForExit();
            int exitCode = process.ExitCode;

            return new ExecuteResult(exitCode, message, errorMessage);
        }

    }
    
}