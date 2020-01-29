﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.AdoNet.Clustering;
using Orleans.AdoNet.SqlServer.Clustering;
using Orleans.AdoNet.SqlServer.Persistence;
using Orleans.AdoNet.SqlServer.Reminder;
using Orleans.Grains;

namespace Orleans.Server
{
    class Program
    {
        static Task Main(string[] args)
        {
            Console.Title = typeof(Program).Namespace;

            // define the cluster configuration
            return Host.CreateDefaultBuilder()
                .UseOrleans((builder) =>
                    {

                        var invariant = "System.Data.SqlClient";
                        var connectionString =
                            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Hello.Orleans;Integrated Security=True;Pooling=False;Max Pool Size=200;MultipleActiveResultSets=True";
                        //use AdoNet for clustering 

                        builder.UseSqlServerClustering(option =>
                        {
                            option.ConnectionString = connectionString;
                        }).Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "Hello.Orleans";
                            options.ServiceId = "Hello.Orleans";
                        }).ConfigureEndpoints(new Random().Next(10001, 20000), new Random().Next(20001, 30000));

                        //use AdoNet for reminder service
                        builder.UseSqlServerReminderService(options =>
                        {
                            options.ConnectionString = connectionString;
                            options.Invariant = invariant;
                        }
                        );

                        //use AdoNet for Persistence
                        builder.AddSqlServerGrainStorageAsDefault(options =>
                       {
                           options.Invariant = invariant;
                           options.ConnectionString = connectionString;
                           options.UseJsonFormat = true;
                       });

                        builder.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IHelloGrain).Assembly).WithReferences());
                    }
                )
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                })
                .ConfigureLogging(builder => { builder.AddConsole(); })
                .RunConsoleAsync();
        }
    }
}
