using NBitcoin;
using System;
using System.Threading.Tasks;

namespace GhostBTC.UI
{
    public class ConsoleUI
    {
        private readonly Wallet _wallet;

        public ConsoleUI(Wallet wallet)
        {
            _wallet = wallet;
        }

        public async Task RunAsync()
        {
            Console.Clear();
            Console.WriteLine("GhostBTC Console Interface");
            Console.WriteLine("--------------------------");
            
            while (true)
            {
                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. Get Balance");
                Console.WriteLine("2. Generate New Address");
                Console.WriteLine("3. Send Bitcoin");
                Console.WriteLine("4. Exit");
                Console.Write("Select option: ");
                
                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        await ShowBalanceAsync();
                        break;
                    case "2":
                        await GenerateAddressAsync();
                        break;
                    case "3":
                        await SendBitcoinAsync();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid option");
                        break;
                }
            }
        }

        private async Task ShowBalanceAsync()
        {
            var balance = await _wallet.GetBalanceAsync();
            Console.WriteLine($"\nCurrent Balance: {balance.ToUnit(MoneyUnit.BTC)} BTC");
        }

        private async Task GenerateAddressAsync()
        {
            var address = _wallet.GetNewAddress();
            Console.WriteLine($"\nNew Address: {address}");
        }

        private async Task SendBitcoinAsync()
        {
            try
            {
                Console.Write("\nEnter destination address: ");
                var destAddress = Console.ReadLine();
                
                Console.Write("Enter amount (BTC): ");
                var amount = decimal.Parse(Console.ReadLine());
                
                Console.Write("Enter fee rate (sat/byte): ");
                var feeRate = new FeeRate(Money.Satoshis(int.Parse(Console.ReadLine())));
                
                var txId = await _wallet.SendToAddressAsync(
                    BitcoinAddress.Create(destAddress, _wallet.Network),
                    Money.Coins(amount),
                    feeRate);
                
                Console.WriteLine($"\nTransaction sent! TXID: {txId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}