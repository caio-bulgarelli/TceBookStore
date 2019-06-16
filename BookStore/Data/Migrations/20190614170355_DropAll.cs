using Microsoft.EntityFrameworkCore.Migrations;

namespace BookStore.Data.Migrations
{
    public partial class DropAll : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Books");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
