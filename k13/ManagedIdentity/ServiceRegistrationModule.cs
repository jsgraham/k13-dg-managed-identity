using Azure.Core;
using Azure.Identity;
using CMS;
using CMS.Core;
using CMS.DataEngine;
using ManagedIdentity;

[assembly: RegisterModule(typeof(ServiceRegistrationModule))]

namespace ManagedIdentity;

/// <summary>
/// Defines a <see cref="Module"/> that registers the assembly's services with Kentico's DI container.
/// </summary>
public class ServiceRegistrationModule()
    : Module(typeof(ServiceRegistrationModule).FullName)
{
    /// <inheritdoc/>
    protected override void OnPreInit()
    {
        base.OnPreInit();

        Service.Use<TokenCredential, DefaultAzureCredential>();
    }
}
