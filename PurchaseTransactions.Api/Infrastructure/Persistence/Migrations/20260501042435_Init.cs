using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseTransactions.Api.Infrastructure.Persistence.Migrations
{
  /// <inheritdoc />
  public partial class Init : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "purchase_transactions",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            Description = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            AmountUsd = table.Column<decimal>(type: "numeric(19,2)", precision: 19, scale: 2, nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_purchase_transactions", x => x.Id);
          });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "purchase_transactions");
    }
  }
}
