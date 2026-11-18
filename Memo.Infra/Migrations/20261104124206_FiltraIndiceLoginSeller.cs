using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class FiltraIndiceLoginSeller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sellers_Login",
                table: "Sellers");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_Login",
                table: "Sellers",
                column: "Login",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sellers_Login",
                table: "Sellers");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_Login",
                table: "Sellers",
                column: "Login",
                unique: true);
        }
    }
}
