using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelIdToReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HotelId",
                table: "Reviews",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE "Reviews"
                SET "HotelId" = (
                    SELECT "Id"
                    FROM "Hotels"
                    ORDER BY "Id"
                    LIMIT 1
                )
                WHERE "HotelId" IS NULL;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "Reviews",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_HotelId",
                table: "Reviews",
                column: "HotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Hotels_HotelId",
                table: "Reviews",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Hotels_HotelId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_HotelId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "Reviews");
        }
    }
}
