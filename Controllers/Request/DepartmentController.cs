using Commons.Interfaces;
using Commons.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace support.itm.api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DepartmentController : ControllerBase
{
    private readonly IDepartment _departmentService;
    public DepartmentController(IDepartment departmentService)
    {
        _departmentService = departmentService;
    }


    [HttpGet]
    [Route("FindAll")]
    [ProducesResponseType(typeof(IEnumerable<DepartmentModel>), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var departments = await _departmentService.FindAll();
        return Ok(departments);

    }
}
