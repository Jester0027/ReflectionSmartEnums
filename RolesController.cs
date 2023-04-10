using Microsoft.AspNetCore.Mvc;

namespace EnumsTest;

[ApiController]
[Route("roles")]
public class RolesController : ControllerBase
{
    private sealed record RoleResponse(string Name, string? Claim);

    /// <summary>
    /// Returns all roles and their claims
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Index()
    {
        var roles = Enum
            .GetValues<Role>()
            .Select(role => new RoleResponse(role.GetName(), role.GetClaim()))
            .ToList();

        return Ok(roles);
    }

    public sealed record RoleRequest(string Claim);

    /// <summary>
    /// Get a role from a claim
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult FromClaim([FromBody] RoleRequest request)
    {
        var role = RoleUtils.FromClaim(request.Claim);
        return Ok(new RoleResponse(role.GetName(), role.GetClaim()));
    }
}