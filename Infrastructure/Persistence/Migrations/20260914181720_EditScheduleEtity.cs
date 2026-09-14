using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EditScheduleEtity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeofenceRadiusMeters",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "RouterMac",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "StoreID",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "RegisteredDeviceId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "CheckInDeviceId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "CheckInLatitude",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "CheckInLongitude",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "RegisteredRouterMac",
                table: "Devices",
                newName: "RegisteredDeviceMac");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Schedules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredDeviceIp",
                table: "Devices",
                type: "character varying(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckInDeviceMac",
                table: "Attendances",
                type: "character varying(17)",
                maxLength: 17,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckInIp",
                table: "Attendances",
                type: "character varying(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StoreRouters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    MacAddress = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    Label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreRouters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreRouters_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreRouters_MacAddress",
                table: "StoreRouters",
                column: "MacAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreRouters_StoreId",
                table: "StoreRouters",
                column: "StoreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreRouters");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "RegisteredDeviceIp",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "CheckInDeviceMac",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "CheckInIp",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "RegisteredDeviceMac",
                table: "Devices",
                newName: "RegisteredRouterMac");

            migrationBuilder.AddColumn<int>(
                name: "GeofenceRadiusMeters",
                table: "Stores",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Stores",
                type: "numeric(9,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Stores",
                type: "numeric(9,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RouterMac",
                table: "Stores",
                type: "character varying(17)",
                maxLength: 17,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StoreID",
                table: "Stores",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegisteredDeviceId",
                table: "Devices",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CheckInDeviceId",
                table: "Attendances",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CheckInLatitude",
                table: "Attendances",
                type: "numeric(9,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CheckInLongitude",
                table: "Attendances",
                type: "numeric(9,6)",
                nullable: true);
        }
    }
}
