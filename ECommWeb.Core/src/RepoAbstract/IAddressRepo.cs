using ECommWeb.Core.src.Entity;
using ECommWeb.Core.src.Common;

namespace ECommWeb.Core.src.RepoAbstract;

public interface IAddressRepo
{
    Task<Address> GetAddressByIdAsync(Guid id);
    Task<IEnumerable<Address>> GetAddressesByUserAsync(Guid userId, QueryOptions? options);
    Task<Address> UpdateAddressByIdAsync(Address address);
    Task<Guid> DeleteAddressByIdAsync(Guid id);
    Task<Address> CreateAddressAsync(Address address);
    Task<Address> SetDefaultAddressAsync(Guid userId, Guid addressId);
    Task<Address> GetDefaultAddressAsync(Guid userId);
}
