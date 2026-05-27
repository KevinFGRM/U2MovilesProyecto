using MiniJokeRPGAPP.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace MiniJokeRPGAPP.Services
{
    public class PartidasService : GeneralService
    {
        public async Task<bool> CrearPartida(CrearPartidaDto dto)
        {
            await SetToken();

            var response = await client.PostAsJsonAsync("api/partidas", dto);

            await VerificarError(response);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SeleccionarPersonaje(SeleccionarPersonajeDto dto)
        {
            await SetToken();

            var response = await client.PostAsJsonAsync($"api/partidas/personaje", dto);

            await VerificarError(response);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<PartidaResponseDTO>> GetPartidas()
        {
            await SetToken();

            var response = await client.GetAsync("api/partidas");

            await VerificarError(response);

            return await response.Content.ReadFromJsonAsync<List<PartidaResponseDTO>>() ?? [];
        }

        public async Task<EntrarPartidaResult?> EntrarPartida(int idPartida)
        {
            await SetToken();

            var response = await client.GetAsync($"api/partidas/{idPartida}");

            await VerificarError(response);

            return await response.Content.ReadFromJsonAsync<EntrarPartidaResult>();
        }

        public async Task<bool> RealizarAccion( int idPartida, RealizarAccionDto dto)
        {
            await SetToken();

            var response = await client.PostAsJsonAsync($"api/partidas/{idPartida}/accion", dto);

            await VerificarError(response);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<AccionResponseDTO>> CargarAcciones(int idPartida)
        {
            await SetToken();

            var response = await client.GetAsync($"api/partidas/{idPartida}/acciones");

            await VerificarError(response);

            return await response.Content.ReadFromJsonAsync<List<AccionResponseDTO>>() ?? [];
        }

        public async Task<EstadoPartidaDto?> ObtenerEstado(int idPartida)
        {
            await SetToken();

            var response = await client.GetAsync($"api/partidas/{idPartida}/estado");

            await VerificarError(response);

            return await response.Content.ReadFromJsonAsync<EstadoPartidaDto>();
        }
    }
}
