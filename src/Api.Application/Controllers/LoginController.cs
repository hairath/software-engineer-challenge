using Api.Application.Domain.Dtos.Login;
using API.PP.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace API.PP.Controllers
{
    /// <summary>
    /// Controlador de autenticação de usuários na API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        /// <summary>
        /// Método responsável por autenticar usuários.
        /// </summary>
        /// /// <param name="user"></param>
        /// <returns>retorna uma lista de usuários</returns>
        [AllowAnonymous]
        [HttpPost]
        public object Login([FromBody] LoginDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = _loginService.Authenticate(user);

                if (result == null)
                    return NotFound(result);

                return Ok(result);

            }
            catch (ArgumentException ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
