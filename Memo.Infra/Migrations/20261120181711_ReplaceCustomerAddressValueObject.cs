using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceCustomerAddressValueObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressNumber",
                table: "Customers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "S/N");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Customers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Não informado");

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "Customers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Não informado");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Customers",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "00000000");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Customers",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "SP");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Customers",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                """
                UPDATE "Customers"
                SET "Street" = "Address"
                WHERE "Address" IS NOT NULL;
                """);

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Customers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Customers",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
