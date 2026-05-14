using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace MiniJokeRPGAPP.Services
{
    public class GeneralService
    {
        protected readonly HttpClient client;

        protected string baseUrl = "https://TUAPI.com/";

        public GeneralService()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        protected async Task SetToken()
        {
            var token = await SecureStorage.GetAsync("Token");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        protected async Task VerificarError(HttpResponseMessage response)
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
