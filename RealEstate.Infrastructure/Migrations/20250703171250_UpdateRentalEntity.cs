using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRentalEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Rentals");

            migrationBuilder.AddColumn<short>(
                name: "Duration",
                table: "Rentals",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "RentType",
                table: "Rentals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "RentType",
                table: "Rentals");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "Rentals",
                type: "date",
                nullable: false,
                defaultValueSql: "(CONVERT([date],getdate()))");
        }
    }
}
