using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace MiniJokeRPGAPP.Services
{
    public class GeneralService
    {
        public readonly HttpClient client;

        public string url = "https://localhost:7202/";

        public GeneralService()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
        }

        public async Task SetToken()
        {
            var token = await SecureStorage.GetAsync("Token");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task VerificarError(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                if (error.StartsWith("["))
                {
                    try
                    {
                        var errores =
                            System.Text.Json.JsonSerializer
                            .Deserialize<List<string>>(error);

                        if (errores != null)
                        {
                            error = string.Join("\n", errores);
                        }
                    }
                    catch { }
                }

                throw new Exception(error);
            }
        }
    }
}
