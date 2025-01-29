using System.Linq;
using Tactile.Console.Commands;
using Tactile.Console.Parameters;
using Tactile.Console.Printing;

namespace Tactile.Console.Utility
{
    public class CommandPrinter : ICommandPrinter
    {
        public bool PrintDescription { get; set; } = true;
        public bool RecurseIntoSubcommands { get; set; } = true;
        public bool PrintHiddenCommands { get; set; } = false;
        
        public BasePrintBuilder PrintCommand(BaseCommand command, BasePrintBuilder builder)
        {
            return PrintCommandRecursive(command, builder, builder.Format);
        }

        private BasePrintBuilder PrintCommandRecursive(BaseCommand command, BasePrintBuilder pb, PrintFormat f)
        {
            if (!command.Hidden)
            {
                var namePrefix = string.Join(' ', command.GetCommandPrefix());
                if (namePrefix.Length > 0)
                {
                    namePrefix += " ";
                }
            
                pb.With(f.PrimaryColor, p => p + namePrefix + command.Name +
                                             p.With(f.SecondaryColor, p => PrintCommandArguments(p, command)));

                if (PrintDescription)
                {
                    pb.AppendString($": {command.Description}");    
                }
            }

            if (!RecurseIntoSubcommands || command is not BaseCommandGroup commandGroup) return pb;
            var commands = commandGroup.GetSubcommands();
            for (var i = 0; i < commands.Length; i++)
            {
                var subCommand = commands[i];
                if (!(i == 0 && command.Hidden))
                {
                    pb.AppendLine();
                }
                PrintCommandRecursive(subCommand, pb, f);
            }

            return pb;
        }

        private BasePrintBuilder PrintCommandArguments(BasePrintBuilder p, BaseCommand command)
        {
            if (command is not BaseCommandWithParameters { HasParameters: true } parameterCommand)
                return p;

            var parameters =
                string.Join(' ',
                    parameterCommand.Parameters.Select(pm =>
                    {
                        var name = pm.Name + (pm is RestParameter ? "..." : string.Empty);
                        return pm.IsRequired ? $"<{name}>" : $"({name})";
                    }));

            return p + " " + parameters;
        }
    }
}