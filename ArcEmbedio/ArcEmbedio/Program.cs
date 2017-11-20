using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO.Modules;
using Unosquare.Net;
using Unosquare.Swan;

namespace ArcEmbedio
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "http://localhost:9696/";

            using(var server = new WebServer(url, RoutingStrategy.Regex))
            {
                server.RegisterModule(new WebApiModule());
                server.WithWebApiController<Controller>();
                server.RunAsync();

                var browser = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo(url + "api/echo/Warsong") { UseShellExecute = true }
                };

                browser.Start();
                Console.ReadKey(true);
            }
        }
    }

    public class Controller : WebApiController
    {
        [WebApiHandler(HttpVerbs.Get, "/api/echo/{message}")]
        public bool Echo(WebServer server, HttpListenerContext context, string message)
        {
            try
            {
                context.JsonResponse(new
                {
                    Message = Reverse(message)
                });
                return true;
            }
            catch(Exception ex)
            {
                return HandleError(context, ex);
            }
        }
        protected bool HandleError(HttpListenerContext context, Exception ex, int statusCode = 500)
        {
            var errorResponse = new
            {
                Title = "Unexpected Error",
                ErrorCode = ex.GetType().Name,
                Description = ex.ExceptionMessage(),
            };
            context.Response.StatusCode = statusCode;
            return context.JsonResponse(errorResponse);
        }

        public string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }

    
}
