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

        public Task<ExecuteResult> ExecuteAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<ExecuteResult>();
            
            string command = _builder.Build("bundletool.jar", ExecuteParams);
            
            var process = CreateProcess(command);

            process.OutputDataReceived += (sender, args) => LogMessage?.Invoke(args.Data);
            process.ErrorDataReceived += (sender, args) => LogMessage?.Invoke(args.Data);

            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) =>
            {
                var exitCode = process.ExitCode;
                process.Dispose();
                taskCompletionSource.SetResult(new ExecuteResult(exitCode));
            };
            
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return taskCompletionSource.Task;
        }

        protected abstract Process CreateProcess(string command);
        
    }
    
}