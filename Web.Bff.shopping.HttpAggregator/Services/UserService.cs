
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using Web.Bff.shopping.HttpAggregator.Model;

namespace Web.Bff.shopping.HttpAggregator.Services
{
    public class UserService
    {
        private HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<object> GetCuentClaim()
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_httpClient.BaseAddress + "User/GetCuentClaim"),
                Method = HttpMethod.Post
            };
            var result = await _httpClient.SendAsync(request);
          
            return result.Content;
        }
        public async Task<bool> CreateaUser(CreateUserModel createUserModel)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_httpClient.BaseAddress + "User/CreateUser"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(createUserModel), Encoding.UTF8, "application/json")

            };
            var result = await _httpClient.SendAsync(request);
            var c = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<object>(await result.Content.ReadAsStringAsync());

            return true;//refavtor it to retutn strongly typed object 
        }
        public async Task<string> SignIn(LoginUserModel loginUserModel)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_httpClient.BaseAddress + "User/LoginUser"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(loginUserModel), Encoding.UTF8, "application/json")

            };

            var result = await _httpClient.SendAsync(request);
            var c = await result.Content.ReadAsStringAsync();

            var response = JsonSerializer.Deserialize<string>(c);

            return response;
        }
    }
 
}
