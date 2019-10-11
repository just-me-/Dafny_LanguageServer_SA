using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DafnyLanguageServerCore
{
    internal class NuGetAutoCompleteService : IDisposable
    {
        private HttpClient _client = new HttpClient();
        static List<string> fakeResponse = new List<string> {
            "Cake", "Cake.Mix", "Cake.Npm", "Cake.Git", "Cake.IIS", "Cake.Tfs", "Cake.Watch", "Cake.Tfx", "Cake.Gem", "Cake.Ftp", "Cake.CakeMail",
            "Cake.Utility", "Cake.Hg", "Cake.Npx", "Cake.CakeBoss", "Cake.Svn", "Cake.CK.Pack", "Aero.Cake", "Cake.Unicorn", "CakeApp"};

        public async Task<IReadOnlyCollection<string>> GetPackages(string query)
        {
            var response = await _client.GetStringAsync($"https://api-v2v3search-0.nuget.org/autocomplete?q={query}");
            return JObject.Parse(response)["data"].ToObject<List<string>>();
            //return await Task.Run(() => fakeResponse);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task<IReadOnlyCollection<string>> GetPackageVersions(string package, string version)
        {
            var response = await _client.GetStringAsync($"https://api-v2v3search-0.nuget.org/autocomplete?id={package}");
            return JObject.Parse(response)["data"].ToObject<List<string>>();
            //return await Task.Run(() => fakeResponse);
        }
    }
}