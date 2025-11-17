using FluentMigrator;

namespace WeatherApp.Migrations.Migrations
{
    [Migration(001)]
    public class CreateCitiesTable : Migration
    {
        public override void Up()
        {
            Create.Table("Cities")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString(100).NotNullable()
                .WithColumn("Country").AsString(100).NotNullable()
                .WithColumn("Latitude").AsDecimal(9, 6).NotNullable()
                .WithColumn("Longitude").AsDecimal(9, 6).NotNullable()
                .WithColumn("TimeZone").AsString(50).Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("UpdatedAt").AsDateTime().Nullable();

            Create.Index("IX_Cities_Name_Country")
                .OnTable("Cities")
                .OnColumn("Name").Ascending()
                .OnColumn("Country").Ascending()
                .WithOptions().Unique();
        }

        public override void Down()
        {
            Delete.Index("IX_Cities_Name_Country").OnTable("Cities");
            Delete.Table("Cities");
        }
    }
}
