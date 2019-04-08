using Microsoft.EntityFrameworkCore.Migrations;

namespace RichDomainModel.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(nullable: true),
                    Answer = table.Column<string>(nullable: true),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuestionId = table.Column<int>(nullable: true),
                    NotUsed = table.Column<bool>(nullable: false),
                    GameCategoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameQuestion_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    GameId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Place = table.Column<int>(nullable: false),
                    IsInPenaltyBox = table.Column<bool>(nullable: false),
                    GoldCoins = table.Column<int>(nullable: false),
                    LastQuestionId = table.Column<int>(nullable: true),
                    GameId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_Question_LastQuestionId",
                        column: x => x.LastQuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    CurrentPlayerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Player_CurrentPlayerId",
                        column: x => x.CurrentPlayerId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Sports" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 1, "sport 1", 1, "Sport 1" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 73, "sport 73", 1, "Sport 73" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 72, "sport 72", 1, "Sport 72" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 71, "sport 71", 1, "Sport 71" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 70, "sport 70", 1, "Sport 70" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 69, "sport 69", 1, "Sport 69" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 68, "sport 68", 1, "Sport 68" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 67, "sport 67", 1, "Sport 67" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 66, "sport 66", 1, "Sport 66" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 65, "sport 65", 1, "Sport 65" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 64, "sport 64", 1, "Sport 64" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 63, "sport 63", 1, "Sport 63" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 62, "sport 62", 1, "Sport 62" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 61, "sport 61", 1, "Sport 61" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 60, "sport 60", 1, "Sport 60" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 59, "sport 59", 1, "Sport 59" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 58, "sport 58", 1, "Sport 58" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 57, "sport 57", 1, "Sport 57" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 56, "sport 56", 1, "Sport 56" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 55, "sport 55", 1, "Sport 55" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 54, "sport 54", 1, "Sport 54" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 53, "sport 53", 1, "Sport 53" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 74, "sport 74", 1, "Sport 74" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 52, "sport 52", 1, "Sport 52" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 75, "sport 75", 1, "Sport 75" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 77, "sport 77", 1, "Sport 77" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 98, "sport 98", 1, "Sport 98" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 97, "sport 97", 1, "Sport 97" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 96, "sport 96", 1, "Sport 96" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 95, "sport 95", 1, "Sport 95" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 94, "sport 94", 1, "Sport 94" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 93, "sport 93", 1, "Sport 93" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 92, "sport 92", 1, "Sport 92" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 91, "sport 91", 1, "Sport 91" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 90, "sport 90", 1, "Sport 90" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 89, "sport 89", 1, "Sport 89" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 88, "sport 88", 1, "Sport 88" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 87, "sport 87", 1, "Sport 87" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 86, "sport 86", 1, "Sport 86" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 85, "sport 85", 1, "Sport 85" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 84, "sport 84", 1, "Sport 84" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 83, "sport 83", 1, "Sport 83" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 82, "sport 82", 1, "Sport 82" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 81, "sport 81", 1, "Sport 81" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 80, "sport 80", 1, "Sport 80" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 79, "sport 79", 1, "Sport 79" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 78, "sport 78", 1, "Sport 78" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 76, "sport 76", 1, "Sport 76" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 51, "sport 51", 1, "Sport 51" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 50, "sport 50", 1, "Sport 50" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 49, "sport 49", 1, "Sport 49" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 22, "sport 22", 1, "Sport 22" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 21, "sport 21", 1, "Sport 21" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 20, "sport 20", 1, "Sport 20" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 19, "sport 19", 1, "Sport 19" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 18, "sport 18", 1, "Sport 18" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 17, "sport 17", 1, "Sport 17" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 16, "sport 16", 1, "Sport 16" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 15, "sport 15", 1, "Sport 15" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 14, "sport 14", 1, "Sport 14" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 13, "sport 13", 1, "Sport 13" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 12, "sport 12", 1, "Sport 12" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 11, "sport 11", 1, "Sport 11" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 10, "sport 10", 1, "Sport 10" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 9, "sport 9", 1, "Sport 9" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 8, "sport 8", 1, "Sport 8" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 7, "sport 7", 1, "Sport 7" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 6, "sport 6", 1, "Sport 6" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 5, "sport 5", 1, "Sport 5" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 4, "sport 4", 1, "Sport 4" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 3, "sport 3", 1, "Sport 3" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 2, "sport 2", 1, "Sport 2" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 23, "sport 23", 1, "Sport 23" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 24, "sport 24", 1, "Sport 24" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 25, "sport 25", 1, "Sport 25" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 26, "sport 26", 1, "Sport 26" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 48, "sport 48", 1, "Sport 48" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 47, "sport 47", 1, "Sport 47" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 46, "sport 46", 1, "Sport 46" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 45, "sport 45", 1, "Sport 45" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 44, "sport 44", 1, "Sport 44" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 43, "sport 43", 1, "Sport 43" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 42, "sport 42", 1, "Sport 42" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 41, "sport 41", 1, "Sport 41" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 40, "sport 40", 1, "Sport 40" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 39, "sport 39", 1, "Sport 39" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 99, "sport 99", 1, "Sport 99" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 38, "sport 38", 1, "Sport 38" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 36, "sport 36", 1, "Sport 36" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 35, "sport 35", 1, "Sport 35" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 34, "sport 34", 1, "Sport 34" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 33, "sport 33", 1, "Sport 33" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 32, "sport 32", 1, "Sport 32" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 31, "sport 31", 1, "Sport 31" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 30, "sport 30", 1, "Sport 30" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 29, "sport 29", 1, "Sport 29" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 28, "sport 28", 1, "Sport 28" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 27, "sport 27", 1, "Sport 27" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 37, "sport 37", 1, "Sport 37" });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "Id", "Answer", "CategoryId", "Text" },
                values: new object[] { 100, "sport 100", 1, "Sport 100" });

            migrationBuilder.CreateIndex(
                name: "IX_GameCategory_GameId",
                table: "GameCategory",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameQuestion_GameCategoryId",
                table: "GameQuestion",
                column: "GameCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GameQuestion_QuestionId",
                table: "GameQuestion",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_CurrentPlayerId",
                table: "Games",
                column: "CurrentPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_GameId",
                table: "Player",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_LastQuestionId",
                table: "Player",
                column: "LastQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_CategoryId",
                table: "Question",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameQuestion_GameCategory_GameCategoryId",
                table: "GameQuestion",
                column: "GameCategoryId",
                principalTable: "GameCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GameCategory_Games_GameId",
                table: "GameCategory",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Player_Games_GameId",
                table: "Player",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Player_Games_GameId",
                table: "Player");

            migrationBuilder.DropTable(
                name: "GameQuestion");

            migrationBuilder.DropTable(
                name: "GameCategory");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
