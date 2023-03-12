// Global using directives

global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Net;
global using System.Net.Mail;
global using System.Security.Authentication;
global using System.Threading;
global using System.Threading.Tasks;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
global using OneOf;
global using OneOf.Types;
global using Polly;
global using SendOutcome = OneOf.OneOf<OneOf.Types.Success, System.FormatException, System.Security.Authentication.AuthenticationException, System.Net.Mail.SmtpException, System.TimeoutException, System.Exception>;