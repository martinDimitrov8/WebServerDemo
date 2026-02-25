using System.Text;
using BasicWebServer.Server.HTTP;

namespace BasicWebServer.Server.Responses
{
    public abstract class ContentResponse : Response
    {
        protected ContentResponse(
            string content,
            string contentType,
            System.Action<Request, Response> preRenderAction = null)
            : base(StatusCode.OK)
        {
            Body = content;
            Headers.Add(new Header(Header.ContentType, contentType));
            PreRenderAction = preRenderAction;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Body))
            {
                var length = Encoding.UTF8.GetByteCount(Body);
                Headers.Add(new Header(Header.ContentLength, length.ToString()));
            }

            return base.ToString();
        }
    }
}
