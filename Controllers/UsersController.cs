using ApiWithRedisCache.Services;
using Microsoft.AspNetCore.Mvc;


namespace ApiWithRedisCache.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await _userService.GetUsersAsync();
        if (users == null)
        {
            return NotFound();
        }
        
        return Ok(users);
    }
}