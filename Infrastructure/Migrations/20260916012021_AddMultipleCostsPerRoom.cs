using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleCostsPerRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Costs_CostId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_CostId",
                table: "Rooms");

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Costs",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(
                """
                WITH ranked_room_costs AS (
                    SELECT "Id" AS "RoomId",
                           "CostId",
                           ROW_NUMBER() OVER (PARTITION BY "CostId" ORDER BY "Id") AS rn
                    FROM "Rooms"
                    WHERE "CostId" IS NOT NULL
                )
                UPDATE "Costs" AS c
                SET "RoomId" = ranked_room_costs."RoomId"
                FROM ranked_room_costs
                WHERE c."Id" = ranked_room_costs."CostId"
                  AND ranked_room_costs.rn = 1;
                """);

            migrationBuilder.Sql(
                """
                WITH ranked_room_costs AS (
                    SELECT "Id" AS "RoomId",
                           "CostId",
                           ROW_NUMBER() OVER (PARTITION BY "CostId" ORDER BY "Id") AS rn
                    FROM "Rooms"
                    WHERE "CostId" IS NOT NULL
                )
                INSERT INTO "Costs" ("CostPerTime", "Hour", "RoomId")
                SELECT c."CostPerTime",
                       c."Hour",
                       ranked_room_costs."RoomId"
                FROM "Costs" AS c
                INNER JOIN ranked_room_costs
                    ON c."Id" = ranked_room_costs."CostId"
                WHERE ranked_room_costs.rn > 1;
                """);

            migrationBuilder.Sql(
                """
                DELETE FROM "Costs"
                WHERE "RoomId" IS NULL;
                """);

            migrationBuilder.DropColumn(
                name: "CostId",
                table: "Rooms");

            migrationBuilder.AlterColumn<int>(
                name: "RoomId",
                table: "Costs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CostId",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Costs_RoomId",
                table: "Costs",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Costs_Rooms_RoomId",
                table: "Costs",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Costs_Rooms_RoomId",
                table: "Costs");

            migrationBuilder.DropIndex(
                name: "IX_Costs_RoomId",
                table: "Costs");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Costs");

            migrationBuilder.DropColumn(
                name: "CostId",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "CostId",
                table: "Rooms",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CostPerTime", "Hour" },
                values: new object[] { 120000m, 8 });

            migrationBuilder.UpdateData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CostPerTime", "Hour" },
                values: new object[] { 165000m, 12 });

            migrationBuilder.UpdateData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CostPerTime", "Hour" },
                values: new object[] { 52000m, 2 });

            migrationBuilder.UpdateData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CostPerTime", "Hour" },
                values: new object[] { 86000m, 4 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                column: "CostId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                column: "CostId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                column: "CostId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4,
                column: "CostId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5,
                column: "CostId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 6,
                column: "CostId",
                value: 8);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 7,
                column: "CostId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 8,
                column: "CostId",
                value: 8);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 9,
                column: "CostId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 10,
                column: "CostId",
                value: 5);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CostId",
                table: "Rooms",
                column: "CostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Costs_CostId",
                table: "Rooms",
                column: "CostId",
                principalTable: "Costs",
                principalColumn: "Id");
        }
    }
}
