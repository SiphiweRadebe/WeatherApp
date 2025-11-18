using FluentMigrator;

namespace WeatherApp.Migrations.Migrations
{
    [Migration(004)]
    public class CreateCityAlertsTable : Migration
    {
        public override void Up()
        {
            Create.Table("CityAlerts")
                .WithColumn("CityId").AsInt32().NotNullable()
                .WithColumn("AlertId").AsInt32().NotNullable()
                .WithColumn("AssociatedAt").AsDateTime().NotNullable()
                .WithColumn("NotificationSent").AsBoolean().NotNullable().WithDefaultValue(false);

            Create.PrimaryKey("PK_CityAlerts")
                .OnTable("CityAlerts")
                .Columns("CityId", "AlertId");

            Create.ForeignKey("FK_CityAlerts_Cities")
                .FromTable("CityAlerts").ForeignColumn("CityId")
                .ToTable("Cities").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);

            Create.ForeignKey("FK_CityAlerts_Alerts")
                .FromTable("CityAlerts").ForeignColumn("AlertId")
                .ToTable("Alerts").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);

            Create.Index("IX_CityAlerts_CityId")
                .OnTable("CityAlerts")
                .OnColumn("CityId");

            Create.Index("IX_CityAlerts_AlertId")
                .OnTable("CityAlerts")
                .OnColumn("AlertId");
        }

        
        /// Rollback: dismantles the city-alert relationship table and all its constraints.
        
        public override void Down()
        {
            Delete.Index("IX_CityAlerts_AlertId").OnTable("CityAlerts");
            Delete.Index("IX_CityAlerts_CityId").OnTable("CityAlerts");
            Delete.ForeignKey("FK_CityAlerts_Alerts").OnTable("CityAlerts");
            Delete.ForeignKey("FK_CityAlerts_Cities").OnTable("CityAlerts");
            Delete.PrimaryKey("PK_CityAlerts").FromTable("CityAlerts");
            Delete.Table("CityAlerts");
        }
    }
}