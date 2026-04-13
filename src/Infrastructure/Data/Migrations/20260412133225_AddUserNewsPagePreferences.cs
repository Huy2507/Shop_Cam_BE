using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop_Cam_BE.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNewsPagePreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserNewsPagePreferences",
                columns: table => new
                {
                    UserNewsPagePreferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValueJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNewsPagePreferences", x => x.UserNewsPagePreferenceId);
                    table.ForeignKey(
                        name: "FK_UserNewsPagePreferences_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserNewsPagePreferences_UserId",
                table: "UserNewsPagePreferences",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserNewsPagePreferences");
        }
    }
}
