using Microsoft.Extensions.Configuration;
using MiniJokeRPGAPP.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace MiniJokeRPGAPP.Services
{
    public class MensajesService : GeneralService
    {
        public async Task<bool> MandarMensaje( MandarMensajeDTO dto)
        {
            await SetToken();

            var response = await client.PostAsJsonAsync("api/mensajes", dto);

            await VerificarError(response);

            return response.IsSuccessStatusCode;
        }

        public async Task<ListaMensajesDTO> ObtenerChat(int idUsuario)
        {
            await SetToken();

            var response = await client.GetAsync($"api/mensajes/{idUsuario}");

            await VerificarError(response);

            return await response.Content.ReadFromJsonAsync<ListaMensajesDTO>() ?? new ListaMensajesDTO();
        }

        internal async Task<bool> EnviarImagen(EnviarImagenDTO dto)
        {
            await SetToken();

            var response = await client.PostAsJsonAsync("api/mensajes/imagen", dto);

            await VerificarError(response);

            return response.IsSuccessStatusCode;
        }
    }
}
