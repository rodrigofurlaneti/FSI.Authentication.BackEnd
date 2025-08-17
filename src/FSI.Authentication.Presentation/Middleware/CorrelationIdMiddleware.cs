﻿namespace FSI.Authentication.Presentation.Middleware
{
    public sealed class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(HeaderName, out var correlationId) || string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString("N");
                context.Response.Headers[HeaderName] = correlationId!;
            }
            else
            {
                context.Response.Headers[HeaderName] = correlationId.ToString();
            }

            context.Items[HeaderName] = correlationId.ToString();
            await _next(context);
        }
    }
}
