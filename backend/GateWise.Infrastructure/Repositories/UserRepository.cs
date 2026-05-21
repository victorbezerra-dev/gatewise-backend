using GateWise.Core.DTOs;
using GateWise.Core.Entities;
using GateWise.Core.Interfaces;
using GateWise.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GateWise.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync() =>
        await _context.Users.ToListAsync();

    public async Task<User?> GetByIdAsync(string id) =>
        await _context.Users.FindAsync(id);

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
    public async Task UpdateAsync(string id, UserUpsertDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new Exception("User not found");

        if (!string.IsNullOrWhiteSpace(dto.Name))
            user.Name = dto.Name;

        if (!string.IsNullOrWhiteSpace(dto.Email))
            user.Email = dto.Email;

        if (dto.RegistrationNumber is not null)
            user.RegistrationNumber = dto.RegistrationNumber;

        if (!string.IsNullOrWhiteSpace(dto.UserAvatarUrl))
            user.UserAvatarUrl = dto.UserAvatarUrl;

        user.UserType = dto.UserType;

        if (!string.IsNullOrWhiteSpace(dto.OperationalSystem))
            user.OperationalSystem = dto.OperationalSystem;

        if (!string.IsNullOrWhiteSpace(dto.OperationalSystemVersion))
            user.OperationalSystemVersion = dto.OperationalSystemVersion;

        if (!string.IsNullOrWhiteSpace(dto.DeviceModel))
            user.DeviceModel = dto.DeviceModel;

        if (!string.IsNullOrWhiteSpace(dto.DeviceManufactureName))
            user.DeviceManufactureName = dto.DeviceManufactureName;

        if (!string.IsNullOrWhiteSpace(dto.DevicePublicKeyPem))
            user.DevicePublicKeyPem = dto.DevicePublicKeyPem;

        await _context.SaveChangesAsync();
    }

    public async Task UpdatePublicKeyAsync(string id, string newPublicKeyPem)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new Exception("User not found");

        user.DevicePublicKeyPem = newPublicKeyPem;
        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<User?> GetByEmailOrRegistrationAsync(string email, string registrationNumber)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email || u.RegistrationNumber == registrationNumber);
    }

}
