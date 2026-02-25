using System.Collections.Generic;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;

namespace BasicWebServer.Server.Routing
{
    public class RoutingTable : IRoutingTable
    {
        private readonly Dictionary<Method,
            Dictionary<string, Response>> routes;

        public RoutingTable()
        {
            routes = new Dictionary<Method,
                Dictionary<string, Response>>
            {
                [Method.GET] = new Dictionary<string, Response>(),
                [Method.POST] = new Dictionary<string, Response>()
            };
        }

        public IRoutingTable MapGet(string url, Response response)
        {
            routes[Method.GET][url] = response;
            return this;
        }

        public IRoutingTable MapPost(string url, Response response)
        {
            routes[Method.POST][url] = response;
            return this;
        }

        public Response MatchRequest(Request request)
        {
            if (routes.ContainsKey(request.Method) &&
                routes[request.Method].ContainsKey(request.Url))
            {
                return routes[request.Method][request.Url];
            }

            return new NotFoundResponse();
        }
    }
}