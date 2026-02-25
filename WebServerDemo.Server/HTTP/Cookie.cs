namespace BasicWebServer.Server.HTTP
{
    public class Cookie
    {
        public Cookie(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; set; }

        public override string ToString()
            => $"{Name}={Value}";
    }
}
