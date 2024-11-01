using System.IdentityModel.Tokens.Jwt;

namespace cinemate.Services.TokenValidation
{
    public class TokenValidationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenValidationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string ValidateToken()
        {
            var authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                if (!string.IsNullOrEmpty(token))
                {
                    var jwtHandler = new JwtSecurityTokenHandler();
                    var jwtToken = jwtHandler.ReadJwtToken(token);

                    var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                    return userId;
                }
            }

            return null;
        }
    }
}
