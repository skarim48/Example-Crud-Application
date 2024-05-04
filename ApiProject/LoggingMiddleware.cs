namespace ApiProject
{
    public class LoggingMiddleware : IMiddleware
    {
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _logger.LogInformation("Request received: {Method} {Path}", context.Request.Method, context.Request.Path);

            //using (StreamWriter writer = new StreamWriter(@"C:\Users\skarim\Desktop\Samir\Result.txt"))
            //{
            //    writer.WriteLine("Start :");
            //    writer.WriteLine("Request received: {Method} {Path}", context.Request.Method, context.Request.Path);
            //}

            await next(context);

            _logger.LogInformation("Response sent: {StatusCode}", context.Response.StatusCode);

            //using (StreamWriter writer = new StreamWriter(@"C:\Users\skarim\Desktop\Samir\Result.txt"))
            //{
            //    writer.WriteLine("End :");
            //    writer.WriteLine("Response sent: {StatusCode}", context.Response.StatusCode);
            //}
        }
    }
}
