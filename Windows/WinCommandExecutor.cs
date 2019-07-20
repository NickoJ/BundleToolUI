using BundleToolUI.Models;

namespace BundleToolUI.Win
{
    public class WinCommandExecutor : CommandExecutor
    {
        
        public WinCommandExecutor(CommandBuilder builder) : base(builder) {}

        protected override ExecuteResult Execute(string command)
        {
            throw new System.NotImplementedException();
        }
        
    }
}