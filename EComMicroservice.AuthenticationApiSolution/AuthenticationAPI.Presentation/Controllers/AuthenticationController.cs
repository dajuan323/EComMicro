using AuthenticationAPI.App.DTOs;
using AuthenticationAPI.App.Interfaces;
using EComMicro.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAPI.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(IUser userInterface) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<Response>> Register(AppUserDTO appUser)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await userInterface.Register(appUser);

        return result.Flag ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<Response>> Login(LoginDTO loginDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await userInterface.Login(loginDto);

        return result.Flag ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetUserDTO>> GetUser(int id)
    {
        if (id <= 0) return BadRequest("Invalid user Id");
        var user = await userInterface.GetUser(id);

        return user.Id > 0 ? Ok(user) : NotFound() ;
    }

}
