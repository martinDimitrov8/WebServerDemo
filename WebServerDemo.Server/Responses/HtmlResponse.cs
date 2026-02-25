using BasicWebServer.Server.HTTP;

namespace BasicWebServer.Server.Responses
{
	public class HtmlResponse : ContentResponse
	{
		public HtmlResponse(string content)
			: base(content, ContentType.Html)
		{
		}
	}
}