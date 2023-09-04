namespace Mediator7Hangfire.ViewModels;

public class HelloMessageDto
{
    public DateTime Date { get; set; } = DateTime.Now;

    public string? Message { get; set; }
}