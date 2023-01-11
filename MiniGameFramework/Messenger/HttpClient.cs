using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;

using System.Text.Json;

namespace MiniGameFramework
{
    public class MGHttpClient : IHttpClient
    {
        protected HttpClient _client;

        public MGHttpClient()
        {
            _client = new HttpClient();
        }

        public async Task<string> GetStringAsync(string uri)
        {
            HttpResponseMessage res = await _client.GetAsync(uri);
            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadAsStringAsync();
            }
            else
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Http Get [{uri}] failed with status code [{res.StatusCode}]");
            }
            return "";
        }
        public async Task<T> GetJsonAsync<T>(string uri) where T : class
        {
            HttpResponseMessage res = await _client.GetAsync(uri);
            if (res.IsSuccessStatusCode)
            {
                string jsonStr = await res.Content.ReadAsStringAsync();

                //return JsonSerializer.Deserialize<T>(jsonStr);
                return JsonUtil.FromJson<T>(jsonStr);
            }
            else
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Http Get [{uri}] failed with status code [{res.StatusCode}]");
            }
            return null;
        }

        public async Task<string> PostStringAsync(string uri, string content)
        {
            var data = new StringContent(content, Encoding.UTF8, "text/plain");
            HttpResponseMessage res = await _client.PostAsync(uri, data);
            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadAsStringAsync();
            }
            else
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Http Post [{uri}] failed with status code [{res.StatusCode}]");
            }
            return "";

        }
        public async Task<T> PostJsonAsync<T, U>(string uri, U content) where T : class
        {
            var json = JsonSerializer.Serialize(content);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage res = await _client.PostAsync(uri, data);
            if (res.IsSuccessStatusCode)
            {
                string jsonStr = await res.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<T>(jsonStr);
            }
            else
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Http Post [{uri}] failed with status code [{res.StatusCode}]");
            }

            return null;
        }
        public async Task<string> PostJsonStringAsync(string uri, string content)
        {
            var data = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage res = await _client.PostAsync(uri, data);
            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadAsStringAsync();
            }
            else
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Http Post [{uri}] failed with status code [{res.StatusCode}]");
            }
            return "";
        }
    }
}
