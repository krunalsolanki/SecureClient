using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using Microsoft.Extensions.Configuration;


namespace SecureClient
{
   public class AuthConfig
    {
        public string Instance { get; set; }
        public string TenatId { get; set; }
        public string ClientId { get; set; }
        public string Authority { get {
                return String.Format(CultureInfo.InvariantCulture, Instance, TenatId);
            } }
        public string ClientSecret { get; set; }
        public string BaseAddress { get; set; }
        public string ResourceId { get; set; }

        public static   AuthConfig ReadJsonFileFromFile(string path)
        {
            IConfiguration Configuration;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path);
            Configuration = builder.Build();
            return Configuration.Get<AuthConfig>();

        }
    }
}
