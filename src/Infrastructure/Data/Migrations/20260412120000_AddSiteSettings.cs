using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop_Cam_BE.Infrastructure.Data.Migrations
{
    public partial class AddSiteSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteSettings",
                columns: table => new
                {
                    SiteSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Group = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ValueJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.SiteSettingId);
                    table.ForeignKey(
                        name: "FK_SiteSettings_user_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SiteSettings_Group_Key",
                table: "SiteSettings",
                columns: new[] { "Group", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiteSettings_IsActive",
                table: "SiteSettings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SiteSettings_UpdatedByUserId",
                table: "SiteSettings",
                column: "UpdatedByUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteSettings");
        }
    }
}
