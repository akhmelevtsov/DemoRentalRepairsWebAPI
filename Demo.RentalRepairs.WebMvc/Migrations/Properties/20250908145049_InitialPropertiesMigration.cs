using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.RentalRepairs.WebMvc.Migrations.Properties
{
    /// <inheritdoc />
    public partial class InitialPropertiesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PropertyTbl",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_StreetNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_StreetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Superintendent_FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Superintendent_LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Superintendent_EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Superintendent_MobilePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoReplyEmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoginEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Units = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdCreated = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WorkerTbl",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactInfo_FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInfo_LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInfo_EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInfo_MobilePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TenantTbl",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactInfo_FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInfo_LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInfo_EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInfo_MobilePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropertyTblID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UnitNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoginEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantTbl", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TenantTbl_PropertyTbl_PropertyTblID",
                        column: x => x.PropertyTblID,
                        principalTable: "PropertyTbl",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "TenantRequestTbl",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestStatus = table.Column<int>(type: "int", nullable: false),
                    ServiceWorkOrderCount = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestChanges = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantRequestTbl", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TenantRequestTbl_TenantTbl_TenantID",
                        column: x => x.TenantID,
                        principalTable: "TenantTbl",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantRequestTbl_TenantID",
                table: "TenantRequestTbl",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantTbl_PropertyTblID",
                table: "TenantTbl",
                column: "PropertyTblID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantRequestTbl");

            migrationBuilder.DropTable(
                name: "WorkerTbl");

            migrationBuilder.DropTable(
                name: "TenantTbl");

            migrationBuilder.DropTable(
                name: "PropertyTbl");
        }
    }
}
