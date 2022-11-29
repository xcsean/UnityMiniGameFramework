using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IHttpClient
    {
        Task<string> GetStringAsync(string uri);
        Task<T> GetJsonAsync<T>(string uri) where T : class;

        Task<string> PostStringAsync(string uri, string content);
        Task<T> PostJsonAsync<T, U>(string uri, U content) where T : class;

        Task<string> PostJsonStringAsync(string uri, string content);
    }
}
