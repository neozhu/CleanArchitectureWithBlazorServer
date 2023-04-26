using Microsoft.AspNetCore.Builder;
using Serilog.Events;
using Serilog;
using CleanArchitecture.Blazor.Application.Common.Configurations;
using Microsoft.Extensions.Configuration;
using CleanArchitecture.Blazor.Infrastructure.Constants.Database;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using NpgsqlTypes;
using Serilog.Sinks.PostgreSQL;

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
                .WriteTo.Console()
                .WriteToDatabase(context.Configuration)
    );
    }
    private static void WriteToDatabase(this LoggerConfiguration serilogConfig,IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            var dbProvider = configuration.GetValue<string>($"{nameof(DatabaseSettings)}:{nameof(DatabaseSettings.DBProvider)}");
            var connectionString = configuration.GetValue<string>($"{nameof(DatabaseSettings)}:{nameof(DatabaseSettings.ConnectionString)}");
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
    }
    private static void WriteToSqlServer(LoggerConfiguration serilogConfig, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        var sinkOpts = new MSSqlServerSinkOptions
        {
            TableName = "Loggers", SchemaName = "dbo", AutoCreateSqlDatabase = false,
            AutoCreateSqlTable = false,
            BatchPostingLimit = 100,
            BatchPeriod = new TimeSpan(0, 0, 20)
        };

        var columnOpts = new Serilog.Sinks.MSSqlServer.ColumnOptions();
        columnOpts.Store.Add(StandardColumn.LogEvent);
        columnOpts.AdditionalColumns = new Collection<SqlColumn>
        {  
            new SqlColumn{ColumnName = "ClientIP", PropertyName = "ClientIP", DataType = SqlDbType.NVarChar, DataLength = 64},
            new SqlColumn{ColumnName = "UserName",PropertyName = "UserName", DataType = SqlDbType.NVarChar, DataLength=64},
            new SqlColumn{ColumnName = "ClientAgent", PropertyName = "ClientAgent", DataType = SqlDbType.NVarChar, DataLength = -1},
        };
        columnOpts.LogEvent.DataLength = 2048;
        columnOpts.PrimaryKey = columnOpts.Id;
        columnOpts.TimeStamp.NonClusteredIndex = true;

        serilogConfig.WriteTo.Async(wt => wt.MSSqlServer(
            connectionString: connectionString,
            sinkOptions: sinkOpts,
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
            {"Message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
            {"MessageTemplate", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
            {"Level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
            {"TimeStamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
            {"Exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
            {"Properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Varchar) },
            {"LogEvent", new LogEventSerializedColumnWriter(NpgsqlDbType.Varchar) },
            {"UserName", new LogEventSerializedColumnWriter(NpgsqlDbType.Varchar) },
            {"ClientIP", new LogEventSerializedColumnWriter(NpgsqlDbType.Varchar) },
            {"ClientAgent", new LogEventSerializedColumnWriter(NpgsqlDbType.Varchar) },

        };
        serilogConfig.WriteTo.Async(wt => wt.PostgreSQL(
            connectionString: connectionString,
            tableName: tableName,
            columnOptions: columnOptions
        ));
    }
    private static void WriteToSqLite(LoggerConfiguration serilogConfig, string? connectionString)
    {
        if (!string.IsNullOrEmpty(connectionString))
        {

        }
    }
}
