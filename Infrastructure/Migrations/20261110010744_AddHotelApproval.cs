using System;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Hotels that already exist are live: backfill them as Approved (1) so they don't
            // disappear from search. New hotels always get an explicit value from the app
            // (Pending unless created by a SuperAdmin); the model has no DB default.
            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Hotels",
                type: "integer",
                nullable: false,
                defaultValue: (int)HotelApprovalStatus.Approved);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Hotels",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAtUtc",
                table: "Hotels",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedByUserId",
                table: "Hotels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAtUtc",
                table: "Hotels",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_ApprovalStatus",
                table: "Hotels",
                column: "ApprovalStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Hotels_ApprovalStatus",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "ReviewedAtUtc",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "ReviewedByUserId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "SubmittedAtUtc",
                table: "Hotels");
        }
    }
}
