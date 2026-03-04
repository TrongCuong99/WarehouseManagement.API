namespace WarehouseManagement.Application.Comom
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Email { get; }
        string? Roles { get; }
    }
}
