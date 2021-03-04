using Api.Application.Domain.Dtos.User;
using API.PP.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace Api.Application.Controllers
{
    /// <summary>
    /// Controlador de query dos usuários do sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Método responsável por buscar usuarios ordenados por prioridade e filtrar pelo nome e username a partir de uma palavra chave.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Authorize("Bearer")]
        [HttpGet]
        public IActionResult Get([FromQuery] UserDTOFilter filter)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = _userService.Get(filter);
                var msg = result != null && result.Count > 0 ? "Success." : "No Found";

                return Ok(new JsonResult(new
                {
                    users = result,
                    message = msg,
                    page = filter.PageNumber,
                    totalRegisters = result.Count
                }));
            }
            catch (ArgumentException ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
