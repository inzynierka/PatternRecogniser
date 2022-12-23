using Microsoft.AspNetCore.Http;
using PatternRecogniser.Errors;
using System;
using System.Threading.Tasks;

namespace PatternRecogniser.Middleware
{
    public class ErrorHandlingMiddleware: IMiddleware
    {
        public ErrorHandlingMiddleware()
        {
        }

public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch(BadRequestExeption badRequestExeption)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(badRequestExeption.Message);
            }
            catch (UnauthorizedExeption unauthorizedExeption)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(unauthorizedExeption.Message);
            }
            catch (NotFoundExeption notFoundExeption)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(notFoundExeption.Message);
            }
            
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(e.Message);
            }
        }
    }
}
