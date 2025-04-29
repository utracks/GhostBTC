using System;
using GhostBTC;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Initialize configuration
            var config = Config.Load();
            
            // Set up Tor client
            var torClient = new TorClient(config.TorSocksPort);
            
            // Initialize wallet
            var wallet = new Wallet(config, torClient);
            
            // Start UI
            if (args.Contains("--gui") && config.EnableGUI)
            {
                var gui = new GtkUI(wallet);
                gui.Run();
            }
            else
            {
                var consoleUi = new ConsoleUI(wallet);
                consoleUi.Run();
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Fatal error: {ex.Message}");
            Environment.Exit(1);
        }
    }
}