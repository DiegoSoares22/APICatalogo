using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace APICatalogo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ITokenService tokenService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Verificar as credenciais de um usuário
    /// </summary>
    /// <param name="model">Um objeto do tipo UsuarioDTO</param>
    /// <returns>Status 200 e o token para o cliente</returns>
    /// <remarks>Retorna o Status 200 e o token</remarks>
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName!);

        if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password!))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("id", user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            var token = _tokenService.GenerateAccesToken(authClaims, _configuration);

            var refreshToken = _tokenService.GenerateRefreshToken();
            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes);

            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidityInMinutes);

            user.RefreshToken = refreshToken;

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }
        return Unauthorized();
    }

    /// <summary>
    /// Cria um novo perfil (role) no sistema
    /// </summary>
    /// <param name="roleName">O nome do perfil a ser criado</param>
    /// <returns>Status 200 se criado com sucesso, ou Status 400 em caso de erro</returns>
    /// <remarks>Requer a política "SuperAdminOnly". Retorna erro se o perfil já existir</remarks>
    [HttpPost]
    [Authorize(Policy = "SuperAdminOnly")]
    [Route("CreateRole")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var roleExist = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                _logger.LogInformation(1, "Roles added");
                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = $"Role {roleName} added successfully!" });
            }
            else
            {
                _logger.LogInformation(2, "Error");
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = $"Issue adding the new {roleName} role!" });
            }
        }

        return StatusCode(StatusCodes.Status400BadRequest,
            new Response { Status = "Error", Message = $"Role {roleName} already exists!" });
    }

    /// <summary>
    /// Atribui um perfil (role) a um usuário existente
    /// </summary>
    /// <param name="email">O e-mail do usuário que receberá o perfil</param>
    /// <param name="roleName">O nome do perfil a ser atribuído</param>
    /// <returns>Status 200 se atribuído com sucesso, ou Status 400 em caso de erro</returns>
    /// <remarks>Requer a política "SuperAdminOnly". Retorna erro se o usuário não for encontrado</remarks>
    [HttpPost]
    [Authorize(Policy = "SuperAdminOnly")]
    [Route("AddUserToRole")]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                _logger.LogInformation(1, $"User {user.Email} added to the {roleName} role");
                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = $"User {user.Email} added to {roleName} role successfully!" });
            }
            else
            {
                _logger.LogInformation(1, $"Error: Unable to add user {user.Email} to the {roleName} role");
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = $"Issue adding the user {user.Email} to the {roleName} role!" });
            }
        }
        return BadRequest(new { error = "Unable to find user" });
    }

    /// <summary>
    /// Registra um novo usuário no sistema
    /// </summary>
    /// <param name="model">Um objeto do tipo RegisterModel contendo username, email e password</param>
    /// <returns>Status 200 se registrado com sucesso, ou Status 500 em caso de erro</returns>
    /// <remarks>Retorna erro se o usuário já existir ou se os dados fornecidos forem inválidos</remarks>
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username!);

        if (userExists != null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
        }

        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };

        var result = await _userManager.CreateAsync(user, model.Password!);

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
        }
        return Ok(new Response { Status = "Success", Message = "User created successfully!" });
    }

    /// <summary>
    /// Renova o Access Token utilizando um Refresh Token válido
    /// </summary>
    /// <param name="tokenModel">Um objeto contendo o AccessToken expirado e o RefreshToken válido</param>
    /// <returns>Status 200 com o novo AccessToken e RefreshToken, ou Status 400 em caso de erro</returns>
    /// <remarks>O RefreshToken deve ser válido e não expirado para que a renovação ocorra</remarks>
    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
        if (tokenModel is null)
        {
            return BadRequest("Invalid client request");
        }
        string? accessToken = tokenModel.AccesToken ?? throw new ArgumentNullException(nameof(tokenModel.AccesToken));
        string? refreshToken = tokenModel.RefreshToken ?? throw new ArgumentNullException(nameof(tokenModel.RefreshToken));

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _configuration);

        if (principal == null)
        {
            return BadRequest("Invalid access token or refresh token");
        }

        string username = principal.Identity.Name;

        var user = await _userManager.FindByNameAsync(username);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return BadRequest("Invalid access token or refresh token");
        }
        var newAccessToken = _tokenService.GenerateAccesToken(principal.Claims.ToList(), _configuration);

        var newRefreshToken = _tokenService.GenerateRefreshToken();

        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken
        });
    }

    /// <summary>
    /// Revoga o Refresh Token de um usuário, encerrando sua sessão
    /// </summary>
    /// <param name="username">O nome do usuário que terá o token revogado</param>
    /// <returns>Status 204 se revogado com sucesso, ou Status 400 se o usuário não for encontrado</returns>
    /// <remarks>Requer a política "ExclusiveOnly". Após a revogação o usuário deverá realizar login novamente</remarks>
    [Authorize(Policy = "ExclusiveOnly")]
    [HttpPost]
    [Route("revoke/{username}")]
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null) return BadRequest("User not found!");

        user.RefreshToken = null;

        await _userManager.UpdateAsync(user);

        return NoContent();
    }
}