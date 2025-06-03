using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopping_Tutorial.Migrations
{
    /// <inheritdoc />
    public partial class ThongKeeeeee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataCreated",
                table: "ThongKes",
                newName: "DateCreated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "ThongKes",
                newName: "DataCreated");
        }
    }
}
