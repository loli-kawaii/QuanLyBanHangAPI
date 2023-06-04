﻿using QuanLyBanHangAPI.Models.Token;

namespace QuanLyBanHangAPI.Services.TokenServices
{
    public interface ITokenServices
    {
        bool SetTokenExprired(string token);
        TokenVM GetByToken(string token);
        void Update(TokenVM vm);
        bool IsTokenExpired(string token);
        string GenerateRefreshToken();
    }
}
