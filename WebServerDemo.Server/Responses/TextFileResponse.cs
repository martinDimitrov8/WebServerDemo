using System.IO;
using System.Text;
using BasicWebServer.Server.HTTP;

namespace BasicWebServer.Server.Responses
{
	public class TextFileResponse : Response
	{
		public string FileName { get; }

		public TextFileResponse(string fileName)
			: base(StatusCode.OK)
		{
			FileName = fileName;
			Headers.Add(new Header(Header.ContentType,
				ContentType.PlainText));
		}

		public override string ToString()
		{
			if (File.Exists(FileName))
			{
				var content = File.ReadAllText(FileName);
				Body = content;

				var fileInfo = new FileInfo(FileName);

				Headers.Add(new Header(
					Header.ContentLength,
					fileInfo.Length.ToString()));

				Headers.Add(new Header(
					Header.ContentDisposition,
					$"attachment; filename={FileName}"));
			}

			return base.ToString();
		}
	}
}