using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Erp.Documents.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentValidationFlows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentValidationFlows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    BucketKey = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidationStatus = table.Column<int>(type: "int", nullable: true),
                    ValidationFlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Documents_DocumentValidationFlows_ValidationFlowId",
                        column: x => x.ValidationFlowId,
                        principalTable: "DocumentValidationFlows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ValidationSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ApproverUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValidationSteps_DocumentValidationFlows_FlowId",
                        column: x => x.FlowId,
                        principalTable: "DocumentValidationFlows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValidationActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    ActorUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValidationActions_DocumentValidationFlows_FlowId",
                        column: x => x.FlowId,
                        principalTable: "DocumentValidationFlows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ValidationActions_ValidationSteps_StepId",
                        column: x => x.StepId,
                        principalTable: "ValidationSteps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_BucketKey",
                table: "Documents",
                column: "BucketKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CompanyId",
                table: "Documents",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CompanyId_EntityType_EntityId",
                table: "Documents",
                columns: new[] { "CompanyId", "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ValidationFlowId",
                table: "Documents",
                column: "ValidationFlowId",
                unique: true,
                filter: "[ValidationFlowId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ValidationActions_FlowId",
                table: "ValidationActions",
                column: "FlowId");

            migrationBuilder.CreateIndex(
                name: "IX_ValidationActions_StepId",
                table: "ValidationActions",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_ValidationSteps_FlowId",
                table: "ValidationSteps",
                column: "FlowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "ValidationActions");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "ValidationSteps");

            migrationBuilder.DropTable(
                name: "DocumentValidationFlows");
        }
    }
}
