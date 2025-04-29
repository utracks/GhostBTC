using NBitcoin;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GhostBTC
{
    public class KeyStore
    {
        private readonly string _walletPath;
        private readonly byte[] _encryptionKey;
        private readonly ExtKey _masterKey;
        
        public KeyStore(string walletPath, string password)
        {
            _walletPath = walletPath;
            _encryptionKey = DeriveEncryptionKey(password);
            
            if (File.Exists(walletPath))
            {
                _masterKey = LoadWallet();
            }
            else
            {
                _masterKey = CreateNewWallet();
                SaveWallet();
            }
        }

        private ExtKey CreateNewWallet()
        {
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            return mnemonic.DeriveExtKey();
        }

        private ExtKey LoadWallet()
        {
            var encryptedData = File.ReadAllBytes(_walletPath);
            var decrypted = Decrypt(encryptedData);
            return ExtKey.Parse(decrypted);
        }

        public void SaveWallet()
        {
            var encrypted = Encrypt(_masterKey.ToString(_network));
            File.WriteAllBytes(_walletPath, encrypted);
        }

        private byte[] DeriveEncryptionKey(string password)
        {
            using var derive = new Rfc2898DeriveBytes(
                password, 
                salt: Encoding.UTF8.GetBytes("GhostBTC-Salt"), 
                iterations: 100000);
            return derive.GetBytes(32); // 256-bit key
        }

        private byte[] Encrypt(string data)
        {
            using var aes = Aes.Create();
            aes.Key = _encryptionKey;
            
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(Encoding.UTF8.GetBytes(data));
            cs.FlushFinalBlock();
            return ms.ToArray();
        }

        private string Decrypt(byte[] data)
        {
            using var aes = Aes.Create();
            aes.Key = _encryptionKey;
            
            using var ms = new MemoryStream(data);
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}