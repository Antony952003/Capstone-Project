﻿using EmployeeGrievanceRedressal.Models.DTOs.Login;
using EmployeeGrievanceRedressal.Models.DTOs.Register;

namespace EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(UserRegisterDTO model);
        Task<AuthResponse> LoginAsync(UserLoginDTO model);
    }
}
