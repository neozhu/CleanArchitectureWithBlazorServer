using System.Collections.ObjectModel;
using System.Data;
using CleanArchitecture.Blazor.Application.Common.Configurations;
using CleanArchitecture.Blazor.Infrastructure.Constants.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using NpgsqlTypes;
using Serilog;
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
                .Enrich.WithClientIp()
                .Enrich.WithClientAgent()
                .WriteTo.Async(wt => wt.File("./log/log-.txt", rollingInterval: RollingInterval.Day))
                .WriteTo.Async(wt => wt.Console())
                .WriteToDatabase(context.Configuration)
        );
    }

    private static void WriteToDatabase(this LoggerConfiguration serilogConfig, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            return;
        }

        string? dbProvider =
            configuration.GetValue<string>($"{nameof(DatabaseSettings)}:{nameof(DatabaseSettings.DBProvider)}");
        string? connectionString =
            configuration.GetValue<string>(
                $"{nameof(DatabaseSettings)}:{nameof(DatabaseSettings.ConnectionString)}");
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

    private static void WriteToSqlServer(LoggerConfiguration serilogConfig, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        MSSqlServerSinkOptions sinkOpts = new()
        {
            TableName = "Loggers",
            SchemaName = "dbo",
            AutoCreateSqlDatabase = false,
            AutoCreateSqlTable = false,
            BatchPostingLimit = 100,
            BatchPeriod = new TimeSpan(0, 0, 20)
        };

        ColumnOptions columnOpts = new();
        columnOpts.Store.Add(StandardColumn.LogEvent);
        columnOpts.AdditionalColumns = new Collection<SqlColumn>
        {
            new() { ColumnName = "ClientIP", PropertyName = "ClientIp", DataType = SqlDbType.NVarChar, DataLength = 64 },
            new() { ColumnName = "UserName", PropertyName = "UserName", DataType = SqlDbType.NVarChar, DataLength = 64 },
            new() { ColumnName = "ClientAgent", PropertyName = "ClientAgent", DataType = SqlDbType.NVarChar, DataLength = -1 }
        };
        columnOpts.LogEvent.DataLength = 2048;
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
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

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
            { "UserName", new SinglePropertyColumnWriter("UserName", PropertyWriteMethod.ToString, NpgsqlDbType.Varchar) }, 
            { "ClientIP", new SinglePropertyColumnWriter("ClientIp", PropertyWriteMethod.ToString, NpgsqlDbType.Varchar) },
            { "ClientAgent", new SinglePropertyColumnWriter("ClientAgent", PropertyWriteMethod.ToString, NpgsqlDbType.Varchar) }
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
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        const string tableName = "Loggers";
        serilogConfig.WriteTo.Async(wt => wt.SQLite(
            connectionString,
            tableName,
            LogEventLevel.Information
        ));
    }
}