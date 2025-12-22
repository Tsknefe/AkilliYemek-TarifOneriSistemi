using AkilliYemekTarifOneriSistemi.Models;

namespace AkilliYemekTarifOneriSistemi.Services.Interfaces
{
    public interface ISupportChatService
    {
        Task<List<SupportMessage>> GetThreadAsync(string userId, string threadKey = "default", int take = 200);
        Task SendAsync(string userId, string senderRole, string message, string? userEmail = null, string threadKey = "default");
        Task<List<(string userId, string? userEmail, DateTime lastAt)>> GetActiveThreadsAsync(int take = 100);
    }
}
