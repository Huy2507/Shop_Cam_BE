using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop_Cam_BE.Infrastructure.Data.Migrations
{
    public partial class RemoveKeycloakAddPasswordHash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "user_keycloak_id_key",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_user_keycloak_id",
                table: "user");

            migrationBuilder.DropColumn(
                name: "keycloak_id",
                table: "user");

            migrationBuilder.AddColumn<string>(
                name: "password_hash",
                table: "user",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password_hash",
                table: "user");

            migrationBuilder.AddColumn<Guid>(
                name: "keycloak_id",
                table: "user",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_keycloak_id",
                table: "user",
                column: "keycloak_id",
                unique: true,
                filter: "[keycloak_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "user_keycloak_id_key",
                table: "user",
                column: "keycloak_id",
                unique: true,
                filter: "[keycloak_id] IS NOT NULL");
        }
    }
}
