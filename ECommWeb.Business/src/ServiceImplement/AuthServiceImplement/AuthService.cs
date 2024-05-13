using AutoMapper;
using ECommWeb.Core.src.Common;
using ECommWeb.Core.src.Entity;
using ECommWeb.Core.src.RepoAbstract;
using ECommWeb.Business.src.DTO;
using ECommWeb.Business.src.ServiceAbstract.AuthServiceAbstract;
using ECommWeb.Business.src.Shared;

namespace Server.Service.src.ServiceImplement.AuthServiceImplement;

public class AuthService : IAuthService
{
    private IUserRepo _userRepo;
    private ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IPasswordService _passwordService;

    public AuthService(IUserRepo userRepo, ITokenService tokenService, IMapper mapper, IPasswordService passwordService)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
        _mapper = mapper;
        _passwordService = passwordService;
    }

    public async Task<UserReadDto> GetCurrentProfile(Guid id)
    {
        var foundUser = await _userRepo.GetUserByIdAsync(id);
        if (foundUser != null)
        {
            return _mapper.Map<User, UserReadDto>(foundUser);
        }
        throw CustomException.NotFoundException("User not found");
    }

    public async Task<string> Login(UserCredential credential)
    {
        var foundByEmail = await _userRepo.GetUserByCredentialsAsync(credential);
        if (foundByEmail is null)
        {
            throw CustomException.NotFoundException("Email not found");
        }
        Console.WriteLine("AuthService hashed password: " + foundByEmail.Password + "salt: " + foundByEmail.Salt);
        var isPasswordMatch = _passwordService.VerifyPassword(credential.Password, foundByEmail.Password, foundByEmail.Salt);
        if (isPasswordMatch)
        {
            return _tokenService.GetToken(foundByEmail);
        }
        throw CustomException.UnauthorizedException("Password incorrect");
    }

}

