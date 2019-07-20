using System;

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

        protected abstract ExecuteResult Execute(string command);

    }
    
}