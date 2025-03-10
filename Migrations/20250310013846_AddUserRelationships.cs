using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NatureHub.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Discussions");

            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Discussions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_ApplicationUserId",
                table: "Discussions",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ApplicationUserId",
                table: "Comments",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_ApplicationUserId",
                table: "Comments",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Discussions_AspNetUsers_ApplicationUserId",
                table: "Discussions",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_ApplicationUserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Discussions_AspNetUsers_ApplicationUserId",
                table: "Discussions");

            migrationBuilder.DropIndex(
                name: "IX_Discussions_ApplicationUserId",
                table: "Discussions");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ApplicationUserId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Discussions");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Discussions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
