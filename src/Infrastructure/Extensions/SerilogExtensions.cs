using System.Collections.ObjectModel;
using System.Data;
using CleanArchitecture.Blazor.Application.Common.Configurations;
using CleanArchitecture.Blazor.Infrastructure.Constants.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using NpgsqlTypes;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using ColumnOptions = Serilog.Sinks.MSSqlServer.ColumnOptions;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public static class SerilogExtensions
{
    public static void RegisterSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
                .MinimumLevel.Override("Serilog", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Update", LogEventLevel.Error)
                .MinimumLevel.Override("Hangfire.BackgroundJobServer", LogEventLevel.Error)
                .MinimumLevel.Override("Hangfire.Server.BackgroundServerProcess", LogEventLevel.Error)
                .MinimumLevel.Override("Hangfire.Server.ServerHeartbeatProcess", LogEventLevel.Error)
                .MinimumLevel.Override("Hangfire.Processing.BackgroundExecution", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .Enrich.WithUtcTime()
                .WriteTo.Async(wt => wt.File("./log/log-.txt", rollingInterval: RollingInterval.Day))
                .WriteTo.Async(wt =>
                    wt.Console(
                        outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3} {ClientIp}] {Message:lj}{NewLine}{Exception}"))
                .ApplyConfigPreferences(context.Configuration)
        );
    }

    private static void ApplyConfigPreferences(this LoggerConfiguration serilogConfig, IConfiguration configuration)
    {
        EnrichWithClientInfo(serilogConfig, configuration);
        WriteToDatabase(serilogConfig, configuration);
    }

    private static void WriteToDatabase(LoggerConfiguration serilogConfig, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("UseInMemoryDatabase")) return;

        var dbProvider =
            configuration.GetValue<string>($"{nameof(DatabaseSettings)}:{nameof(DatabaseSettings.DBProvider)}");
        var connectionString =
            configuration.GetValue<string>($"{nameof(DatabaseSettings)}:{nameof(DatabaseSettings.ConnectionString)}");
        switch (dbProvider)
        {
            case DbProviderKeys.SqlServer:
                WriteToSqlServer(serilogConfig, connectionString);
                break;
            case DbProviderKeys.Npgsql:
                WriteToNpgsql(serilogConfig, connectionString);
                break;
            case DbProviderKeys.SqLite:
                WriteToSqLite(serilogConfig, connectionString);
                break;
        }
    }

    private static void EnrichWithClientInfo(LoggerConfiguration serilogConfig, IConfiguration configuration)
    {
        var privacySettings = configuration.GetRequiredSection(PrivacySettings.Key).Get<PrivacySettings>();

        if (privacySettings == null) return;
        if (privacySettings.LogClientIpAddresses) serilogConfig.Enrich.WithClientIp();
        if (privacySettings.LogClientAgents) serilogConfig.Enrich.WithClientAgent();
    }

    private static void WriteToSqlServer(LoggerConfiguration serilogConfig, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString)) return;

        MSSqlServerSinkOptions sinkOpts = new()
        {
            TableName = "Loggers",
            SchemaName = "dbo",
            AutoCreateSqlDatabase = false,
            AutoCreateSqlTable = false,
            BatchPostingLimit = 100,
            BatchPeriod = new TimeSpan(0, 0, 20)
        };

        ColumnOptions columnOpts = new()
        {
            Store = new Collection<StandardColumn>
            {
                StandardColumn.Id,
                StandardColumn.TimeStamp,
                StandardColumn.Level,
                StandardColumn.LogEvent,
                StandardColumn.Exception,
                StandardColumn.Message,
                StandardColumn.MessageTemplate,
                StandardColumn.Properties
            },
            AdditionalColumns = new Collection<SqlColumn>
            {
                new()
                {
                    ColumnName = "ClientIP", PropertyName = "ClientIp", DataType = SqlDbType.NVarChar, DataLength = 64
                },
                new()
                {
                    ColumnName = "UserName", PropertyName = "UserName", DataType = SqlDbType.NVarChar, DataLength = 64
                },
                new()
                {
                    ColumnName = "ClientAgent", PropertyName = "ClientAgent", DataType = SqlDbType.NVarChar,
                    DataLength = -1
                }
            },
            TimeStamp = { ConvertToUtc = true, ColumnName = "TimeStamp" },
            LogEvent = { DataLength = 2048 }
        };
        columnOpts.PrimaryKey = columnOpts.Id;
        columnOpts.TimeStamp.NonClusteredIndex = true;

        serilogConfig.WriteTo.Async(wt => wt.MSSqlServer(
            connectionString,
            sinkOpts,
            columnOptions: columnOpts
        ));
    }

    private static void WriteToNpgsql(LoggerConfiguration serilogConfig, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString)) return;

        const string tableName = "Loggers";
        //Used columns (Key is a column name) 
        //Column type is writer's constructor parameter
        IDictionary<string, ColumnWriterBase> columnOptions = new Dictionary<string, ColumnWriterBase>
        {
            { "Message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
            { "MessageTemplate", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
            { "Level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
            { "TimeStamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
            { "Exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
            { "Properties", new PropertiesColumnWriter(NpgsqlDbType.Varchar) },
            { "LogEvent", new LogEventSerializedColumnWriter(NpgsqlDbType.Varchar) },
            { "UserName", new SinglePropertyColumnWriter("UserName", PropertyWriteMethod.Raw, NpgsqlDbType.Varchar) },
            { "ClientIP", new SinglePropertyColumnWriter("ClientIp", PropertyWriteMethod.Raw, NpgsqlDbType.Varchar) },
            {
                "ClientAgent",
                new SinglePropertyColumnWriter("ClientAgent", PropertyWriteMethod.ToString, NpgsqlDbType.Varchar)
            }
        };
        serilogConfig.WriteTo.Async(wt => wt.PostgreSQL(
            connectionString,
            tableName,
            columnOptions,
            LogEventLevel.Information,
            needAutoCreateTable: false,
            schemaName: "public",
            useCopy: false
        ));
    }

    private static void WriteToSqLite(LoggerConfiguration serilogConfig, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString)) return;

        const string tableName = "Loggers";
        serilogConfig.WriteTo.Async(wt => wt.SQLite(
            connectionString,
            tableName,
            LogEventLevel.Information
        ));
    }


    public static LoggerConfiguration WithUtcTime(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        return enrichmentConfiguration.With<UtcTimestampEnricher>();
    }
}

internal class UtcTimestampEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory pf)
    {
        logEvent.AddOrUpdateProperty(pf.CreateProperty("TimeStamp", logEvent.Timestamp.UtcDateTime));
    }
}