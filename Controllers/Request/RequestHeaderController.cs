using Commons;
using Commons.Interfaces;
using Commons.Models;
using Commons.SearchModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace support.itm.api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RequestHeaderController : ControllerBase
{
    private readonly IRequestE _requestService;

    public RequestHeaderController(IRequestE requestService)
    {
        _requestService = requestService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("Insert")]
    public async Task<IActionResult> Upload([FromForm] RequestHeaderAttach request)
    {
        await _requestService.Insert(request?.Request, request?.Attach);

        return Ok(new { Result = "Requerimiento agregado exitosamente" });
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="userCreated"></param>
    /// <param name="status"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("FindByFields")]
    [ProducesResponseType(typeof(IEnumerable<RequestEModel>), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> Find(string? UserCreated, string? Status, DateTime? StartDate, DateTime? EndDate)
    {
        IEnumerable<RequestEModel> data = null;
        int? totalItems = null;
        var pagination = BaseController.GetPagination(HttpContext);
        var modelSearch = BaseController.CreateModelSearch<RequestHeaderSearch>(new { UserCreated, Status, StartDate, EndDate });
        var isValidPagination = pagination.IsValid;

        if (!isValidPagination)
            data = await _requestService.GetRequirements(modelSearch);
        else
        {
            var requirements = await _requestService.GetRequirements(modelSearch, pagination);
            data = requirements.data;
            totalItems = requirements.totalItems;
        }

        return Ok(isValidPagination ? BaseController.CreateResponse(data, totalItems, pagination) : data);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestId"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("FindById")]
    [ProducesResponseType(typeof(RequestEModel), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> FindById([FromBody] RequestEIdSearch search)
    {
        var requirements = await _requestService.FindById(search.IdRequest);

        return Ok(requirements);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("Close")]
    public async Task<IActionResult> Close([FromBody] RequestEIdSearch search)
    {
        await _requestService.Close(search?.IdRequest);

        return Ok(new { Result = "Requerimiento cerrado exitosamente" });
    }
}
