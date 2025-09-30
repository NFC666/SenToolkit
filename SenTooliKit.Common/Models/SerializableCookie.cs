using System.Net;

namespace SenTooliKit.Common.Models
{
    public class SerializableCookie
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public DateTime Expires { get; set; }

        public SerializableCookie() { }

        public SerializableCookie(Cookie cookie)
        {
            Name = cookie.Name;
            Value = cookie.Value;
            Domain = cookie.Domain;
            Path = cookie.Path;
            Expires = cookie.Expires;
        }

        public Cookie ToCookie()
        {
            return new Cookie(Name, Value, Path, Domain)
            {
                Expires = Expires
            };
        }
    }
}