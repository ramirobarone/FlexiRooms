using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FK_Hotel_user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityNumber",
                table: "Hotels",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 1,
                column: "IdentityNumber",
                value: null);

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 2,
                column: "IdentityNumber",
                value: null);

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 3,
                column: "IdentityNumber",
                value: null);

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 4,
                column: "IdentityNumber",
                value: null);

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 5,
                column: "IdentityNumber",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_IdentityNumber",
                table: "Hotels",
                column: "IdentityNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_AspNetUsers_IdentityNumber",
                table: "Hotels",
                column: "IdentityNumber",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_AspNetUsers_IdentityNumber",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_IdentityNumber",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "IdentityNumber",
                table: "Hotels");
        }
    }
}
