﻿using Taxify.Domain.Configuration;
using Taxify.Service.DTOs.Users;

namespace Taxify.Service.Interfaces;

public interface IUserService
{
    ValueTask<UserResultDto> AddAsync(UserCreationDto dto);
    ValueTask<UserResultDto> ModifyAsync(UserUpdateDto dto);
    ValueTask<bool> RemoveAsync(long id);
    ValueTask<bool> DestroyAsync(long id);
    ValueTask<UserResultDto> RetrieveByIdAsync(long id);
    IEnumerable<UserResultDto> RetrieveAllAsync(PaginationParams @params);
}