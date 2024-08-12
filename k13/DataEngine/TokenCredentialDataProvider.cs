// <copyright file="TokenCredentialDataProvider.cs" company="SouthState Bank, N.A.">
// Copyright (c) SouthState Bank, N.A.. All rights reserved.
// </copyright>

using CMS;
using CMS.Core;
using CMS.DataEngine;
using DataEngine;

[assembly: RegisterImplementation(typeof(IDataProvider), typeof(TokenCredentialDataProvider), Lifestyle = Lifestyle.Transient)]

namespace DataEngine;

using System.Data.SqlClient;
using Azure.Core;
using CMS.DataProviderSQL;

/// <summary>
/// Implements a <see cref="IDataProvider"/> that provides connections authenticated via
/// a <see cref="TokenCredential"/>.
/// </summary>
/// <param name="credential">The Azure token credential.</param>
public class TokenCredentialDataProvider(TokenCredential credential) : DataProvider
{
    /// <inheritdoc/>
    public override IDataConnection GetNewConnection(string? connectionString)
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString ?? this.ConnectionString);
        return connectionStringBuilder.IntegratedSecurity || !string.IsNullOrWhiteSpace(connectionStringBuilder.UserID) ?
            base.GetNewConnection(connectionString) :
            new TokenCredentialDataConnection(connectionString, credential);
    }
}
