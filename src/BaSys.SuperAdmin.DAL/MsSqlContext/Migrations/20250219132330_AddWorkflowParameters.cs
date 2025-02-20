using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaSys.SuperAdmin.DAL.MsSqlContext.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxConcurrentWorkflows",
                table: "AppRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "UseWorkflowsScheduler",
                table: "AppRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WorkflowPollInterval",
                table: "AppRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkflowThreadsCount",
                table: "AppRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxConcurrentWorkflows",
                table: "AppRecords");

            migrationBuilder.DropColumn(
                name: "UseWorkflowsScheduler",
                table: "AppRecords");

            migrationBuilder.DropColumn(
                name: "WorkflowPollInterval",
                table: "AppRecords");

            migrationBuilder.DropColumn(
                name: "WorkflowThreadsCount",
                table: "AppRecords");
        }
    }
}
