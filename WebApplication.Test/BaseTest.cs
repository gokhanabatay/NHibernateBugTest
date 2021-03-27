using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.Test
{
    public abstract class BaseApiTest<TStartup> where TStartup : class
        {
            public async Task<T> GetAsync<T>(string uri, HttpStatusCode statusCode = HttpStatusCode.OK) 
            {
                return await GetAsync<T>(uri, null, statusCode);
            }

            public async Task<T> GetAsync<T>(string uri, Dictionary<string, string> parameters, HttpStatusCode statusCode = HttpStatusCode.OK) 
            {
                using (var client = WebFactory.CreateClient())
                {
                    AddRequestHeaders(client);
                    string endpoint = GetEndpoint(uri, parameters);
                    var httpResponse = await client.GetAsync(endpoint);
                    var response = await httpResponse.Content.ReadFromJsonAsync<T>();
                    if (statusCode == HttpStatusCode.OK)
                    {
                        response.Should().NotBeNull();
                    }
                    httpResponse.StatusCode.Should().Be(statusCode);
                    return response;
                }
            }

            public async Task<T> DeleteAsync<T>(string uri, Dictionary<string, string> parameters, HttpStatusCode statusCode = HttpStatusCode.OK) 
            {
                using (var client = WebFactory.CreateClient())
                {
                    AddRequestHeaders(client);
                    string endpoint = GetEndpoint(uri, parameters);
                    var httpResponse = await client.DeleteAsync(endpoint);
                    var response = await httpResponse.Content.ReadFromJsonAsync<T>();
                    if (statusCode == HttpStatusCode.OK)
                    {
                        response.Should().NotBeNull();
                    }
                    httpResponse.StatusCode.Should().Be(statusCode);
                    return response;
                }
            }

            public async Task<T> PostAsync<T>(string uri, object content = null, HttpStatusCode statusCode = HttpStatusCode.OK) 
            {
                return await PostAsync<T>(uri, content, null, statusCode);
            }

            public async Task<T> PostAsync<T>(string uri, Dictionary<string, string> parameters, HttpStatusCode statusCode = HttpStatusCode.OK) 
            {
                return await PostAsync<T>(uri, null, parameters, statusCode);
            }

            public async Task<T> PostAsync<T>(string uri, object content, Dictionary<string, string> parameters, HttpStatusCode statusCode = HttpStatusCode.OK) 
            {
                using (var client = WebFactory.CreateClient())
                {
                    AddRequestHeaders(client);
                    string endpoint = GetEndpoint(uri, parameters);
                    var httpResponse = await client.PostAsJsonAsync(endpoint, content);
                    var response = await httpResponse.Content.ReadFromJsonAsync<T>();
                    if (statusCode == HttpStatusCode.OK)
                    {
                        response.Should().NotBeNull();
                    }
                    httpResponse.StatusCode.Should().Be(statusCode);
                    return response;
                }
            }

            public async Task<T> PutAsync<T>(string uri, object content, HttpStatusCode statusCode = HttpStatusCode.OK) 
            {
                return await PutAsync<T>(uri, content, null, statusCode);
            }

            public async Task<T> PutAsync<T>(string uri, object content, Dictionary<string, string> parameters, HttpStatusCode statusCode = HttpStatusCode.OK) 
            {
                using (var client = WebFactory.CreateClient())
                {
                    AddRequestHeaders(client);
                    string endpoint = GetEndpoint(uri, parameters);
                    var httpResponse = await client.PutAsJsonAsync(endpoint, content);
                    var response = await httpResponse.Content.ReadFromJsonAsync<T>();
                    if (statusCode == HttpStatusCode.OK)
                    {
                        response.Should().NotBeNull();
                    }
                    httpResponse.StatusCode.Should().Be(statusCode);
                    return response;
                }
            }

            private static void AddRequestHeaders(HttpClient client)
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
            }

            private static string GetEndpoint(string uri, Dictionary<string, string> parameters)
            {
                StringBuilder builder = new StringBuilder();
                if (parameters?.Any() == true)
                {
                    foreach (var parameter in parameters)
                    {
                        if (builder.Length == 0)
                        {
                            builder.Append($"?{parameter.Key}={parameter.Value}");
                        }
                        else
                        {
                            builder.Append($"&{parameter.Key}={parameter.Value}");
                        }
                    }
                }

                return $"{uri}{builder}";
            }

            protected CustomWebApplicationFactory<TStartup> WebFactory => Singleton<CustomWebApplicationFactory<TStartup>>.Instance;
        }
    

    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TStartup>();
                });
        }
    }

}
