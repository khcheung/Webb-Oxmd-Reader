using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace WebbSite.Common.Logic;

public enum StateEnum
{
    MainMenu,
    LoadDBRecord
}

public class ConsoleService(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    private StateEnum State { get; set; } = StateEnum.MainMenu;
    
    public async Task StartAsync()
    {
        Spectre.Console.AnsiConsole.MarkupLine("[green]Welcome to the WebbSite Console Application![/]");

        while (true)
        {
            await ProcessState();
        }
    }

    private async Task ProcessState()
    {
        switch (State)
        {
            case StateEnum.MainMenu:
                // Handle main menu logic
                await ProcessMainMenu();
                break;
            case StateEnum.LoadDBRecord:
                // Handle loading DB record logic
                await ProcessLoadDBRecord();
                break;
            default:
                break;
        }
    }

    private async Task ProcessMainMenu()
    {
        var mainMenu = AnsiConsole.Prompt<String>(new SelectionPrompt<String>()
        .Title("Please select an option:")
        .AddChoices("Load DB Record", "Exit"));

        AnsiConsole.MarkupLine($"You selected: [blue]{mainMenu}[/]");

        switch (mainMenu)
        {
            case "Load DB Record":
                State = StateEnum.LoadDBRecord;
                break;
            case "Exit":
                AnsiConsole.MarkupLine("[yellow]Exiting the application. Goodbye![/]");
                Environment.Exit(0);
                break;
            default:
                break;
        }
    }

    private async Task ProcessLoadDBRecord()
    {
        var stockCodeInput = AnsiConsole.Ask<string>("Enter a [blue]stock code[/] to load from DB:");

        if (!int.TryParse(stockCodeInput, out int stockCode))
        {
            Spectre.Console.AnsiConsole.MarkupLine("[red]Invalid stock code. Please enter a valid integer.[/]");
            return;
        }

        // Here you can call your logic to load the record from the database for the given stock code.
        // For example:
        var dbService = serviceProvider.GetRequiredService<DbService>();
        var records = await dbService.GetCCASSRecordsAsync(stockCode);

        if (records == null || !records.Any())
        {
            AnsiConsole.MarkupLine($"[yellow]No records found for stock code {stockCode}.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[green]Records for stock code {stockCode}:[/]");

            var table = new Table();
            table.AddColumn("Row");
            table.AddColumn("Date");
            table.AddColumn("Top 5");
            table.AddColumn("Top 10");
            table.AddColumn("Top 10 + NCIP");
            table.AddColumn("Stake");

            var rowCount = 0;

            foreach (var record in records)
            {
                table.AddRow(
                    (++rowCount).ToString(),
                    record.Date.ToShortDateString(),
                    record.Top5.ToString(),
                    record.Top10.ToString(),
                    record.Top10NCIP.ToString(),
                    record.Stake.ToString());
            }

            AnsiConsole.Write(table);
        }

        var stockMenu = AnsiConsole.Prompt<String>(new SelectionPrompt<String>()
        .Title("Please select an action:")
        .AddChoices("Get Update From 0xmd", "Read Other Stock Code", "Back to Main Menu"));

        switch (stockMenu)
        {
            case "Get Update From 0xmd":
                var client = serviceProvider.GetRequiredService<WebbSiteClient>();
                var newRecords = await client.GetCCASSAsync(stockCode);
                if (newRecords == null || !newRecords.Any())
                {
                    AnsiConsole.MarkupLine($"[yellow]No new records found for stock code {stockCode} from 0xmd.[/]");
                    return;
                }
                await dbService.SaveCCASSRecordsAsync(stockCode, newRecords);
                AnsiConsole.MarkupLine($"[green]Updated records for stock code {stockCode} from 0xmd.[/]");

                
                break;
            case "Read Other Stock Code":
                State = StateEnum.LoadDBRecord;
                break;
            case "Back to Main Menu":
                State = StateEnum.MainMenu;
                break;
            default:
                break;
        }
    }
}