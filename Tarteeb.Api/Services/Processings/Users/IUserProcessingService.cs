//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using Tarteeb.Api.Models.Foundations.Users;

namespace Tarteeb.Api.Services.Processings.Users
{
    public interface IUserProcessingService
    {
        User RetrieveUserByCredentials(string email, string password);
        ValueTask<Guid> VerifyUserByIdAsync(Guid userId);
        ValueTask<Guid> ActivateUserByIdAsync(Guid userId);
    }
}