using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop_Cam_BE.Infrastructure.Data.Migrations
{
    public partial class AddProductDescriptionAndNewsBody : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Products",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "NewsArticles",
                type: "nvarchar(max)",
                maxLength: 16000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Body",
                table: "NewsArticles");
        }
    }
}
