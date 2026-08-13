using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team_8_Final_Project.Migrations
{
    /// <inheritdoc />
    public partial class Dev4Test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorBook_authors_AuthorsAuthorID",
                table: "AuthorBook");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorBook_books_BooksBookId",
                table: "AuthorBook");

            migrationBuilder.DropForeignKey(
                name: "FK_bookCopies_books_BookId",
                table: "bookCopies");

            migrationBuilder.DropForeignKey(
                name: "FK_bookCopies_shelves_ShelfId",
                table: "bookCopies");

            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_events_EventsEventID",
                table: "EventUser");

            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_users_UsersUserID",
                table: "EventUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Fines_loans_LoanId",
                table: "Fines");

            migrationBuilder.DropForeignKey(
                name: "FK_loans_bookCopies_BookCopyId",
                table: "loans");

            migrationBuilder.DropForeignKey(
                name: "FK_loans_users_UserID",
                table: "loans");

            migrationBuilder.DropForeignKey(
                name: "FK_reservations_books_BookId",
                table: "reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_reservations_users_UserId",
                table: "reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_books_BookId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_users_UserId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_shelves",
                table: "shelves");

            migrationBuilder.DropPrimaryKey(
                name: "PK_reservations",
                table: "reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_loans",
                table: "loans");

            migrationBuilder.DropIndex(
                name: "IX_Fines_LoanId",
                table: "Fines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_events",
                table: "events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_books",
                table: "books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_bookCopies",
                table: "bookCopies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_authors",
                table: "authors");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "shelves",
                newName: "Shelves");

            migrationBuilder.RenameTable(
                name: "reservations",
                newName: "Reservations");

            migrationBuilder.RenameTable(
                name: "loans",
                newName: "Loans");

            migrationBuilder.RenameTable(
                name: "events",
                newName: "Events");

            migrationBuilder.RenameTable(
                name: "books",
                newName: "Books");

            migrationBuilder.RenameTable(
                name: "bookCopies",
                newName: "BookCopies");

            migrationBuilder.RenameTable(
                name: "authors",
                newName: "Authors");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Reservations",
                newName: "Status");

            migrationBuilder.RenameIndex(
                name: "IX_reservations_UserId",
                table: "Reservations",
                newName: "IX_Reservations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_reservations_BookId",
                table: "Reservations",
                newName: "IX_Reservations_BookId");

            migrationBuilder.RenameIndex(
                name: "IX_loans_UserID",
                table: "Loans",
                newName: "IX_Loans_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_loans_BookCopyId",
                table: "Loans",
                newName: "IX_Loans_BookCopyId");

            migrationBuilder.RenameColumn(
                name: "IBSN",
                table: "Books",
                newName: "ISBN");

            migrationBuilder.RenameIndex(
                name: "IX_bookCopies_ShelfId",
                table: "BookCopies",
                newName: "IX_BookCopies_ShelfId");

            migrationBuilder.RenameIndex(
                name: "IX_bookCopies_BookId",
                table: "BookCopies",
                newName: "IX_BookCopies_BookId");

            migrationBuilder.AddColumn<int>(
                name: "EventMaxCap",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PublisherID",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shelves",
                table: "Shelves",
                column: "ShelfId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations",
                column: "ReservationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Loans",
                table: "Loans",
                column: "LoanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "EventID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Books",
                table: "Books",
                column: "BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookCopies",
                table: "BookCopies",
                column: "BookCopyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authors",
                table: "Authors",
                column: "AuthorID");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_LoanId",
                table: "Fines",
                column: "LoanId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorBook_Authors_AuthorsAuthorID",
                table: "AuthorBook",
                column: "AuthorsAuthorID",
                principalTable: "Authors",
                principalColumn: "AuthorID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorBook_Books_BooksBookId",
                table: "AuthorBook",
                column: "BooksBookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_Books_BookId",
                table: "BookCopies",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_Shelves_ShelfId",
                table: "BookCopies",
                column: "ShelfId",
                principalTable: "Shelves",
                principalColumn: "ShelfId",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_Loans_LoanId",
                table: "Fines",
                column: "LoanId",
                principalTable: "Loans",
                principalColumn: "LoanId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_BookCopies_BookCopyId",
                table: "Loans",
                column: "BookCopyId",
                principalTable: "BookCopies",
                principalColumn: "BookCopyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Users_UserID",
                table: "Loans",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Books_BookId",
                table: "Reservations",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_UserId",
                table: "Reservations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Books_BookId",
                table: "Reviews",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorBook_Authors_AuthorsAuthorID",
                table: "AuthorBook");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorBook_Books_BooksBookId",
                table: "AuthorBook");

            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_Books_BookId",
                table: "BookCopies");

            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_Shelves_ShelfId",
                table: "BookCopies");

            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_Events_EventsEventID",
                table: "EventUser");

            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_Users_UsersUserID",
                table: "EventUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Fines_Loans_LoanId",
                table: "Fines");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_BookCopies_BookCopyId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Users_UserID",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Books_BookId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_UserId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Books_BookId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shelves",
                table: "Shelves");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Loans",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Fines_LoanId",
                table: "Fines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Books",
                table: "Books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookCopies",
                table: "BookCopies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Authors",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "EventMaxCap",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PublisherID",
                table: "Books");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Shelves",
                newName: "shelves");

            migrationBuilder.RenameTable(
                name: "Reservations",
                newName: "reservations");

            migrationBuilder.RenameTable(
                name: "Loans",
                newName: "loans");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "events");

            migrationBuilder.RenameTable(
                name: "Books",
                newName: "books");

            migrationBuilder.RenameTable(
                name: "BookCopies",
                newName: "bookCopies");

            migrationBuilder.RenameTable(
                name: "Authors",
                newName: "authors");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "reservations",
                newName: "status");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_UserId",
                table: "reservations",
                newName: "IX_reservations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_BookId",
                table: "reservations",
                newName: "IX_reservations_BookId");

            migrationBuilder.RenameIndex(
                name: "IX_Loans_UserID",
                table: "loans",
                newName: "IX_loans_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Loans_BookCopyId",
                table: "loans",
                newName: "IX_loans_BookCopyId");

            migrationBuilder.RenameColumn(
                name: "ISBN",
                table: "books",
                newName: "IBSN");

            migrationBuilder.RenameIndex(
                name: "IX_BookCopies_ShelfId",
                table: "bookCopies",
                newName: "IX_bookCopies_ShelfId");

            migrationBuilder.RenameIndex(
                name: "IX_BookCopies_BookId",
                table: "bookCopies",
                newName: "IX_bookCopies_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_shelves",
                table: "shelves",
                column: "ShelfId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_reservations",
                table: "reservations",
                column: "ReservationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_loans",
                table: "loans",
                column: "LoanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_events",
                table: "events",
                column: "EventID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_books",
                table: "books",
                column: "BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_bookCopies",
                table: "bookCopies",
                column: "BookCopyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_authors",
                table: "authors",
                column: "AuthorID");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_LoanId",
                table: "Fines",
                column: "LoanId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorBook_authors_AuthorsAuthorID",
                table: "AuthorBook",
                column: "AuthorsAuthorID",
                principalTable: "authors",
                principalColumn: "AuthorID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorBook_books_BooksBookId",
                table: "AuthorBook",
                column: "BooksBookId",
                principalTable: "books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_bookCopies_books_BookId",
                table: "bookCopies",
                column: "BookId",
                principalTable: "books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_bookCopies_shelves_ShelfId",
                table: "bookCopies",
                column: "ShelfId",
                principalTable: "shelves",
                principalColumn: "ShelfId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_events_EventsEventID",
                table: "EventUser",
                column: "EventsEventID",
                principalTable: "events",
                principalColumn: "EventID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_users_UsersUserID",
                table: "EventUser",
                column: "UsersUserID",
                principalTable: "users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_loans_LoanId",
                table: "Fines",
                column: "LoanId",
                principalTable: "loans",
                principalColumn: "LoanId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_loans_bookCopies_BookCopyId",
                table: "loans",
                column: "BookCopyId",
                principalTable: "bookCopies",
                principalColumn: "BookCopyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_loans_users_UserID",
                table: "loans",
                column: "UserID",
                principalTable: "users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reservations_books_BookId",
                table: "reservations",
                column: "BookId",
                principalTable: "books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reservations_users_UserId",
                table: "reservations",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_books_BookId",
                table: "Reviews",
                column: "BookId",
                principalTable: "books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_users_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
