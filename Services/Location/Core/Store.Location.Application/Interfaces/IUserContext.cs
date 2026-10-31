namespace Store.Location.Application.Interfaces
{
    /// <summary>
    /// JWT token içinden kimliği doğrulanmış istemcinin client_id değerini Guid olarak döner.
    /// </summary>
    public interface IUserContext
    {
        Guid GetClientId();
    }
}
