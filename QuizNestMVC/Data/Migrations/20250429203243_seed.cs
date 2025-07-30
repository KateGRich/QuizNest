using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizNestMVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class seed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[,]
                {
                    { "152558a1-7dfd-4f8a-ac27-a0694e6ff19e", "Admin", "ADMIN", DateTime.Now.ToString() },
                    { Guid.NewGuid().ToString(), "Quiz Maker", "QUIZ MAKER", DateTime.Now.ToString() },
                    { Guid.NewGuid().ToString(), "Quiz Taker", "QUIZ TAKER", DateTime.Now.ToString() }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
                    "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "AccessFailedCount",
                    "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled"
                    },
                values: new object[,]
                {
                    { "4f4af597-4b7a-463b-8e43-bd18c8eee952", "admin@quiznest.org", "ADMIN@QUIZNEST.ORG",
                        "admin@quiznest.org", "ADMIN@QUIZNEST.ORG", true,
                        new PasswordHasher<IdentityUser>().HashPassword(null, "P@ssw0rd"),
                        "BIXSQ7WWCLLC6KADKKKLLOYLY672DD7T", DateTime.Now.ToString(), 0, false, false, true
                    }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[,]
                {
                    { "4f4af597-4b7a-463b-8e43-bd18c8eee952", "152558a1-7dfd-4f8a-ac27-a0694e6ff19e" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}