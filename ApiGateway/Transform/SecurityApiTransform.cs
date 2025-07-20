namespace ApiGateway.Transform
{
    using System.Net.Http.Headers;
    using Yarp.ReverseProxy.Transforms;

    public class SecurityApiTransform : RequestTransform
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SecurityApiTransform(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public override async ValueTask ApplyAsync(RequestTransformContext context)
        {
            var httpContext = context.HttpContext;

            // 1. Check Authorization header
            if (!httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                httpContext.Response.StatusCode = 401;
                return;
            }

            // 2. Call Security API to validate token
            var client = _httpClientFactory.CreateClient("SecurityApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authHeader.ToString().Replace("Bearer ", ""));

            var response = await client.GetAsync("User/IsauthinticatedUser");
            if (!response.IsSuccessStatusCode)
            {
                httpContext.Response.StatusCode = 401;
                return;
            }
        }
    }

}
