using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HotelMaintenanceManyToManyMaintenanceTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Costs",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "HotelPicture",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "HotelPicture",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "HotelPicture",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "HotelPicture",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "HotelPicture",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RoomPictures",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RoomPictures",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RoomPictures",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RoomPictures",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RoomPictures",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RoomPictures",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "RoomPictures",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "RoomPictures",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "RoomPictures",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "RoomPictures",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "TimesAvialable",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TimesAvialable",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TimesAvialable",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TimesAvialable",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TimesAvialable",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TimesAvialable",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TimesAvialable",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TimesAvialable",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.CreateTable(
                name: "HotelMaintenances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameMaintenance = table.Column<string>(type: "text", nullable: false),
                    HotelId = table.Column<int>(type: "integer", nullable: false),
                    TelephoneNumber = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateBy = table.Column<string>(type: "text", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelMaintenances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelMaintenances_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HotelMaintenanceMaintenanceTypes",
                columns: table => new
                {
                    HotelMaintenancesId = table.Column<int>(type: "integer", nullable: false),
                    MaintenanceTypesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelMaintenanceMaintenanceTypes", x => new { x.HotelMaintenancesId, x.MaintenanceTypesId });
                    table.ForeignKey(
                        name: "FK_HotelMaintenanceMaintenanceTypes_HotelMaintenances_HotelMai~",
                        column: x => x.HotelMaintenancesId,
                        principalTable: "HotelMaintenances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HotelMaintenanceMaintenanceTypes_MaintenanceTypes_Maintenan~",
                        column: x => x.MaintenanceTypesId,
                        principalTable: "MaintenanceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "IssuesTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Issue",
                value: "Agua");

            migrationBuilder.UpdateData(
                table: "IssuesTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Issue",
                value: "Electricidad");

            migrationBuilder.UpdateData(
                table: "IssuesTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "Issue",
                value: "Servicios");

            migrationBuilder.InsertData(
                table: "IssuesTypes",
                columns: new[] { "Id", "Issue" },
                values: new object[,]
                {
                    { 6, "Ruido" },
                    { 7, "Problema con el acceso" },
                    { 8, "Otro" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HotelMaintenanceMaintenanceTypes_MaintenanceTypesId",
                table: "HotelMaintenanceMaintenanceTypes",
                column: "MaintenanceTypesId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelMaintenances_HotelId",
                table: "HotelMaintenances",
                column: "HotelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HotelMaintenanceMaintenanceTypes");

            migrationBuilder.DropTable(
                name: "HotelMaintenances");

            migrationBuilder.DropTable(
                name: "MaintenanceTypes");

            migrationBuilder.DeleteData(
                table: "IssuesTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "IssuesTypes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "IssuesTypes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "CountryId", "IdCity", "Latitud", "Longitud", "Number", "PostalCode", "ProvinceId", "Street" },
                values: new object[,]
                {
                    { 1, 0, 1, "-34.6037", "-58.3816", "1234", "C1043", 0, "Av. Corrientes" },
                    { 2, 0, 23, "-32.9442", "-60.6505", "890", "S2000", 0, "Bv. Oroño" },
                    { 3, 0, 15, "-32.8895", "-68.8458", "456", "M5500", 0, "Av. San Martín" },
                    { 4, 0, 18, "-41.1335", "-71.3103", "11500", "R8400", 0, "Av. Bustillo" },
                    { 5, 0, 19, "-24.7829", "-65.4232", "786", "A4400", 0, "Caseros" }
                });

            migrationBuilder.UpdateData(
                table: "IssuesTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Issue",
                value: "Ruido");

            migrationBuilder.UpdateData(
                table: "IssuesTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Issue",
                value: "Problema con el acceso");

            migrationBuilder.UpdateData(
                table: "IssuesTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "Issue",
                value: "Otro");

            migrationBuilder.InsertData(
                table: "TimesAvialable",
                columns: new[] { "Id", "Time" },
                values: new object[,]
                {
                    { 1, "08:00" },
                    { 2, "10:00" },
                    { 3, "12:00" },
                    { 4, "14:00" },
                    { 5, "16:00" },
                    { 6, "18:00" },
                    { 7, "20:00" },
                    { 8, "22:00" }
                });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "AddressHotelId", "CodeArea", "Description", "Email", "IdentityNumber", "MetaDescription", "Name", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, 1, 11, "Hotel urbano en el centro porteño.", "reservas@hotelobelisco.com", null, "Ideal para negocios y escapadas en Buenos Aires.", "Hotel Obelisco", 43219876 },
                    { 2, 2, 341, "Hotel moderno con vista al río Paraná.", "hola@rosarioriverside.com", null, "Alojamiento premium en Rosario.", "Rosario Riverside", 5588776 },
                    { 3, 3, 261, "Suites boutique cerca de bodegas y montaña.", "info@andessuites.com", null, "Descanso y vino en Mendoza.", "Andes Suites Mendoza", 4477551 },
                    { 4, 4, 294, "Hotel de montaña con vista al lago.", "contacto@patagoniaview.com", null, "Experiencia patagónica en Bariloche.", "Patagonia View Bariloche", 4522334 },
                    { 5, 5, 387, "Hotel cálido en el casco histórico salteño.", "reservas@saltacolonial.com", null, "Tradición y confort en Salta.", "Salta Colonial", 4123456 }
                });

            migrationBuilder.InsertData(
                table: "HotelPicture",
                columns: new[] { "Id", "HotelId", "Path" },
                values: new object[,]
                {
                    { 1, 1, "assets/hotels/obelisco.jpg" },
                    { 2, 2, "assets/hotels/rosario-riverside.jpg" },
                    { 3, 3, "assets/hotels/andes-suites.jpg" },
                    { 4, 4, "assets/hotels/patagonia-view.jpg" },
                    { 5, 5, "assets/hotels/salta-colonial.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "AvialableNow", "BedNumbers", "Description", "HotelsId", "Name" },
                values: new object[,]
                {
                    { 1, true, 1, "Habitación individual con escritorio.", 1, "Single Business" },
                    { 2, true, 2, "Habitación doble con desayuno incluido.", 1, "Doble Ejecutiva" },
                    { 3, true, 2, "Suite con vista al río.", 2, "Suite Paraná" },
                    { 4, false, 3, "Ideal para familias y estadías cortas.", 2, "Familiar Rosario" },
                    { 5, true, 2, "Habitación premium con ambientación mendocina.", 3, "Suite Malbec" },
                    { 6, true, 4, "Amplia habitación para grupos pequeños.", 3, "Familiar Cordillera" },
                    { 7, true, 2, "Vista al lago y detalles patagónicos.", 4, "Lago Superior" },
                    { 8, true, 3, "Suite con living y balcón.", 4, "Suite Nahuel" },
                    { 9, true, 1, "Opción práctica para viajeros solos.", 5, "Colonial Single" },
                    { 10, true, 2, "Decoración regional y patio interno.", 5, "Tradición Norteña" }
                });

            migrationBuilder.InsertData(
                table: "Costs",
                columns: new[] { "Id", "CostPerTime", "Hour", "RoomId" },
                values: new object[,]
                {
                    { 1, 45000m, 2, 1 },
                    { 2, 78000m, 4, 1 },
                    { 3, 78000m, 4, 2 },
                    { 4, 120000m, 8, 2 },
                    { 5, 86000m, 4, 3 },
                    { 6, 134000m, 8, 3 },
                    { 7, 134000m, 8, 4 },
                    { 8, 182000m, 12, 4 },
                    { 9, 86000m, 4, 5 },
                    { 10, 134000m, 8, 5 },
                    { 11, 134000m, 8, 6 },
                    { 12, 182000m, 12, 6 },
                    { 13, 134000m, 8, 7 },
                    { 14, 182000m, 12, 7 },
                    { 15, 134000m, 8, 8 },
                    { 16, 182000m, 12, 8 },
                    { 17, 45000m, 2, 9 },
                    { 18, 78000m, 4, 9 },
                    { 19, 52000m, 2, 10 },
                    { 20, 86000m, 4, 10 }
                });

            migrationBuilder.InsertData(
                table: "RoomPictures",
                columns: new[] { "Id", "Name", "RoomId" },
                values: new object[,]
                {
                    { 1, "assets/rooms/single-business.jpg", 1 },
                    { 2, "assets/rooms/doble-ejecutiva.jpg", 2 },
                    { 3, "assets/rooms/suite-parana.jpg", 3 },
                    { 4, "assets/rooms/familiar-rosario.jpg", 4 },
                    { 5, "assets/rooms/suite-malbec.jpg", 5 },
                    { 6, "assets/rooms/familiar-cordillera.jpg", 6 },
                    { 7, "assets/rooms/lago-superior.jpg", 7 },
                    { 8, "assets/rooms/suite-nahuel.jpg", 8 },
                    { 9, "assets/rooms/colonial-single.jpg", 9 },
                    { 10, "assets/rooms/tradicion-nortena.jpg", 10 }
                });
        }
    }
}
