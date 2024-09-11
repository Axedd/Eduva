using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddNewPrimaryKeyToSchoolClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_SchoolClasses_ClassId",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SchoolClasses",
                table: "SchoolClasses");

            migrationBuilder.RenameTable(
                name: "Students",
                newName: "student");

            migrationBuilder.RenameTable(
                name: "SchoolClasses",
                newName: "schoolclass");

            migrationBuilder.RenameIndex(
                name: "IX_Students_ClassId",
                table: "student",
                newName: "IX_student_ClassId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_student",
                table: "student",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_schoolclass",
                table: "schoolclass",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_student_schoolclass_ClassId",
                table: "student",
                column: "ClassId",
                principalTable: "schoolclass",
                principalColumn: "ClassId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_schoolclass_ClassId",
                table: "student");

            migrationBuilder.DropPrimaryKey(
                name: "PK_student",
                table: "student");

            migrationBuilder.DropPrimaryKey(
                name: "PK_schoolclass",
                table: "schoolclass");

            migrationBuilder.RenameTable(
                name: "student",
                newName: "Students");

            migrationBuilder.RenameTable(
                name: "schoolclass",
                newName: "SchoolClasses");

            migrationBuilder.RenameIndex(
                name: "IX_student_ClassId",
                table: "Students",
                newName: "IX_Students_ClassId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SchoolClasses",
                table: "SchoolClasses",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_SchoolClasses_ClassId",
                table: "Students",
                column: "ClassId",
                principalTable: "SchoolClasses",
                principalColumn: "ClassId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
