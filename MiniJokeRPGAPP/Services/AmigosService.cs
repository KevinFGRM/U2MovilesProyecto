using MiniJokeRPGAPP.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace MiniJokeRPGAPP.Services
{
    public class AmigosService : GeneralService
    {
        public async Task<List<AmigoResponseDTO>> GetAmigos()
        {
            await SetToken();

            var response = await client.GetAsync("api/amigos");

            await VerificarError(response);

            return await response.Content.ReadFromJsonAsync<List<AmigoResponseDTO>>() ?? [];
        }

        public async Task<bool> AgregarAmigo(AgregarAmigoDTO dto)
        {
            await SetToken();

            var response = await client.PostAsJsonAsync("api/amigos", dto);

            await VerificarError(response);

            return response.IsSuccessStatusCode;
        }
    }
}
