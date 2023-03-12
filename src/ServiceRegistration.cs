namespace Biplov.Email.Smtp;

public static class ServiceRegistration
{
    public static void AddSmtpEmailService(this IServiceCollection services, IAsyncPolicy asyncPolicy)
    {
        services.AddSingleton(asyncPolicy);
        services.AddTransient<IEmailService, SmtpEmailService>();
    }
}