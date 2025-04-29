using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GhostBTC
{
    public class Config
    {
        public string WalletPath { get; set; }
        public string EncryptionPassword { get; set; }
        public int TorSocksPort { get; set; } = 9050;
        public bool Testnet { get; set; }
        public bool UseElectrum { get; set; }
        public string ElectrumServer { get; set; }
        public BitcoinCoreCredentials CoreRpcCredentials { get; set; }
        public bool EnableGUI { get; set; }

        public static Config Load()
        {
            var configPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".ghostbtc",
                "config.json");
            
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<Config>(json);
            }
            
            // Create default config
            var config = new Config
            {
                WalletPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".ghostbtc",
                    "wallet.dat"),
                EncryptionPassword = "",
                Testnet = true, // Default to testnet for safety
                UseElectrum = true,
                ElectrumServer = "electrumx-server.tor:50002",
                EnableGUI = false
            };
            
            Directory.CreateDirectory(Path.GetDirectoryName(configPath));
            File.WriteAllText(configPath, JsonSerializer.Serialize(config));
            
            return config;
        }
    }

    public class BitcoinCoreCredentials
    {
        public string RpcUser { get; set; }
        public string RpcPassword { get; set; }
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8332;
    }
}