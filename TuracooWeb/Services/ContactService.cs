using System.Net.Http.Json;
using System.Text.Json;

namespace TuracooWeb.Services
{
    public class ContactService
    {
        private readonly HttpClient _httpClient;

        public ContactService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ContactResult> SendContactMessage(string name, string email, string message)
        {
            try
            {
                var contactRequest = new ContactRequest
                {
                    Name = name,
                    Email = email,
                    Message = message
                };

                var response = await _httpClient.PostAsJsonAsync("api/contact/send", contactRequest);
                
                if (response.IsSuccessStatusCode)
                {
                    return new ContactResult { Success = true, Message = "Din besked blev sendt" };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new ContactResult { Success = false, Message = "Fejl ved afsendelse: " + errorContent };
                }
            }
            catch (Exception ex)
            {
                return new ContactResult { Success = false, Message = "Der opstod en fejl: " + ex.Message };
            }
        }
    }

    public class ContactRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class ContactResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}