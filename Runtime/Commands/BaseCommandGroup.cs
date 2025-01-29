using System.Collections.Generic;
using System.Linq;

namespace Tactile.Console.Commands
{
    public abstract class BaseCommandGroup : BaseCommand
    {
        protected readonly Dictionary<string, BaseCommand> Subcommands = new();
        public override bool Hidden => true;
        private readonly HelpCommand _groupHelpCommand;
        
        public BaseCommandGroup(string name, params BaseCommand[] subcommands) : base(name, $"Shows help for the {name} command.")
        {
            foreach (var subcommand in subcommands)
            {
                subcommand.ParentCommand = this;
                Subcommands[subcommand.Name] = subcommand;
            }

            _groupHelpCommand = new HelpCommand(subcommands);
        }

        public BaseCommandGroup(string name, string description, params BaseCommand[] subcommands) : base(name, description)
        {
            foreach (var subcommand in subcommands)
            {
                subcommand.ParentCommand = this;
                Subcommands[subcommand.Name] = subcommand;
            }
            
            _groupHelpCommand = new HelpCommand(subcommands);
        }

        public BaseCommand[] GetSubcommands() => Subcommands.Values.OrderBy(c => c.Name).ToArray();

        protected virtual void Execute(Console console)
        {
            _groupHelpCommand.Execute(console, string.Empty);
        }

        public override void Execute(Console console, string command)
        {
            var nameAndBody = ParseCommand(command);
            if (nameAndBody == null)
            {
                Execute(console);
                return;
            }

            var (name, body) = nameAndBody.Value;
            if (Subcommands.TryGetValue(name, out var foundSubCommand))
            {
                foundSubCommand.Execute(console, body);    
            }
            else
            {
                console.PrintError($"Unknown subcommand: {name}");
            }
        }
    }
}