﻿namespace FSI.Authentication.Domain.Abstractions.Security
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string passwordHash);
    }
}