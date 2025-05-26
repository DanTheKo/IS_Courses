using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseService.Migrations
{
    /// <inheritdoc />
    public partial class change_0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseMetadataId",
                table: "Courses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CourseMetadataId",
                table: "Courses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
