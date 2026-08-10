using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team_8_Final_Project.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fines_Loans_LoanID",
                table: "Fines");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_User_UserID",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_bookCopies_BookCopyID",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_User_UserID",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_bookCopies_BookCopyId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_User_UserID",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_books_BookID",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_BookCopyId",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Loans",
                table: "Loans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ReservationStatus",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "FinePaymentStatus",
                table: "Fines");

            migrationBuilder.RenameTable(
                name: "Reservations",
                newName: "reservations");

            migrationBuilder.RenameTable(
                name: "Loans",
                newName: "loans");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "users");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Reviews",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "BookID",
                table: "Reviews",
                newName: "BookId");

            migrationBuilder.RenameColumn(
                name: "ReviewID",
                table: "Reviews",
                newName: "ReviewId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_UserID",
                table: "Reviews",
                newName: "IX_Reviews_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_BookID",
                table: "Reviews",
                newName: "IX_Reviews_BookId");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "reservations",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "BookCopyId",
                table: "reservations",
                newName: "status");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_UserID",
                table: "reservations",
                newName: "IX_reservations_UserId");

            migrationBuilder.RenameColumn(
                name: "LoanStatus",
                table: "loans",
                newName: "loanStatus");

            migrationBuilder.RenameColumn(
                name: "BookCopyID",
                table: "loans",
                newName: "BookCopyId");

            migrationBuilder.RenameColumn(
                name: "LoanID",
                table: "loans",
                newName: "LoanId");

            migrationBuilder.RenameIndex(
                name: "IX_Loans_UserID",
                table: "loans",
                newName: "IX_loans_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Loans_BookCopyID",
                table: "loans",
                newName: "IX_loans_BookCopyId");

            migrationBuilder.RenameColumn(
                name: "LoanID",
                table: "Fines",
                newName: "LoanId");

            migrationBuilder.RenameColumn(
                name: "FineID",
                table: "Fines",
                newName: "FineId");

            migrationBuilder.RenameIndex(
                name: "IX_Fines_LoanID",
                table: "Fines",
                newName: "IX_Fines_LoanId");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReservationDate",
                table: "reservations",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "loanStatus",
                table: "loans",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LoanReturnDate",
                table: "loans",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Fines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CopyPrice",
                table: "bookCopies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ShelfId",
                table: "bookCopies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_reservations",
                table: "reservations",
                column: "ReservationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_loans",
                table: "loans",
                column: "LoanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "UserID");

            migrationBuilder.CreateTable(
                name: "authors",
                columns: table => new
                {
                    AuthorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Biography = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authors", x => x.AuthorID);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    EventID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDescription = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.EventID);
                });

            migrationBuilder.CreateTable(
                name: "shelves",
                columns: table => new
                {
                    ShelfId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShelfCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FloorNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shelves", x => x.ShelfId);
                });

            migrationBuilder.CreateTable(
                name: "AuthorBook",
                columns: table => new
                {
                    AuthorsAuthorID = table.Column<int>(type: "int", nullable: false),
                    BooksBookId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorBook", x => new { x.AuthorsAuthorID, x.BooksBookId });
                    table.ForeignKey(
                        name: "FK_AuthorBook_authors_AuthorsAuthorID",
                        column: x => x.AuthorsAuthorID,
                        principalTable: "authors",
                        principalColumn: "AuthorID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorBook_books_BooksBookId",
                        column: x => x.BooksBookId,
                        principalTable: "books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventUser",
                columns: table => new
                {
                    EventsEventID = table.Column<int>(type: "int", nullable: false),
                    UsersUserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventUser", x => new { x.EventsEventID, x.UsersUserID });
                    table.ForeignKey(
                        name: "FK_EventUser_events_EventsEventID",
                        column: x => x.EventsEventID,
                        principalTable: "events",
                        principalColumn: "EventID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventUser_users_UsersUserID",
                        column: x => x.UsersUserID,
                        principalTable: "users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reservations_BookId",
                table: "reservations",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_bookCopies_ShelfId",
                table: "bookCopies",
                column: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorBook_BooksBookId",
                table: "AuthorBook",
                column: "BooksBookId");

            migrationBuilder.CreateIndex(
                name: "IX_EventUser_UsersUserID",
                table: "EventUser",
                column: "UsersUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_bookCopies_shelves_ShelfId",
                table: "bookCopies",
                column: "ShelfId",
                principalTable: "shelves",
                principalColumn: "ShelfId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookCopies_shelves_ShelfId",
                table: "bookCopies");

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

            migrationBuilder.DropTable(
                name: "AuthorBook");

            migrationBuilder.DropTable(
                name: "EventUser");

            migrationBuilder.DropTable(
                name: "shelves");

            migrationBuilder.DropTable(
                name: "authors");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_reservations",
                table: "reservations");

            migrationBuilder.DropIndex(
                name: "IX_reservations_BookId",
                table: "reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_loans",
                table: "loans");

            migrationBuilder.DropIndex(
                name: "IX_bookCopies_ShelfId",
                table: "bookCopies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "CopyPrice",
                table: "bookCopies");

            migrationBuilder.DropColumn(
                name: "ShelfId",
                table: "bookCopies");

            migrationBuilder.RenameTable(
                name: "reservations",
                newName: "Reservations");

            migrationBuilder.RenameTable(
                name: "loans",
                newName: "Loans");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "User");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Reviews",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "Reviews",
                newName: "BookID");

            migrationBuilder.RenameColumn(
                name: "ReviewId",
                table: "Reviews",
                newName: "ReviewID");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                newName: "IX_Reviews_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_BookId",
                table: "Reviews",
                newName: "IX_Reviews_BookID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Reservations",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Reservations",
                newName: "BookCopyId");

            migrationBuilder.RenameIndex(
                name: "IX_reservations_UserId",
                table: "Reservations",
                newName: "IX_Reservations_UserID");

            migrationBuilder.RenameColumn(
                name: "loanStatus",
                table: "Loans",
                newName: "LoanStatus");

            migrationBuilder.RenameColumn(
                name: "BookCopyId",
                table: "Loans",
                newName: "BookCopyID");

            migrationBuilder.RenameColumn(
                name: "LoanId",
                table: "Loans",
                newName: "LoanID");

            migrationBuilder.RenameIndex(
                name: "IX_loans_UserID",
                table: "Loans",
                newName: "IX_Loans_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_loans_BookCopyId",
                table: "Loans",
                newName: "IX_Loans_BookCopyID");

            migrationBuilder.RenameColumn(
                name: "LoanId",
                table: "Fines",
                newName: "LoanID");

            migrationBuilder.RenameColumn(
                name: "FineId",
                table: "Fines",
                newName: "FineID");

            migrationBuilder.RenameIndex(
                name: "IX_Fines_LoanId",
                table: "Fines",
                newName: "IX_Fines_LoanID");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Reviews",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ReservationDate",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "ReservationStatus",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "LoanStatus",
                table: "Loans",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LoanReturnDate",
                table: "Loans",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "FinePaymentStatus",
                table: "Fines",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations",
                column: "ReservationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Loans",
                table: "Loans",
                column: "LoanID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_BookCopyId",
                table: "Reservations",
                column: "BookCopyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_Loans_LoanID",
                table: "Fines",
                column: "LoanID",
                principalTable: "Loans",
                principalColumn: "LoanID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_User_UserID",
                table: "Loans",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_bookCopies_BookCopyID",
                table: "Loans",
                column: "BookCopyID",
                principalTable: "bookCopies",
                principalColumn: "BookCopyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_User_UserID",
                table: "Reservations",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_bookCopies_BookCopyId",
                table: "Reservations",
                column: "BookCopyId",
                principalTable: "bookCopies",
                principalColumn: "BookCopyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_User_UserID",
                table: "Reviews",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_books_BookID",
                table: "Reviews",
                column: "BookID",
                principalTable: "books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
