using GateWise.Core.DTOs;

namespace GateWise.Core.Interfaces;

public interface ISpaceAccessService
{
    Task<string> RequestSpaceAccessAsync(string userId, int spaceId, AccessLogCreateDto dto);
    Task<bool> ConfirmAccessAsync(AccessLogConfirmDto dto);
}
