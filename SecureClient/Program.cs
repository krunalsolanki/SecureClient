﻿using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace SecureClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Making Call");
            RunAsync().GetAwaiter().GetResult();

        }

        private static async Task RunAsync()
        {
            AuthConfig config = AuthConfig.ReadJsonFileFromFile("appsettings.json");
            IConfidentialClientApplication app;
            app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                .WithClientSecret(config.ClientSecret)
                .WithAuthority(new Uri(config.Authority))
                .Build();

            string[] ResourceIds = new string[] { config.ResourceId };
            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenForClient(ResourceIds).ExecuteAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Token aquired \n");
                Console.WriteLine(result.AccessToken);
                Console.ReadLine();
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();

            }

            if(!string.IsNullOrWhiteSpace(result.AccessToken))
            {
                var httpClient = new HttpClient();
                var defaultRequestHeaders = httpClient.DefaultRequestHeaders;
                if(defaultRequestHeaders.Accept==null || !defaultRequestHeaders.Accept.Any
                    (m=>m.MediaType=="application/json"))
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue
                        ("application/json"));
                }
                defaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("bearer", result.AccessToken);

                HttpResponseMessage response = await httpClient.GetAsync(config.BaseAddress);
                if(response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    string json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);

                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to call {response.StatusCode}");
                    string content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
                Console.ResetColor();
            }
        }
    }
}
