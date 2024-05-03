using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using ModelProject.JWT;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Text.Json;

namespace ModelProject.CallingAPIs
{
    public class UsersApis
    {
        private readonly IJSRuntime jsRuntime;
        LocalStorageService localStorageService;


        public UsersApis(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
            localStorageService = new LocalStorageService(jsRuntime);
        }

        public UsersApis()
        {
        }
        public string CreateUser(IdentityUser newUser)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var responseTask = client.PostAsJsonAsync("http://localhost:5264/api/Users/Registration/", newUser);
                    responseTask.Wait();
                    if (responseTask.IsCompleted)
                    {
                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var messageTask = result.Content.ReadAsStringAsync();
                            messageTask.Wait();
                            string token = messageTask.Result;
                            var tokenObject = System.Text.Json.JsonSerializer.Deserialize<ModelProject.JWT.TokenObject>(token);
                            return tokenObject.token;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return "";

        }

        public string LoginUser(IdentityUser newUser)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var responseTask = client.PostAsJsonAsync("http://localhost:5264/api/Users/LoginApi/", newUser);
                    responseTask.Wait();
                    if (responseTask.IsCompleted)
                    {
                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var messageTask = result.Content.ReadAsStringAsync();
                            messageTask.Wait();
                            string token = messageTask.Result;
                            var tokenObject = System.Text.Json.JsonSerializer.Deserialize<ModelProject.JWT.TokenObject>(token);
                            return tokenObject.token;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return "";

        }

        public async void LogOut()
        {
            try
            {
                localStorageService = new LocalStorageService(this.jsRuntime);
                await localStorageService.RemoveItem("jwttoken");
            }
            catch (Exception ex)
            {
            }
        }

        public async Task<List<Product>> GetProducts()
        {
            List<Product> model = new List<Product>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (localStorageService != null)
                    {
                        string accessToken = LocalStorageService.token;
                        //client.DefaultRequestHeaders.Add("Authorization", LocalStorageService.token);
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                    }

                    //var task = client.GetAsync("http://localhost:7076/api/Users/apiGetProductsi/")
                    //  .ContinueWith((taskwithresponse) =>
                    //  {
                    //      var response = taskwithresponse.Result;
                    //      var jsonString = response.Content.ReadAsStringAsync();
                    //      jsonString.Wait();
                    //      model = JsonConvert.DeserializeObject<List<Product>>(jsonString.Result);

                    //  });
                    //task.Wait();

                    var responseTask = client.GetAsync("https://localhost:7076/api/Users/apiGetProducts/");
                    responseTask.Wait();
                    if (responseTask.IsCompleted)
                    {
                        var result = responseTask.Result;
                        var jsonString = result.Content.ReadAsStringAsync();
                        jsonString.Wait();
                        model = JsonConvert.DeserializeObject<List<Product>>(jsonString.Result);
                        if (result.IsSuccessStatusCode)
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return model;
        }

    }
}
