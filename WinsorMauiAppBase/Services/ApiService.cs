
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WinsorMauiAppBase.Services
{
    internal static class ApiService
    {
        public static string? AuthUserId => AuthorizedUser?.userId;
        public static DateTime? AuthExpires => AuthorizedUser?.expires;

        private static AuthResponse? AuthorizedUser;
        public static UserInfo? UserInfo;
        private static readonly HttpClient client = new HttpClient()
        {
            BaseAddress = new("https://forms-dev.winsor.edu")
        };


        public static Task MaintainAuthorization(CancellationToken cancellationToken) => Task.Run(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (AuthorizedUser is not null && AuthorizedUser.expires < DateTime.Now.AddMinutes(-2))
                {
                    RenewToken();
                }

                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        });

        public static void Login(string email, string password)
        {
            LoginRequest login = new(email, password);
            try
            {
                AuthorizedUser = ApiCall<AuthResponse>(HttpMethod.Post, "api/auth", JsonSerializer.Serialize(login), false);
                UserInfo = ApiCall<UserInfo>(HttpMethod.Get, "api/users/self");
            }
            catch(ApiException e)
            {
                throw e;
            }
        }

        public static void RenewToken()
        {
            AuthorizedUser = ApiCall<AuthResponse>(HttpMethod.Get, "api/auth/renew");
        }

        public static T? ApiCall<T>(HttpMethod method, string endpoint, string jsonContent = "", bool authorize = true, Func<HttpResponseMessage, T?> onNonSuccessStatus = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, endpoint);
            if(authorize && (AuthorizedUser is null || AuthorizedUser.expires < DateTime.Now))
            {
                throw new UnauthorizedAccessException("Unable to Authorize request.  Token is missing or expired.");
            }

            if(authorize)
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthorizedUser.jwt);
            }

            if(!string.IsNullOrEmpty(jsonContent))
            {
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            }

            try
            {
                var response = client.SendAsync(request).Result;
                if (!response.IsSuccessStatusCode)
                {
                    if(onNonSuccessStatus is null)
                        throw new ApiException(response);

                    return onNonSuccessStatus(response);
                }
                var jsonResponse = response.Content.ReadAsStringAsync().Result;

                return JsonSerializer.Deserialize<T>(jsonResponse);
            }
            catch(Exception e)
            {
                throw new InvalidOperationException($"Api Call to {endpoint} failed.", e);
            }

        }
    }

    internal class ApiException : Exception
    { 
        public HttpResponseMessage ResponseMessage { get; private set; }
        public ApiException(HttpResponseMessage responseMessage) : base(responseMessage.Content.ReadAsStringAsync().Result)
        {
            ResponseMessage = responseMessage;
        }
    }

    internal record LoginRequest(string email, string password);
    internal record AuthResponse(string userId, string jwt, DateTime expires);
    internal record UserInfo(string firstName, string lastName, string email, int blackbaudId);
}
