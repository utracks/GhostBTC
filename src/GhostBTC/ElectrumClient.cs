using NBitcoin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GhostBTC
{
    public class ElectrumClient
    {
        private readonly TorClient _torClient;
        private readonly string _serverUrl;
        private static int _requestId;

        public ElectrumClient(TorClient torClient, string serverUrl)
        {
            _torClient = torClient;
            _serverUrl = serverUrl;
        }

        public async Task<IEnumerable<Coin>> GetUnspentCoinsAsync(DerivationStrategyBase derivationStrategy)
        {
            var scriptHashes = new List<string>();
            // Generate script hashes for first 20 addresses
            for (int i = 0; i < 20; i++)
            {
                var address = derivationStrategy.Derive(new KeyPath($"0/{i}")).ScriptPubKey;
                scriptHashes.Add(address.Hash.ToString());
            }

            var utxos = new List<Coin>();
            foreach (var scriptHash in scriptHashes)
            {
                var response = await SendRequestAsync("blockchain.scripthash.listunspent", scriptHash);
                var items = JArray.Parse(response);
                
                foreach (var item in items)
                {
                    var txId = uint256.Parse(item["tx_hash"].ToString());
                    var index = (uint)item["tx_pos"];
                    var amount = Money.Coins((decimal)item["value"]);
                    var scriptPubKey = derivationStrategy.Derive(new KeyPath($"0/{scriptHashes.IndexOf(scriptHash)}")).ScriptPubKey;
                    
                    utxos.Add(new Coin(txId, index, amount, scriptPubKey));
                }
            }

            return utxos;
        }

        public async Task<string> BroadcastTransactionAsync(Transaction transaction)
        {
            var hex = transaction.ToHex();
            return await SendRequestAsync("blockchain.transaction.broadcast", hex);
        }

        private async Task<string> SendRequestAsync(string method, params object[] parameters)
        {
            var request = new JObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = ++_requestId,
                ["method"] = method,
                ["params"] = new JArray(parameters)
            };

            var content = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
            var response = await _torClient.PostAsync(_serverUrl, content);
            var json = JObject.Parse(response);
            
            if (json["error"] != null)
                throw new Exception(json["error"].ToString());
            
            return json["result"].ToString();
        }
    }
}