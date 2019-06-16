using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookStore.Data.Migrations
{
    public partial class AddBooks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false).Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Isbn = table.Column<string>(maxLength: 13, nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: false),
                    Author = table.Column<string>(maxLength: 128, nullable: false),
                    Price = table.Column<double>(nullable: false),
                    PublicationDate = table.Column<int>(nullable: false),
                    Cover = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.UniqueConstraint("Unique_ISBN", x => x.Isbn);
                }); ;
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
