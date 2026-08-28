using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicalSystem.DTOs;
using MedicalSystem.Helpers;
using MedicalSystem.Services;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var result = await _authService.GetProfileAsync(userId);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var result = await _authService.ChangePasswordAsync(userId, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("check-claim")]
        public async Task<IActionResult> CheckClaimAccount([FromBody] CheckClaimAccountDTO dto)
        {
            var result = await _authService.CheckClaimAccountAsync(dto.Phone);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("claim")]
        public async Task<IActionResult> ClaimAccount([FromBody] ClaimAccountDTO dto)
        {
            var result = await _authService.ClaimAccountAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
