namespace WarehouseManagement.Application.Comom
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? Email { get; }
        string? Roles { get; }
    }
}
