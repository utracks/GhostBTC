using NBitcoin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostBTC
{
    public class Wallet
    {
        private readonly Config _config;
        private readonly TorClient _torClient;
        private readonly KeyStore _keyStore;
        private readonly Network _network;
        
        public Wallet(Config config, TorClient torClient)
        {
            _config = config;
            _torClient = torClient;
            _network = config.Testnet ? Network.TestNet : Network.Main;
            _keyStore = new KeyStore(config.WalletPath, config.EncryptionPassword);
        }

        public async Task<Money> GetBalanceAsync()
        {
            var utxos = await GetUnspentCoinsAsync();
            return utxos.Sum(u => u.Amount);
        }

        public async Task<string> GenerateNewAddressAsync()
        {
            var keyPath = new KeyPath(_keyStore.GetNextAddressIndex());
            var address = _keyStore.DeriveAddress(keyPath);
            return address.ToString();
        }

        public async Task<string> SendBitcoinAsync(
            string destination, 
            Money amount, 
            FeeRate feeRate,
            IEnumerable<OutPoint>? selectedCoins = null)
        {
            var builder = _network.CreateTransactionBuilder();
            var coins = selectedCoins != null 
                ? await GetUnspentCoinsAsync(selectedCoins)
                : await GetUnspentCoinsAsync();
                
            var tx = builder
                .AddCoins(coins)
                .AddKeys(_keyStore.GetPrivateKeys())
                .Send(destination, amount)
                .SendFees(feeRate)
                .SetChange(await GenerateNewAddressAsync())
                .BuildTransaction(true);
                
            return await BroadcastTransactionAsync(tx);
        }

        private async Task<string> BroadcastTransactionAsync(Transaction tx)
        {
            if (_config.UseElectrum)
            {
                var electrum = new ElectrumClient(_torClient, _config.ElectrumServer);
                return await electrum.BroadcastTransactionAsync(tx);
            }
            else
            {
                var core = new BitcoinCoreClient(_torClient, _config.CoreRpcCredentials);
                return await core.SendRawTransactionAsync(tx);
            }
        }
    }
}