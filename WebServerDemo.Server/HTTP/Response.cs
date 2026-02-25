public override string ToString()
{
    public CookieCollection Cookies { get; }
Cookies = new CookieCollection();
foreach (var cookie in Cookies)
{
    sb.AppendLine($"{Header.SetCookie}: {cookie}");
}
var sb = new StringBuilder();

    sb.AppendLine($"HTTP/1.1 {(int)StatusCode} {StatusCode}");

    foreach (var header in Headers)
    {
        sb.AppendLine(header.ToString());
    }

    sb.AppendLine();

    if (!string.IsNullOrEmpty(Body))
    {
        sb.AppendLine(Body);
    }

    return sb.ToString();
}
