using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Store.WebUI.Helpers
{
    public static class ErrorHelper
    {
        public static async Task<string> GetErrorMessageAsync(HttpResponseMessage response)
        {
            var rawContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(rawContent))
            {
                return $"Sunucudan hata kodu alındı: {response.StatusCode}";
            }

            try
            {
                // Dönen içeriğin JSON formatında bir hata objesi olup olmadığını kontrol et
                var errorObj = JsonConvert.DeserializeObject<ApiErrorResponse>(rawContent);
                if (errorObj != null && !string.IsNullOrEmpty(errorObj.Message))
                {
                    if (!string.IsNullOrEmpty(errorObj.Detail))
                    {
                        return $"{errorObj.Message} (Detay: {errorObj.Detail})";
                    }
                    return errorObj.Message;
                }
            }
            catch
            {
                // JSON parse edilemezse doğrudan ham içeriği dön
            }

            return rawContent;
        }

        private class ApiErrorResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; } = string.Empty;

            [JsonProperty("detail")]
            public string Detail { get; set; } = string.Empty;
        }
    }
}
