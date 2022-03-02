
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetIdentitydemo.Migrations
{
    public partial class extendOrg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExternalId",
                table: "Organizations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_ExternalId",
                table: "Organizations",
                column: "ExternalId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organizations_ExternalId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Organizations");
        }
    }
}
