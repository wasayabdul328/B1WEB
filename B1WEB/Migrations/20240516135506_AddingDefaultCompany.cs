using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B1WEB.Migrations
{
    /// <inheritdoc />
    public partial class AddingDefaultCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DefaultCompany",
                table: "CompanyConfiguration",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultCompany",
                table: "CompanyConfiguration");
        }
    }
}
