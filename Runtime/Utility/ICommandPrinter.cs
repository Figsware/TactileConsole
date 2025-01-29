using Tactile.Console.Commands;
using Tactile.Console.Printing;

namespace Tactile.Console.Utility
{
    public interface ICommandPrinter
    {
        BasePrintBuilder PrintCommand(BaseCommand command, BasePrintBuilder builder);
    }
}