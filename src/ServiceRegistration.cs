namespace Biplov.Email.Smtp;

public static class ServiceRegistration
{
    public static IServiceCollection AddSmtpEmailService(this IServiceCollection services, IAsyncPolicy asyncPolicy)
    {
        services.AddSingleton(asyncPolicy);
        services.AddTransient<IEmailService, SmtpEmailService>();
        return services;
    }
}