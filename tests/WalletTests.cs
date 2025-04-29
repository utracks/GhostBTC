using NBitcoin;
using Xunit;

namespace GhostBTC.Tests
{
    public class WalletTests
    {
        [Fact]
        public void CanCreateNewWallet()
        {
            var config = new Config { Testnet = true };
            var torClient = new TorClient();
            var wallet = new Wallet(config, torClient);
            
            var address = wallet.GetNewAddress();
            Assert.NotNull(address);
            Assert.StartsWith("tb1", address.ToString());
        }

        [Fact]
        public async Task CanGenerateValidAddresses()
        {
            var config = new Config { Testnet = true };
            var torClient = new TorClient();
            var wallet = new Wallet(config, torClient);
            
            var address1 = wallet.GetNewAddress();
            var address2 = wallet.GetNewAddress();
            
            Assert.NotEqual(address1, address2);
            Assert.True(address1.IsValid);
            Assert.True(address2.IsValid);
        }
    }
}