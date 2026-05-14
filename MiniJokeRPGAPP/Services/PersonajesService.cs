using MiniJokeRPGAPP.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace MiniJokeRPGAPP.Services
{
    public class PersonajesService : GeneralService
    {
        public async Task<List<PersonajeResponseDto>> ObtenerPersonajes()
        {
            await SetToken();

            var response = await client.GetAsync("api/partidas/personajes");

            await VerificarError(response);

            return await response.Content.ReadFromJsonAsync<List<PersonajeResponseDto>>() ?? [];
        }

        public async Task<List<HabilidadResponseDto>> GetHabilidades(int idPersonaje)
        {
            await SetToken();

            var response = await client.GetAsync($"api/partidas/personajes/{idPersonaje}/habilidades");

            await VerificarError(response);

            return await response.Content.ReadFromJsonAsync<List<HabilidadResponseDto>>() ?? [];
        }
    }
}
