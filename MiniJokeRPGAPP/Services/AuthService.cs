using MiniJokeRPGAPP.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace MiniJokeRPGAPP.Services
{
    public class AuthService : GeneralService
    {
        public async Task<LoginResponseDto?> Login(LoginDto dto)
        {
            var response =
                await client.PostAsJsonAsync("api/auth", dto);

            await VerificarError(response);

            var result =
                await response.Content
                .ReadFromJsonAsync<LoginResponseDto>();

            if (result != null)
            {
                await SecureStorage.SetAsync(
                    "Token",
                    result.Token);
            }

            return result;
        }

        public async Task<bool> Register(RegisterDto dto)
        {
            var response =
                await client.PostAsJsonAsync(
                    "api/auth/register",
                    dto);

            await VerificarError(response);

            return response.IsSuccessStatusCode;
        }

        public void Logout()
        {
            SecureStorage.Remove("Token");

            client.DefaultRequestHeaders.Authorization = null;
        }
    }
}
