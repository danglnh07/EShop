using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Log the incoming request
            logger.LogInformation("[START] Handle Request={Request} - Response={Response} - RequestData={RequestData}",
                    typeof(TRequest).Name, typeof(TResponse).Name, request);


            // Calculate request time
            var timer = new Stopwatch();
            timer.Start();
            var response = await next();
            timer.Stop();

            if (timer.ElapsedMilliseconds > 1000)
            {
                logger.LogWarning("[PERFORMANCE] Request {Request} taken longer than expected ({TimeTaken} ms)",
                        typeof(TRequest).Name, timer.ElapsedMilliseconds);
            }

            // Log the outgoing response
            logger.LogInformation("[END] Handle Request={Request} with Response={Response} in ElapsedTime={ElapsedTime} ms",
                    typeof(TRequest).Name, typeof(TResponse).Name, timer.ElapsedMilliseconds);

            return response;
        }
    }
}
