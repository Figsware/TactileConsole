using Tactile.Console.Utility;

namespace Tactile.Console.Commands
{
    [GlobalCommand]
    public class HelpCommand : BaseCommandWithParameters
    {
        private readonly BaseCommand[] _subcommands;
        private readonly ICommandPrinter _printer;

        private const string CommandName = "help";
        private const string CommandDescription = "Shows a list of all available commands.";

        public HelpCommand() : base(CommandName, CommandDescription)
        {
            _printer = new CommandPrinter();
            _subcommands = null;
        }

        public HelpCommand(BaseCommand[] subcommands) : base(CommandName, CommandDescription)
        {
            _printer = new CommandPrinter();
            _subcommands = subcommands;
        }

        protected override void Execute(Console console, ParsedArguments arguments)
        {
            var commands = _subcommands ?? console.GetCommands();
            foreach (var command in commands)
            {
                console.Print((p, _) => _printer.PrintCommand(command, p));                    
            }
        }
    }
}