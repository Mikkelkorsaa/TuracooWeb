// Program.cs
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable CORS if needed
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", 
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Define API endpoints
app.MapGet("/", () => "Hello from Minimal API!");

app.MapPost("/send-email", async (ContactModel contact) =>
{
    try
    {
        // Log the received contact information
        Console.WriteLine($"Contact form received: {contact.Name}, {contact.Email}, {contact.Message}");
        
        // Here you would implement your email sending logic
        // This is a placeholder for the actual email sending implementation
        // You'll need to configure your SMTP settings in appsettings.json
        
        // Uncomment and configure this code when ready to implement email sending
        
        var smtpSettings = app.Configuration.GetSection("SmtpSettings");
        
        // Curanet doesn't use authentication for SMTP from webhotel
        var smtpClient = new SmtpClient(smtpSettings["Server"])
        {
            Port = int.Parse(smtpSettings["Port"]),
            EnableSsl = bool.Parse(smtpSettings["EnableSsl"])
            // No credentials needed - Curanet authenticates based on webhotel source
        };

        // Create the email message
        var mailMessage = new MailMessage
        {
            // From address should be your verified domain email
            From = new MailAddress(smtpSettings["FromEmail"], smtpSettings["FromName"]),
            
            // Set the reply-to as the user's email so replies go directly to them
            ReplyToList = { new MailAddress(contact.Email, contact.Name) },
            
            Subject = $"New Contact Form Message from {contact.Name}",
            Body = $"You have received a new message from your website contact form:\n\n" +
                   $"Name: {contact.Name}\n" +
                   $"Email: {contact.Email}\n\n" +
                   $"Message:\n{contact.Message}\n\n" +
                   $"To reply directly to this person, simply reply to this email.",
            IsBodyHtml = false
        };
        
        // Send to the contact email
        mailMessage.To.Add(smtpSettings["FromEmail"]);
        
        await smtpClient.SendMailAsync(mailMessage);
        
        // For now, we'll just return success without actually sending
        return Results.Ok(new { success = true, message = "Email received successfully" });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending email: {ex.Message}");
        return Results.Problem(
            detail: "There was an error processing your request.",
            statusCode: (int)HttpStatusCode.InternalServerError
        );
    }
});

app.Run();

// Contact Model
public class ContactModel
{
    [Required(ErrorMessage = "Navn er påkrævet")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email er påkrævet")]
    [EmailAddress(ErrorMessage = "Ugyldig email adresse")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Besked er påkrævet")]
    public string Message { get; set; } = string.Empty;
}