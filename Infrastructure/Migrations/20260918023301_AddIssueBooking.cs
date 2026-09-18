using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIssueBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "Issues",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Issues_BookingId",
                table: "Issues",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_IdRoom",
                table: "Bookings",
                column: "IdRoom");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Rooms_IdRoom",
                table: "Bookings",
                column: "IdRoom",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Bookings_BookingId",
                table: "Issues",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Rooms_IdRoom",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Bookings_BookingId",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_BookingId",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_IdRoom",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Issues");
        }
    }
}
