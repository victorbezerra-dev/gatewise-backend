using GateWise.Core.DTOs;
using GateWise.Core.Entities;

namespace GateWise.Core.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(string id, UserUpsertDto user);
    Task UpdatePublicKeyAsync(string id, string newPublicKeyPem);
    Task DeleteAsync(string id);
    Task<User?> GetByEmailOrRegistrationAsync(string email, string registrationNumber);

}
