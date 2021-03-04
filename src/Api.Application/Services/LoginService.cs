using Api.Application.Domain.Dtos.Login;
using Api.Application.Domain.Security;
using API.PP.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace Api.Application.Services
{
    public class LoginService : ILoginService
    {
        private IRepository _repository;
        private IConfiguration _configuration { get; }
        public SigningConfigurations _signingConfigurations;
        public TokenConfigurations _tokeConfigurations;

        public LoginService(IRepository repository,
                            SigningConfigurations signingConfigurations,
                            TokenConfigurations tokenConfigurations,
                            IConfiguration configuration)
        {
            _repository = repository;
            _signingConfigurations = signingConfigurations;
            _tokeConfigurations = tokenConfigurations;
            _configuration = configuration;
        }
        public object Authenticate(LoginDto user)
        {
            if (user == null && (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password)))
            {
                return new
                {
                    authenticated = false,
                    message = "falha ao autenticar"
                };
            }

            if (_repository.FindByLogin(user))
            {
                ClaimsIdentity identity = new ClaimsIdentity(
                    new GenericIdentity(user.UserName),
                    new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Aud, _configuration["TokenConfigurations:Audience"]),
                        new Claim(JwtRegisteredClaimNames.Iss, _configuration["TokenConfigurations:Issuer"])
                    });

                var createDate = DateTime.UtcNow;
                var expirationDate = DateTime.UtcNow.AddMinutes(5);

                var handler = new JwtSecurityTokenHandler();

                string token = CreateToken(identity, createDate, expirationDate, handler);

                return RetornarAutenticacao(createDate, expirationDate, token, user);
            }

            return new
            {
                authenticated = false,
                message = "falha ao autenticar"
            };
        }

        private string CreateToken(ClaimsIdentity identity, DateTime createDate, DateTime expirationDate, JwtSecurityTokenHandler handler)
        {
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _configuration["TokenConfigurations:Issuer"],
                Audience = _configuration["TokenConfigurations:Audience"],
                SigningCredentials = _signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = createDate,
                Expires = expirationDate
            });

            return handler.WriteToken(securityToken);
        }

        private object RetornarAutenticacao(DateTime createDate, DateTime expirationDate, string token, LoginDto dto)
        {
            return new
            {
                authenticated = true,
                created = createDate.ToString("yyyy-MM-dd HH:mm:ss"),
                expiration = expirationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                accessToken = token,
                userName = dto.UserName,
                message = "Usuário logado com sucesso"
            };
        }
    }
}
