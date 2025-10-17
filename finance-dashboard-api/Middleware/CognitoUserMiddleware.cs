namespace finance_dashboard_api.Middleware
{
    public class CognitoUserMiddleware
    {
        private readonly RequestDelegate _next;

        public CognitoUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sub = context.User.FindFirst("sub")?.Value;

            if (!string.IsNullOrEmpty(sub))
            {
                context.Items["UserId"] = sub;
            }

            await _next(context);
        }

    }
}
