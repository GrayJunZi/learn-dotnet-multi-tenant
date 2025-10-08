namespace PMS.Application.Features.Identity.Roles;

public interface IRoleService
{
    Task<string> CreateAsync(CreateRoleRequest createRoleRequest);

    Task<string> UpdateAsync(UpdateRoleRequest updateRoleRequest);

    Task<string> DeleteAsync(string id);

    Task<bool> IsExistsAsync(string name);

    Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest updateRolePermissionsRequest);

    Task<List<RoleResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<RoleResponse> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<RoleResponse> GetRoleWithPermissionsAsync(string id, CancellationToken cancellationToken);
}
