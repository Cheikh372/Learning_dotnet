using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoListApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateDb_Id_to_TaskID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Tasks",
                newName: "TaskID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaskID",
                table: "Tasks",
                newName: "Id");
        }
    }
}
