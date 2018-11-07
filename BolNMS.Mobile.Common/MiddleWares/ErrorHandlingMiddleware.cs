
using BolNMS.Mobile.Common.DataTransfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BolNMS.Mobile.Common.MiddleWares
{
    public class ErrorHandlingMiddleware
    {
        private static ILoggerFactory _loggerFactory;
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            _loggerFactory = loggerFactory;
        }

        public async Task Invoke(HttpContext context /* other scoped dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = context.Response.StatusCode;
            var host = context.Request.Host;
            var url = context.Request.Path;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            var logger = _loggerFactory.CreateLogger(HelperUtility.BolNMSErrorMessage);
            logger.LogError(exception, exception.Message);
            DataTransferObject<List<int>> response = new DataTransferObject<List<int>>();
            response.IsSuccess = false;
            response.Message = HelperUtility.ErrorMessage;
            response.Data = new List<int>();
            var result = JsonConvert.SerializeObject(response);
            return context.Response.WriteAsync(result);
        }
    }
}
