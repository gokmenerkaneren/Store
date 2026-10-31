using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.WebUI.Services
{
    public class ServiceHealthItem
    {
        public string ServiceName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool IsOnline { get; set; }
        public int? StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public long ResponseTimeMs { get; set; }
    }

    public class ServiceHealthReport
    {
        public DateTime CheckTime { get; set; } = DateTime.Now;
        public string TriggerSource { get; set; } = string.Empty;
        public List<ServiceHealthItem> Services { get; set; } = new();
        public List<ServiceHealthItem> OfflineServices => Services.Where(x => !x.IsOnline).ToList();
        public List<ServiceHealthItem> OnlineServices => Services.Where(x => x.IsOnline).ToList();
        public bool HasOfflineServices => OfflineServices.Any();
    }

    public interface IServiceHealthCheckService
    {
        Task<ServiceHealthReport> CheckAndLogAsync(string triggerSource);
        ServiceHealthReport? GetLastReport();
        bool HasDefaultPageBeenChecked { get; set; }
    }
}

