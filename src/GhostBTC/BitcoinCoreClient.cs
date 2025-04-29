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
    public class BitcoinCoreClient
    {
        private readonly TorClient _torClient;
        private readonly BitcoinCoreCredentials _credentials;

        public BitcoinCoreClient(TorClient torClient, BitcoinCoreCredentials credentials)
        {
            _torClient = torClient;
            _credentials = credentials;
        }

        public async Task<IEnumerable<Coin>> GetUnspentCoinsAsync(DerivationStrategyBase derivationStrategy)
        {
            var request = new JObject
            {
                ["jsonrpc"] = "1.0",
                ["id"] = "ghostbtc",
                ["method"] = "listunspent",
                ["params"] = new JArray(0, 9999999, new JArray())
            };

            var response = await SendRequestAsync(request);
            var items = JArray.Parse(response["result"].ToString());

            return items.Select(item => new Coin(
                fromTxHash: uint256.Parse(item["txid"].ToString()),
                fromOutputIndex: (uint)item["vout"],
                amount: Money.Coins((decimal)item["amount"]),
                scriptPubKey: Script.FromHex(item["scriptPubKey"].ToString())
            ));
        }

        public async Task<string> SendRawTransactionAsync(Transaction transaction)
        {
            var request = new JObject
            {
                ["jsonrpc"] = "1.0",
                ["id"] = "ghostbtc",
                ["method"] = "sendrawtransaction",
                ["params"] = new JArray(transaction.ToHex())
            };

            var response = await SendRequestAsync(request);
            return response["result"].ToString();
        }

        private async Task<JObject> SendRequestAsync(JObject request)
        {
            var authBytes = Encoding.UTF8.GetBytes($"{_credentials.RpcUser}:{_credentials.RpcPassword}");
            var authHeader = Convert.ToBase64String(authBytes);

            var content = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
            content.Headers.Add("Authorization", $"Basic {authHeader}");

            var url = $"http://{_credentials.Host}:{_credentials.Port}";
            var response = await _torClient.PostAsync(url, content);
            var json = JObject.Parse(response);
            
            if (json["error"] != null)
                throw new Exception(json["error"].ToString());
            
            return json;
        }
    }
}