namespace PMS.Application.Features.Identity.Users;

public interface IUserService
{
    Task<string> CreateAsync(CreateUserRequest createUserRequest);
    Task<string> UpdateAsync(UpdateUserRequest updateUserRequest);
    Task<string> DeleteAsync(string id);
    Task<string> ActivateOrDeactivateAsync(ChangeUserStatusRequest changeUserStatusRequest);
    Task<string> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest);
    Task<string> AssignRolesAsync(string userId, UserRolesRequest userRolesRequest);
    Task<List<UserResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<UserResponse> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<List<UserRoleResponse>> GetUserRolesAsync(string id, CancellationToken cancellationToken);
    Task<bool> IsEmailTakenAsync(string email);
    Task<List<string>> GetUserPermissionsAsync(string id, CancellationToken cancellationToken);
    Task<bool> IsPermissionAssignedAsync(string id, string permission, CancellationToken cancellationToken);
}
