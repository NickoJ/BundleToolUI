using System;
using System.Diagnostics;
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

        public event Action<string> LogMessage;
        public event Action<string> ErrorMessage;

        public Task<ExecuteResult> ExecuteAsync()
        {
            var tcs = new TaskCompletionSource<ExecuteResult>();
            
            string command = _builder.Build("bundletool.jar", ExecuteParams);
            
            var process = CreateProcess(command);

            process.OutputDataReceived += (sender, args) => LogMessage?.Invoke(args.Data);
            process.ErrorDataReceived += (sender, args) => ErrorMessage?.Invoke(args.Data);

            process.Exited += (sender, args) =>
            {
                tcs.SetResult(new ExecuteResult(process.ExitCode));
                process.Dispose();
            };
            
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
        }

        protected abstract Process CreateProcess(string command);
        
    }
    
}