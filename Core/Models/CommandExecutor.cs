using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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
        
        public async ValueTask<ExecuteResult> ExecuteAsync()
        {
            string command = _builder.Build("bundletool.jar", ExecuteParams);
            return await Task.Run(() => Execute(command));
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