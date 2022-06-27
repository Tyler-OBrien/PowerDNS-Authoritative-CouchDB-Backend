﻿using System.Net;
using PowerDNS_Auth_CouchDB_Remote_Backend.Extensions.HTTPClient;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.API_Responses;
using Sentry;
using Serilog;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Middleware;

public class JSONErrorMiddleware : IMiddleware
{
    // We want to return as many errors as we can in json ErrorResponse Format so any client can get a helpful error response it can read and not just a boring status code
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
            if (context.Response.StatusCode == 404 && context.Response.HasStarted == false)
            {
                var errorResponse = new ErrorResponse(context.Response.StatusCode,
                    $"Path not found: ({context.Request.Method}: {context.Request.Path})", "path_not_found");

                await context.Response.WriteAsJsonAsync(errorResponse);
            }

            if (context.Response.StatusCode == 405 && context.Response.HasStarted == false)
            {
                var errorResponse = new ErrorResponse(context.Response.StatusCode,
                    $"Method not allowed: ({context.Request.Method}: {context.Request.Path})", "method_not_allowed");

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
        catch (HttpRequestException requestException)
        {
            await requestException.HandleExceptionAsync(
                $"Internal Server Error {context.Request.Method}: {context.Request.Path})");
            var errorResponse = new ErrorResponse(HttpStatusCode.InternalServerError, "Unexpected internal error",
                "internal_error");

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error");
            SentrySdk.CaptureException(ex);
            var errorResponse =
                new ErrorResponse(context.Response.StatusCode, "Internal Server Error", "internal_error");

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}