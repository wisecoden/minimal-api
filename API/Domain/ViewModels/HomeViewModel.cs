namespace MinimalApi.Domain.ViewModels
{
    public class HomeViewModel
    {
        public string Message { get; set; } = "Welcome to Minimal API!";
        public string documentationUrl { get => "/swagger"; }
         public string Version { get; set; } = "v1";
    }
}