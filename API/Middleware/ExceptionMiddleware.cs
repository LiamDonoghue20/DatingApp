using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware
        (
        //next = what runs next in the middleware after each part runs
        RequestDelegate next, 
        ILogger<ExceptionMiddleware>logger, 
        IHostEnvironment env
        )
        {
            this._next = next;
            this._logger = logger;
            this._env = env;
        }
        //InvokeAsync will run after to decide what we do with the exceptions
        public async Task InvokeAsync(HttpContext context)
        {
            //if any exception is thrown in the application, it will run in this try catch block 
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                // because its not an api controller, we need to specify the response is json
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //if the env is development, print out the stack trace error
            //other just return message, status code and "internal server error"
                var response = _env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, ex.Message, "internal server error");
                //setting this as we're not on an api controller    
                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};    
                var json = JsonSerializer.Serialize(response, options);

                //return http response that we receive when we encounter an exception in the app
                await context.Response.WriteAsync(json);
            }
        }
    }
}