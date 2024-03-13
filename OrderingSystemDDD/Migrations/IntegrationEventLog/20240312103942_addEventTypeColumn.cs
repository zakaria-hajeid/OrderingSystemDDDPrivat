using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderingSystemDDD.Migrations.IntegrationEventLog
{
    /// <inheritdoc />
    public partial class addEventTypeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventType",
                table: "IntegrationEventLog",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventType",
                table: "IntegrationEventLog");
        }
    }
}
