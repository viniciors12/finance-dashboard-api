using finance_dashboard_api.Interface;

namespace finance_dashboard_api.Service
{
    public class UserContext: IUserContext
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public UserContext(IHttpContextAccessor accessor, IConfiguration config, IWebHostEnvironment env)
        {
            _accessor = accessor;
            _config = config;
            _env = env;
        }

        public string UserId
        {
            get
            {
                var userId = _accessor.HttpContext?.Items["UserId"]?.ToString();
                if (string.IsNullOrEmpty(userId) && _env.IsDevelopment())
                {
                    userId = _config["SimulatedUser:UserId"];
                }

                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User not available in context");

                return userId;
            }
        }
    }
}
