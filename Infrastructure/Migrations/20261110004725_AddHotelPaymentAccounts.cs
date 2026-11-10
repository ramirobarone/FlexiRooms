using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelPaymentAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ApplicationFee",
                table: "PaymentTransactions",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HotelId",
                table: "PaymentTransactions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HotelPaymentAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HotelId = table.Column<int>(type: "integer", nullable: false),
                    MercadoPagoUserId = table.Column<long>(type: "bigint", nullable: false),
                    PublicKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AccessTokenEncrypted = table.Column<string>(type: "text", nullable: false),
                    RefreshTokenEncrypted = table.Column<string>(type: "text", nullable: false),
                    AccessTokenExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LiveMode = table.Column<bool>(type: "boolean", nullable: false),
                    ConnectedByUserId = table.Column<string>(type: "text", nullable: true),
                    ConnectedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelPaymentAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelPaymentAccounts_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_HotelId",
                table: "PaymentTransactions",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelPaymentAccounts_HotelId",
                table: "HotelPaymentAccounts",
                column: "HotelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HotelPaymentAccounts_MercadoPagoUserId",
                table: "HotelPaymentAccounts",
                column: "MercadoPagoUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HotelPaymentAccounts");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_HotelId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "ApplicationFee",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "PaymentTransactions");
        }
    }
}
