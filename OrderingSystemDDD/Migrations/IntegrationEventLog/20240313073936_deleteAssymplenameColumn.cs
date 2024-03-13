using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderingSystemDDD.Migrations.IntegrationEventLog
{
    /// <inheritdoc />
    public partial class deleteAssymplenameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "eventAssymblyName",
                table: "IntegrationEventLog");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "eventAssymblyName",
                table: "IntegrationEventLog",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
