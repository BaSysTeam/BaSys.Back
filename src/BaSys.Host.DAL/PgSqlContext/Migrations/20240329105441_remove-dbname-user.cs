using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaSys.Host.DAL.PgSqlContext.Migrations
{
    /// <inheritdoc />
    public partial class removedbnameuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DbName",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DbName",
                table: "AspNetUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
