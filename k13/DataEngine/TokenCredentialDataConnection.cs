// <copyright file="TokenCredentialDataConnection.cs" company="SouthState Bank, N.A.">
// Copyright (c) SouthState Bank, N.A.. All rights reserved.
// </copyright>

namespace DataEngine;

using System.Data;
using System.Data.SqlClient;
using Azure.Core;
using CMS.DataEngine;
using CMS.DataProviderSQL;

/// <summary>
/// Implements a <see cref="IDataConnection"/> that uses Azure OAuth tokens for authentication.
/// </summary>
/// <param name="connectionString">The database connection string.</param>
/// <param name="credential">The Azure token credential.</param>
internal sealed class TokenCredentialDataConnection(string? connectionString, TokenCredential credential)
    : DataConnection(connectionString)
{
    private static readonly string[] Scopes =
    [
        "https://database.windows.net/.default",
    ];

    /// <inheritdoc/>
    protected override IDbConnection CreateNativeConnection()
    {
        var tokenRequestContext = new TokenRequestContext(Scopes);
        var token = credential.GetToken(tokenRequestContext, default);

        var connection = (SqlConnection)base.CreateNativeConnection();
        connection.AccessToken = token.Token;
        return connection;
    }
}
