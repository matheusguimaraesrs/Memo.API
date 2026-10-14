using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class SimplificaSenha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Sellers");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Sellers");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Sellers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_Login",
                table: "Sellers",
                column: "Login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sellers_Login",
                table: "Sellers");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Sellers");

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "Sellers",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "Sellers",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
