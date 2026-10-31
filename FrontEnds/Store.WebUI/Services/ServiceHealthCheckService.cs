using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Store.WebUI.Models;

namespace Store.WebUI.Services
{
    public class ServiceHealthCheckService : IServiceHealthCheckService
    {
        private readonly ApiSettings _apiSettings;
        private readonly ILogger<ServiceHealthCheckService> _logger;
        private readonly HttpClient _httpClient;
        private static readonly object _fileLock = new();
        private ServiceHealthReport? _lastReport;

        public bool HasDefaultPageBeenChecked { get; set; } = false;

        public ServiceHealthCheckService(
            IOptions<ApiSettings> apiSettings,
            ILogger<ServiceHealthCheckService> logger)
        {
            _apiSettings = apiSettings?.Value ?? new ApiSettings();
            _logger = logger;

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMilliseconds(1800)
            };
        }

        public ServiceHealthReport? GetLastReport()
        {
            return _lastReport;
        }

        public async Task<ServiceHealthReport> CheckAndLogAsync(string triggerSource)
        {
            var report = new ServiceHealthReport
            {
                CheckTime = DateTime.Now,
                TriggerSource = triggerSource
            };

            // Store mikroservisleri (7301-7308 ardışık port standartı)
            var endpoints = new List<(string Name, string Url, int Port)>
            {
                ("IdentityServer (Kimlik)", string.IsNullOrEmpty(_apiSettings.IdentityServerUrl) ? "https://localhost:5001" : _apiSettings.IdentityServerUrl, 5001),
                ("Catalog (Katalog Servisi)", string.IsNullOrEmpty(_apiSettings.CatalogBaseUrl) ? "http://localhost:7301" : _apiSettings.CatalogBaseUrl, 7301),
                ("Discount (İndirim Servisi)", string.IsNullOrEmpty(_apiSettings.DiscountBaseUrl) ? "https://localhost:7302" : _apiSettings.DiscountBaseUrl, 7302),
                ("Location (Lokasyon Servisi)", string.IsNullOrEmpty(_apiSettings.LocationBaseUrl) ? "https://localhost:7303" : _apiSettings.LocationBaseUrl, 7303),
                ("Order (Sipariş Servisi)", string.IsNullOrEmpty(_apiSettings.OrderBaseUrl) ? "https://localhost:7304" : _apiSettings.OrderBaseUrl, 7304),
                ("B2B (Bayi Yönetim Servisi)", "http://localhost:7305", 7305),
                ("Basket (Sepet Servisi)", string.IsNullOrEmpty(_apiSettings.BasketBaseUrl) ? "http://localhost:7306" : _apiSettings.BasketBaseUrl, 7306),
                ("Favorite (Favori Servisi)", string.IsNullOrEmpty(_apiSettings.FavoriteBaseUrl) ? "http://localhost:7307" : _apiSettings.FavoriteBaseUrl, 7307),
                ("Rental (Kiralama Servisi)", string.IsNullOrEmpty(_apiSettings.RentalBaseUrl) ? "http://localhost:7308" : _apiSettings.RentalBaseUrl, 7308)
            };

            var tasks = endpoints.Select(ep => CheckSingleServiceWithFallbackAsync(ep.Name, ep.Url, ep.Port)).ToList();
            var results = await Task.WhenAll(tasks);

            report.Services = results.ToList();
            _lastReport = report;

            WriteLogToFile(report);

            return report;
        }

        private async Task<ServiceHealthItem> CheckSingleServiceWithFallbackAsync(string name, string url, int port)
        {
            var result = await CheckSingleServiceUrlAsync(name, url, port);
            if (result.IsOnline)
            {
                return result;
            }

            // Eğer protokol uyumsuzluğu (örn: HTTP yerine HTTPS veya tam tersi) olduysa alternatif protokolü dene
            string? fallbackUrl = null;
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                fallbackUrl = "https://" + url.Substring(7);
            }
            else if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                fallbackUrl = "http://" + url.Substring(8);
            }

            if (!string.IsNullOrEmpty(fallbackUrl))
            {
                var fallbackResult = await CheckSingleServiceUrlAsync(name, fallbackUrl, port);
                if (fallbackResult.IsOnline)
                {
                    return fallbackResult;
                }
            }

            return result;
        }

        private async Task<ServiceHealthItem> CheckSingleServiceUrlAsync(string name, string url, int port)
        {
            var item = new ServiceHealthItem
            {
                ServiceName = name,
                Url = url,
                Port = port
            };

            var sw = Stopwatch.StartNew();
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                sw.Stop();

                item.ResponseTimeMs = sw.ElapsedMilliseconds;
                item.StatusCode = (int)response.StatusCode;
                item.IsOnline = true;
                item.Message = $"HTTP {(int)response.StatusCode} {response.StatusCode} ({item.ResponseTimeMs} ms)";
            }
            catch (Exception ex)
            {
                sw.Stop();
                item.ResponseTimeMs = sw.ElapsedMilliseconds;
                item.IsOnline = false;

                var baseEx = ex.GetBaseException();
                var msg = baseEx.Message;
                if (msg.Contains("actively refused") || msg.Contains("reddedildi"))
                {
                    item.Message = "Bağlantı Reddedildi (Servis Kapalı veya Port Dinlenmiyor)";
                }
                else if (msg.Contains("timed out") || msg.Contains("zaman aşımı"))
                {
                    item.Message = "Zaman Aşımı (1800 ms yanıt alınamadı)";
                }
                else
                {
                    item.Message = msg;
                }
            }

            return item;
        }

        private void WriteLogToFile(ServiceHealthReport report)
        {
            try
            {
                string logsDir = @"C:\User\Projects\Store\Loglar";
                if (!Directory.Exists(logsDir))
                {
                    Directory.CreateDirectory(logsDir);
                }

                // Her gün yeni bir dosya açılır: ServisLog_yyyyMMdd.txt
                var fileName = $"ServisLog_{report.CheckTime:yyyyMMdd}.txt";
                var fullPath = Path.Combine(logsDir, fileName);

                var sb = new StringBuilder();
                sb.AppendLine(new string('=', 110));
                sb.AppendLine($"Tarih / Saat       : {report.CheckTime:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"Tetikleyici Durumu : {report.TriggerSource}");
                sb.AppendLine($"Genel Özet         : Toplam {report.Services.Count} Servis | {report.OnlineServices.Count} ÇEVRİMİÇİ | {report.OfflineServices.Count} ÇEVRİMDİŞİ");
                sb.AppendLine(new string('-', 110));
                sb.AppendLine($"{"DURUM",-12} | {"PORT",-6} | {"SERVİS ADI",-30} | {"URL",-28} | {"DETAY / HATA MESAJI"}");
                sb.AppendLine(new string('-', 110));

                foreach (var s in report.Services)
                {
                    var statusText = s.IsOnline ? "[BAŞARILI]" : "[BAŞARISIZ]";
                    sb.AppendLine($"{statusText,-12} | {s.Port,-6} | {s.ServiceName,-30} | {s.Url,-28} | {s.Message}");
                }

                sb.AppendLine(new string('=', 110));
                sb.AppendLine();

                lock (_fileLock)
                {
                    string existingContent = string.Empty;
                    if (File.Exists(fullPath))
                    {
                        existingContent = File.ReadAllText(fullPath, Encoding.UTF8);
                    }

                    // Kullanıcı talebi: Yeni yazılan log bloğu her zaman dosyanın en üstünde kalır
                    var combinedContent = sb.ToString() + existingContent;
                    File.WriteAllText(fullPath, combinedContent, Encoding.UTF8);
                }

                _logger.LogInformation("Servis sağlık logu başarıyla yazıldı: {Path}", fullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis log dosyasına yazılırken hata oluştu.");
            }
        }
    }
}
