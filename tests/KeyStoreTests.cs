using NBitcoin;
using System.IO;
using Xunit;

namespace GhostBTC.Tests
{
    public class KeyStoreTests
    {
        [Fact]
        public void CanCreateAndLoadWallet()
        {
            var walletPath = Path.GetTempFileName();
            var password = "testpassword";
            var network = Network.TestNet;
            
            // Create new wallet
            var keyStore1 = new KeyStore(walletPath, password, network);
            var address1 = keyStore1.DeriveNewAddress();
            
            // Load existing wallet
            var keyStore2 = new KeyStore(walletPath, password, network);
            var address2 = keyStore2.DeriveNewAddress();
            
            Assert.Equal(address1.ToString(), address2.ToString());
            File.Delete(walletPath);
        }

        [Fact]
        public void FailsWithWrongPassword()
        {
            var walletPath = Path.GetTempFileName();
            var password = "correctpassword";
            var network = Network.TestNet;
            
            new KeyStore(walletPath, password, network);
            
            Assert.Throws<System.Security.Cryptography.CryptographicException>(() => 
                new KeyStore(walletPath, "wrongpassword", network));
            
            File.Delete(walletPath);
        }
    }
}