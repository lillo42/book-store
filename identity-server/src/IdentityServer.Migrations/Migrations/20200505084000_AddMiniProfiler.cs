using System;
using FluentMigrator;
using FluentMigrator.Builders.Delete;
using FluentMigrator.Builders.Insert;
using Raven.Client.Documents.Linq;
using StackExchange.Profiling.Storage;

namespace IdentityServer.Migrations.Migrations
{
    [Migration(20200505084000)]
    public class AddMiniProfiler : Migration
    {
        public override void Up()
        {
            Create.Table("MiniProfilers")
                .WithColumn("RowId").AsInt64().Identity()
                .WithColumn("Id").AsGuid().NotNullable().Unique("IX_MiniProfilers_Id")
                .WithColumn("RootTimingId").AsGuid().Nullable()
                .WithColumn("Name").AsString(200).Nullable()
                .WithColumn("Started").AsCustom("timestamp(3)").NotNullable()
                .WithColumn("DurationMilliseconds").AsDecimal(15, 1).NotNullable()
                .WithColumn("User").AsString(100).Nullable()
                .WithColumn("HasUserViewed").AsBoolean().NotNullable()
                .WithColumn("MachineName").AsString(100).Nullable()
                .WithColumn("CustomLinksJson").AsString().Nullable()
                .WithColumn("ClientTimingsRedirectCount").AsInt32().Nullable();

            Create.Index("IX_MiniProfilers_User_HasUserViewed_Includes")
                .OnTable("MiniProfilers")
                .OnColumn("User").Ascending()
                .OnColumn("HasUserViewed");

            Create.Table("MiniProfilerTimings")
                .WithColumn("RowId").AsInt64().Identity()
                .WithColumn("Id").AsGuid().NotNullable().Unique("IX_MiniProfilerTimings_Id")
                .WithColumn("MiniProfilerId").AsGuid().NotNullable().Indexed("IX_MiniProfilerTiming_MiniProfilerId")
                .WithColumn("ParentTimingId").AsGuid().Nullable()
                .WithColumn("Name").AsString(200).NotNullable()
                .WithColumn("DurationMilliseconds").AsDecimal(15, 3).NotNullable()
                .WithColumn("StartMilliseconds").AsDecimal(15, 3).NotNullable()
                .WithColumn("IsRoot").AsBoolean().NotNullable()
                .WithColumn("Depth").AsInt16().NotNullable()
                .WithColumn("CustomTimingsJson").AsString().Nullable();

            Create.Table("MiniProfilerClientTimings")
                .WithColumn("RowId").AsInt64().Identity()
                .WithColumn("Id").AsGuid().NotNullable().Unique("IX_MiniProfilerClientTimings_Id")
                .WithColumn("MiniProfilerId").AsGuid().NotNullable().Indexed("IX_MiniProfilerClientTimings_MiniProfilerId")
                .WithColumn("Name").AsString(200).NotNullable()
                .WithColumn("Start").AsDecimal(9, 3).NotNullable()
                .WithColumn("Duration").AsDecimal(9, 3).NotNullable();

        }

        public override void Down()
        {
            Delete.Table("MiniProfilerClientTimings");
            Delete.Table("MiniProfilerTimings");
            Delete.Table("MiniProfilers");
        }
    }
}