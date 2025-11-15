using FluentMigrator;

namespace WeatherApp.Migrations.Migrations
{
    [Migration(002)]
    public class CreateWeatherRecordsTable : Migration
    {
        public override void Up()
        {
            Create.Table("WeatherRecords")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("CityId").AsInt32().NotNullable()
                .WithColumn("ObservationTime").AsDateTime().NotNullable()
                .WithColumn("Temperature").AsDecimal(5, 2).NotNullable()
                .WithColumn("FeelsLike").AsDecimal(5, 2).Nullable()
                .WithColumn("Humidity").AsInt32().NotNullable()
                .WithColumn("WindSpeed").AsDecimal(5, 2).Nullable()
                .WithColumn("WindDirection").AsString(10).Nullable()
                .WithColumn("Pressure").AsDecimal(6, 2).Nullable()
                .WithColumn("Condition").AsString(50).Nullable()
                .WithColumn("Description").AsString(200).Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable();

            Create.ForeignKey("FK_WeatherRecords_Cities")
                .FromTable("WeatherRecords").ForeignColumn("CityId")
                .ToTable("Cities").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);

            Create.Index("IX_WeatherRecords_CityId_ObservationTime")
                .OnTable("WeatherRecords")
                .OnColumn("CityId").Ascending()
                .OnColumn("ObservationTime").Descending();
        }

        public override void Down()
        {
            Delete.Index("IX_WeatherRecords_CityId_ObservationTime").OnTable("WeatherRecords");
            Delete.ForeignKey("FK_WeatherRecords_Cities").OnTable("WeatherRecords");
            Delete.Table("WeatherRecords");
        }
    }
}