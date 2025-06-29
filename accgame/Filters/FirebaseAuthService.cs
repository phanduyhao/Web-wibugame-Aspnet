using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class FirebaseAuthService
{
    private static readonly string FirebaseApiKey = "AIzaSyBMt1jHBEGi3m9cJKqAouSCwM_3DYFhfb8"; // Thay bằng API Key của Firebase bạn

    public async Task SendPasswordResetEmail(string email)
    {
        using (var client = new HttpClient())
        {
            var requestUri = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={FirebaseApiKey}";

            var payload = new
            {
                requestType = "PASSWORD_RESET",
                email = email
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(requestUri, content);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error sending password reset email: {responseBody}");
            }
        }
    }
}
