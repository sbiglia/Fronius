using Spectre.Console;

namespace FroniusDataCollector;

public static class ConsoleWriter
{
    public static void WriteLogMessage(string message)
    {
        AnsiConsole.MarkupLine($"[grey]LOG:[/] {message}[grey]...[/]");
    }

    public static void WriteErrorMessage(string message)
    {
        AnsiConsole.MarkupLine($"[grey]FATAL:[/] [red]{message}[/]");
    }

}