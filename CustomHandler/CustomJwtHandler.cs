using Employee_Management_Backend.Helper;

namespace Employee_Management_Backend.CustomHandler
{
    public class CustomJwtHandler 
    {
        private readonly RequestDelegate _next;

        public CustomJwtHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var tokenHeader = context.Request.Headers["Authorization"].ToString();
                if (tokenHeader.StartsWith("Bearer "))
                {
                    var encryptedToken = tokenHeader.Substring("Bearer ".Length).Trim();
                    var decryptedToken = EncryptionHelper.DecryptString(encryptedToken);
                    context.Request.Headers["Authorization"] = "Bearer " + decryptedToken;
                }
            }

            await _next(context);
        }
    }
}
