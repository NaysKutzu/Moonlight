using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moonlight.App.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedShardModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NodeAllocations_Servers_ServerId",
                table: "NodeAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Servers_NodeAllocations_MainAllocationId",
                table: "Servers");

            migrationBuilder.DropForeignKey(
                name: "FK_Servers_Nodes_NodeId",
                table: "Servers");

            migrationBuilder.DropIndex(
                name: "IX_NodeAllocations_ServerId",
                table: "NodeAllocations");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "NodeAllocations");

            migrationBuilder.RenameColumn(
                name: "NodeId",
                table: "Servers",
                newName: "ShardId");

            migrationBuilder.RenameIndex(
                name: "IX_Servers_NodeId",
                table: "Servers",
                newName: "IX_Servers_ShardId");

            migrationBuilder.CreateTable(
                name: "ShardProxies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Fqdn = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Key = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShardProxies", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ShardSpaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProxyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShardSpaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShardSpaces_ShardProxies_ProxyId",
                        column: x => x.ProxyId,
                        principalTable: "ShardProxies",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Shards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fqdn = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TokenId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Token = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SftpPort = table.Column<int>(type: "int", nullable: false),
                    HttpPort = table.Column<int>(type: "int", nullable: false),
                    ShardPort = table.Column<int>(type: "int", nullable: false),
                    Ssl = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ShardSpaceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shards_ShardSpaces_ShardSpaceId",
                        column: x => x.ShardSpaceId,
                        principalTable: "ShardSpaces",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ShardAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Port = table.Column<int>(type: "int", nullable: false),
                    ServerId = table.Column<int>(type: "int", nullable: true),
                    ShardId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShardAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShardAllocations_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShardAllocations_Shards_ShardId",
                        column: x => x.ShardId,
                        principalTable: "Shards",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ShardAllocations_ServerId",
                table: "ShardAllocations",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_ShardAllocations_ShardId",
                table: "ShardAllocations",
                column: "ShardId");

            migrationBuilder.CreateIndex(
                name: "IX_Shards_ShardSpaceId",
                table: "Shards",
                column: "ShardSpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_ShardSpaces_ProxyId",
                table: "ShardSpaces",
                column: "ProxyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_ShardAllocations_MainAllocationId",
                table: "Servers",
                column: "MainAllocationId",
                principalTable: "ShardAllocations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_Shards_ShardId",
                table: "Servers",
                column: "ShardId",
                principalTable: "Shards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servers_ShardAllocations_MainAllocationId",
                table: "Servers");

            migrationBuilder.DropForeignKey(
                name: "FK_Servers_Shards_ShardId",
                table: "Servers");

            migrationBuilder.DropTable(
                name: "ShardAllocations");

            migrationBuilder.DropTable(
                name: "Shards");

            migrationBuilder.DropTable(
                name: "ShardSpaces");

            migrationBuilder.DropTable(
                name: "ShardProxies");

            migrationBuilder.RenameColumn(
                name: "ShardId",
                table: "Servers",
                newName: "NodeId");

            migrationBuilder.RenameIndex(
                name: "IX_Servers_ShardId",
                table: "Servers",
                newName: "IX_Servers_NodeId");

            migrationBuilder.AddColumn<int>(
                name: "ServerId",
                table: "NodeAllocations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NodeAllocations_ServerId",
                table: "NodeAllocations",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_NodeAllocations_Servers_ServerId",
                table: "NodeAllocations",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_NodeAllocations_MainAllocationId",
                table: "Servers",
                column: "MainAllocationId",
                principalTable: "NodeAllocations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_Nodes_NodeId",
                table: "Servers",
                column: "NodeId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
