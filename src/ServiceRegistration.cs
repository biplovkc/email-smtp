using System;
using System.Net.Mail;
using System.Security.Authentication;
using Microsoft.Extensions.DependencyInjection;
using OneOf.Types;
using OneOf;
using Polly;

namespace Biplov.Email.Smtp;

public static class ServiceRegistration
{
    public static void AddSmtpEmailService(this IServiceCollection services, IAsyncPolicy<OneOf<AuthenticationException, SmtpException, TimeoutException, Exception>> asyncPolicy)
    {
        services.AddSingleton<IAsyncPolicy<OneOf<Success, FormatException, AuthenticationException, SmtpException, TimeoutException, Exception>>>(asyncPolicy);
        services.AddTransient<IEmailService, SmtpEmailService>();
    }
}