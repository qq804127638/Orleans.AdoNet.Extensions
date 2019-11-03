﻿using Orleans.Configuration;

namespace Orleans.AdoNet.Clustering.ClientOptions
{
    /// <summary>Options for ADO.NET Sql Server clustering</summary>
    public class SqlServerClusteringClientOptions : AdoNetClusteringClientOptions
    {

        /// <summary>
        /// The invariant name of the connector for membership's database.
        /// </summary>
        public new string Invariant { get; private set; } = AdoNetInvariants.InvariantNameSqlServer;
    }
}