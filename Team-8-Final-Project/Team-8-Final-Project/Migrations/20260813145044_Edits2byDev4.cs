using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team_8_Final_Project.Migrations
{
    /// <inheritdoc />
    public partial class Edits2byDev4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorBook_Authors_AuthorsAuthorID",
                table: "AuthorBook");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Categories_CategoryID",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publishers_PublisherID",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_Events_EventsEventID",
                table: "EventUser");

            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_Users_UsersUserID",
                table: "EventUser");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "PublisherID",
                table: "Publishers",
                newName: "PublisherId");

            migrationBuilder.RenameColumn(
                name: "UsersUserID",
                table: "EventUser",
                newName: "UsersUserId");

            migrationBuilder.RenameColumn(
                name: "EventsEventID",
                table: "EventUser",
                newName: "EventsEventId");

            migrationBuilder.RenameIndex(
                name: "IX_EventUser_UsersUserID",
                table: "EventUser",
                newName: "IX_EventUser_UsersUserId");

            migrationBuilder.RenameColumn(
                name: "EventID",
                table: "Events",
                newName: "EventId");

            migrationBuilder.RenameColumn(
                name: "CategoryID",
                table: "Categories",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "PublisherID",
                table: "Books",
                newName: "PublisherId");

            migrationBuilder.RenameColumn(
                name: "CategoryID",
                table: "Books",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Books_PublisherID",
                table: "Books",
                newName: "IX_Books_PublisherId");

            migrationBuilder.RenameIndex(
                name: "IX_Books_CategoryID",
                table: "Books",
                newName: "IX_Books_CategoryId");

            migrationBuilder.RenameColumn(
                name: "AuthorID",
                table: "Authors",
                newName: "AuthorId");

            migrationBuilder.RenameColumn(
                name: "AuthorsAuthorID",
                table: "AuthorBook",
                newName: "AuthorsAuthorId");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Books",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PublisherId",
                table: "Books",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Books",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BookEdition",
                table: "Books",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorBook_Authors_AuthorsAuthorId",
                table: "AuthorBook",
                column: "AuthorsAuthorId",
                principalTable: "Authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Categories_CategoryId",
                table: "Books",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books",
                column: "PublisherId",
                principalTable: "Publishers",
                principalColumn: "PublisherId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_Events_EventsEventId",
                table: "EventUser",
                column: "EventsEventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_Users_UsersUserId",
                table: "EventUser",
                column: "UsersUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorBook_Authors_AuthorsAuthorId",
                table: "AuthorBook");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Categories_CategoryId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_Events_EventsEventId",
                table: "EventUser");

            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_Users_UsersUserId",
                table: "EventUser");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "PublisherId",
                table: "Publishers",
                newName: "PublisherID");

            migrationBuilder.RenameColumn(
                name: "UsersUserId",
                table: "EventUser",
                newName: "UsersUserID");

            migrationBuilder.RenameColumn(
                name: "EventsEventId",
                table: "EventUser",
                newName: "EventsEventID");

            migrationBuilder.RenameIndex(
                name: "IX_EventUser_UsersUserId",
                table: "EventUser",
                newName: "IX_EventUser_UsersUserID");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "Events",
                newName: "EventID");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Categories",
                newName: "CategoryID");

            migrationBuilder.RenameColumn(
                name: "PublisherId",
                table: "Books",
                newName: "PublisherID");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Books",
                newName: "CategoryID");

            migrationBuilder.RenameIndex(
                name: "IX_Books_PublisherId",
                table: "Books",
                newName: "IX_Books_PublisherID");

            migrationBuilder.RenameIndex(
                name: "IX_Books_CategoryId",
                table: "Books",
                newName: "IX_Books_CategoryID");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Authors",
                newName: "AuthorID");

            migrationBuilder.RenameColumn(
                name: "AuthorsAuthorId",
                table: "AuthorBook",
                newName: "AuthorsAuthorID");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PublisherID",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryID",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BookEdition",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorBook_Authors_AuthorsAuthorID",
                table: "AuthorBook",
                column: "AuthorsAuthorID",
                principalTable: "Authors",
                principalColumn: "AuthorID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Categories_CategoryID",
                table: "Books",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publishers_PublisherID",
                table: "Books",
                column: "PublisherID",
                principalTable: "Publishers",
                principalColumn: "PublisherID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_Events_EventsEventID",
                table: "EventUser",
                column: "EventsEventID",
                principalTable: "Events",
                principalColumn: "EventID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_Users_UsersUserID",
                table: "EventUser",
                column: "UsersUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
