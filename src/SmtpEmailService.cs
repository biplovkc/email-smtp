using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;

namespace Biplov.Email.Smtp;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpCredentials _smtpCredentials;

    public SmtpEmailService(IOptions<SmtpCredentials> smtpCredentials)
    {
        _smtpCredentials = smtpCredentials is null ? throw new ArgumentNullException(nameof(smtpCredentials)) : smtpCredentials.Value;
    }

    public async Task<OneOf<Success, FormatException, AuthenticationException, SmtpException, TimeoutException, Exception>> SendAsync(string subject, string message, List<string> receivers, string sender, List<string> bcc = null, string replyTo = null,
        bool isBodyHtml = false, string correlationId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(subject))
                return new FormatException("Subject cannot be empty");

            if (string.IsNullOrWhiteSpace(message))
                return new FormatException("Message cannot be empty");

            if (!sender.IsValidEmail())
                return new FormatException("Invalid sender email");

            if (receivers.Count(x => !x.IsValidEmail()) > 0)
                return new FormatException("Contains Invalid receiver email");

            if (bcc is not null && bcc.Count > 0 && bcc.Count(x => !x.IsValidEmail()) > 0)
                return new FormatException("Contains Invalid bcc email");

            if (string.IsNullOrWhiteSpace(_smtpCredentials.Host)
             || string.IsNullOrWhiteSpace(_smtpCredentials.Username)
             || string.IsNullOrWhiteSpace(_smtpCredentials.Password))
                return new FormatException("Invalid credentials");

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(sender),
                Subject = subject,
                Body = message,
                IsBodyHtml = isBodyHtml,
                Priority = MailPriority.Normal,
            };

            foreach (var receiver in receivers)
                mailMessage.To.Add(new MailAddress(receiver));

            if (bcc is not null)
                foreach (var bccAddress in bcc)
                    mailMessage.Bcc.Add(new MailAddress(bccAddress));

            if (!string.IsNullOrWhiteSpace(replyTo))
                mailMessage.ReplyToList.Add(new MailAddress(replyTo));

            using var smtpClient = new SmtpClient(_smtpCredentials.Host, _smtpCredentials.Port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpCredentials.Username, _smtpCredentials.Password),
                EnableSsl = _smtpCredentials.UseSsl,
                Timeout = _smtpCredentials.Timeout
            };

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);

            return new Success();
        }
        catch (FormatException ex)
        {
            return new FormatException($"Invalid email address: {ex.Message}");
        }
        catch (AuthenticationException ex)
        {
            return new AuthenticationException($"Authentication failed: {ex.Message}");
        }
        catch (SmtpException ex)
        {
            return new SmtpException($"SMTP error: {ex.Message}");
        }
        catch (TimeoutException ex)
        {
            return new TimeoutException($"SMTP connection timed out: {ex.Message}");
        }
        catch (Exception ex)
        {
            return ex;
        }

    }

    public Task<OneOf<Success, FormatException, AuthenticationException, SmtpException, TimeoutException, Exception>> SendFromTemplateAsync(string templateId, string subject, string message, Dictionary<string, string> templateData, List<string> receivers,
        string sender, List<string> bcc = null, string replyTo = null, bool isBodyHtml = false, string correlationId = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}