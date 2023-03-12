# SmtpEmail Library

<h3 align="center">

[![NuGet](https://img.shields.io/nuget/v/Biplov.Email.Smtp)](https://www.nuget.org/packages/Biplov.Email.Smtp/)
[![Downloads](https://img.shields.io/nuget/dt/Biplov.Email.Smtp)](https://www.nuget.org/packages/Biplov.Email.Smtp/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)

</h3>

This library provides a simple way to send fault tolerant SMTP emails in .NET applications using [OneOf](https://github.com/mcintyre321/OneOf)

## Features

- Supports sending plain text and HTML emails.
- Supports sending emails to multiple recipients and adding CC and BCC recipients.
- Uses OneOf return types.

## Installation

To install the library, run the following command in the Package Manager Console:

```dotnetcli
dotnet add package Biplov.Email.Smtp --version 0.1.3
```

## Usage

To use the library first configure the SmtpCredentials in `appsettings.json` then via services then invoke the `AddSmtpEmailService` method passing the IAsncPolicy of Polly.

```json

  "SmtpCredentials": {
    "Host": "<Your Smtp host>",
    "Port": <Your smtp port>,
    "Username": "<Your smtp username>",
    "Password": "<Your smtp password>",
    "UseSsl": <true or false>,
    "Timeout": <timeout in milliseconds>
}
```

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.Configure<SmtpCredentials>(Configuration.GetSection("SmtpCredentials"));
    
    var asyncPolicy = Policy
        .TimeoutAsync(TimeSpan.FromSeconds(30))
        .WrapAsync(Policy
            .Handle<TimeoutException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
            ));

    services.AddSmtpEmailService(asyncPolicy);
}

```

## Example usage

```csharp
var emailService = serviceProvider.GetRequiredService<IEmailService>();

var result = await emailService.SendAsync(
    "Hello World",
    "This is a test email from SmtpEmail",
    new List<string> { "recipient@example.com" },
    "sender@example.com"
);

result.Match(
    success => Console.WriteLine("Email sent successfully."),
    formatException => Console.WriteLine($"Email format exception: {formatException.Message}"),
    authenticationException => Console.WriteLine($"Authentication failed: {authenticationException.Message}"),
    smtpException => Console.WriteLine($"SMTP error: {smtpException.Message}"),
    timeoutException => Console.WriteLine($"SMTP connection timed out: {timeoutException.Message}"),
    exception => Console.WriteLine($"Unhandled exception: {exception.Message}")
);
```