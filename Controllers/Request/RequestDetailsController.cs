using Commons.Interfaces;
using Commons.Models;
using Commons.SearchModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace support.itm.api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RequestDetailsController : ControllerBase
{
    private readonly IRequestD _requestDService;

    public RequestDetailsController(IRequestD requestDService, IHttpContextAccessor httpContextAccessor)
    {
        _requestDService = requestDService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("Insert")]
    public async Task<IActionResult> Upload([FromForm] RequestDetailsAttach request)
    {
        await _requestDService.Insert(request?.Request, request?.Attach);

        return Ok(new { Result = "Detalle - respuesta requerimiento agregado exitosamente" });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestId"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("FindByRequest")]
    [ProducesResponseType(typeof(IEnumerable<RequestDModel>), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> FindByRequest([FromBody] RequestEIdSearch search)
    {
        var requirements = await _requestDService.FindByRequest(search.IdRequest);

        return Ok(requirements);
    }


}
