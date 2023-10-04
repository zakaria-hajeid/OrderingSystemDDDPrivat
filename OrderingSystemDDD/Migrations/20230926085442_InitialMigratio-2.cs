using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderingSystemDDD.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigratio2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_orders_BuyerId",
                schema: "ordering",
                table: "orders",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_PaymentMethodId",
                schema: "ordering",
                table: "orders",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_buyers_BuyerId",
                schema: "ordering",
                table: "orders",
                column: "BuyerId",
                principalSchema: "ordering",
                principalTable: "buyers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_paymentmethods_PaymentMethodId",
                schema: "ordering",
                table: "orders",
                column: "PaymentMethodId",
                principalSchema: "ordering",
                principalTable: "paymentmethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_buyers_BuyerId",
                schema: "ordering",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_paymentmethods_PaymentMethodId",
                schema: "ordering",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_BuyerId",
                schema: "ordering",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_PaymentMethodId",
                schema: "ordering",
                table: "orders");
        }
    }
}
