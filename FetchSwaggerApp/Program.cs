using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program {
    static async Task Main() {
        var handler = new HttpClientHandler {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };
        var client = new HttpClient(handler);
        try {
            var response = await client.GetAsync("http://localhost:7307/swagger/v1/swagger.json");
            Console.WriteLine("Status: " + response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Content: " + content);
        } catch(Exception e) {
            Console.WriteLine("Error: " + e.Message);
        }
    }
}
