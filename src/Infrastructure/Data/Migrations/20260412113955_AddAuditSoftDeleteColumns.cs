using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop_Cam_BE.Infrastructure.Data.Migrations
{
    public partial class AddAuditSoftDeleteColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SiteSettings được tạo đầy đủ trong migration AddSiteSettings (sau migration này theo timestamp).

            migrationBuilder.AddColumn<Guid>(
                name: "created_by_user_id",
                table: "user",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "updated_by_user_id",
                table: "user",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "Roles",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedTime",
                table: "Roles",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE Roles SET CreatedTime = SYSUTCDATETIME(), UpdatedTime = SYSUTCDATETIME() 
                  WHERE CreatedTime IS NULL;");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "Roles",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedTime",
                table: "Roles",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "Products",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedTime",
                table: "Products",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE Products SET CreatedTime = SYSUTCDATETIME(), UpdatedTime = SYSUTCDATETIME() 
                  WHERE CreatedTime IS NULL;");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "Products",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedTime",
                table: "Products",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ProductReviews",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "ProductReviews",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ProductReviews",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ProductReviews",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedTime",
                table: "ProductReviews",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE ProductReviews SET 
                    CreatedTime = TODATETIMEOFFSET(CreatedAt, 0),
                    UpdatedTime = TODATETIMEOFFSET(CreatedAt, 0)
                  WHERE CreatedTime IS NULL;");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProductReviews");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "ProductReviews",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedTime",
                table: "ProductReviews",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ProductCategories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "ProductCategories",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ProductCategories",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ProductCategories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedTime",
                table: "ProductCategories",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE ProductCategories SET CreatedTime = SYSUTCDATETIME(), UpdatedTime = SYSUTCDATETIME() 
                  WHERE CreatedTime IS NULL;");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "ProductCategories",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedTime",
                table: "ProductCategories",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "Orders",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedTime",
                table: "Orders",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE Orders SET 
                    CreatedTime = TODATETIMEOFFSET(CreatedAt, 0),
                    UpdatedTime = TODATETIMEOFFSET(CreatedAt, 0)
                  WHERE CreatedTime IS NULL;");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Orders");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "Orders",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedTime",
                table: "Orders",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "NewsArticles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "NewsArticles",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "NewsArticles",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "NewsArticles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedTime",
                table: "NewsArticles",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE NewsArticles SET 
                    CreatedTime = TODATETIMEOFFSET(PublishedAt, 0),
                    UpdatedTime = TODATETIMEOFFSET(PublishedAt, 0)
                  WHERE CreatedTime IS NULL;");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "NewsArticles",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedTime",
                table: "NewsArticles",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "HomeBanners",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "HomeBanners",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "HomeBanners",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "HomeBanners",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedTime",
                table: "HomeBanners",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE HomeBanners SET CreatedTime = SYSUTCDATETIME(), UpdatedTime = SYSUTCDATETIME() 
                  WHERE CreatedTime IS NULL;");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "HomeBanners",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedTime",
                table: "HomeBanners",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_IsActive",
                table: "Roles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsActive",
                table: "Products",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_IsActive",
                table: "ProductReviews",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_IsActive",
                table: "ProductCategories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IsActive",
                table: "Orders",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticles_IsActive",
                table: "NewsArticles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_HomeBanners_IsActive",
                table: "HomeBanners",
                column: "IsActive");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Roles_IsActive",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Products_IsActive",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_IsActive",
                table: "ProductReviews");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategories_IsActive",
                table: "ProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_Orders_IsActive",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_NewsArticles_IsActive",
                table: "NewsArticles");

            migrationBuilder.DropIndex(
                name: "IX_HomeBanners_IsActive",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "created_by_user_id",
                table: "user");

            migrationBuilder.DropColumn(
                name: "updated_by_user_id",
                table: "user");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "HomeBanners");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ProductReviews",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
