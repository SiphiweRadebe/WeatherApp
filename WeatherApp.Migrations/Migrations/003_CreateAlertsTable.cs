using FluentMigrator;

namespace WeatherApp.Migrations.Migrations
{
    [Migration(003)]
    public class CreateAlertsTable : Migration
    {
        public override void Up()
        {
            Create.Table("Alerts")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Title").AsString(200).NotNullable()
                .WithColumn("Description").AsString(1000).NotNullable()
                .WithColumn("Severity").AsString(20).NotNullable()
                .WithColumn("AlertType").AsString(50).NotNullable()
                .WithColumn("StartTime").AsDateTime().NotNullable()
                .WithColumn("EndTime").AsDateTime().Nullable()
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("UpdatedAt").AsDateTime().Nullable();

            Create.Index("IX_Alerts_IsActive")
                .OnTable("Alerts")
                .OnColumn("IsActive");

            Create.Index("IX_Alerts_AlertType")
                .OnTable("Alerts")
                .OnColumn("AlertType");
        }

        public override void Down()
        {
            Delete.Index("IX_Alerts_AlertType").OnTable("Alerts");
            Delete.Index("IX_Alerts_IsActive").OnTable("Alerts");
            Delete.Table("Alerts");
        }
    }
}
