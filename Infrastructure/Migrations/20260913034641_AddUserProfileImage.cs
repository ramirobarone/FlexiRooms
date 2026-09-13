using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserProfileImageUserId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserProfileImages",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<byte[]>(type: "bytea", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfileImages", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserProfileImages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserProfileImageUserId",
                table: "AspNetUsers",
                column: "UserProfileImageUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserProfileImages_UserProfileImageUserId",
                table: "AspNetUsers",
                column: "UserProfileImageUserId",
                principalTable: "UserProfileImages",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserProfileImages_UserProfileImageUserId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "UserProfileImages");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserProfileImageUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserProfileImageUserId",
                table: "AspNetUsers");
        }
    }
}
