using Commons.Interfaces;
using commons.Jwt;
using Commons.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace support.itm.api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SecurityController : ControllerBase
{

    
    private readonly ISecurity _security;
    protected SecurityContext SecurityContext { get; private set; }

    public SecurityController(ISecurity security, IHttpContextAccessor httpContextAccessor)
    {
        _security = security;

        SecurityContext = ISecurityContext.GetUserData(httpContextAccessor.HttpContext, showExceptions: false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userData"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("Authenticate")]
    public async Task<IActionResult> GenerateJwt([FromBody] AuthenticationModel userData)
    {
        var authData = await _security.GetAuthentication(userData);

        if (authData.HasError) return BadRequest(new { message = authData.MessageError });


        var context = new SecurityContext(userData.UserName, userData.Password, authData.userData.IS_ADMIN);


        return Ok(new { Token = JwtManager.GenerateToken(context, 30) });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="changePass"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel changePass)
    {
        changePass.UserName = SecurityContext.User;
        await _security.ChangePassword(changePass);
        return Ok(new { Response = "Cambio de contraseña realizado exitosamente" });
    }
}
