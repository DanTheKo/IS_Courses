using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseService.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CourseMetadataId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseMetadata_IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CourseMetadata_PreviewImageUrl = table.Column<string>(type: "text", nullable: false),
                    CourseMetadata_Duration = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseItems_CourseItems_ParentId",
                        column: x => x.ParentId,
                        principalTable: "CourseItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseItems_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CourseItemId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contents_CourseItems_CourseItemId",
                        column: x => x.CourseItemId,
                        principalTable: "CourseItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CourseItemId",
                table: "Contents",
                column: "CourseItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseItems_CourseId",
                table: "CourseItems",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseItems_ParentId",
                table: "CourseItems",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contents");

            migrationBuilder.DropTable(
                name: "CourseItems");

            migrationBuilder.DropTable(
                name: "Courses");
        }
    }
}
