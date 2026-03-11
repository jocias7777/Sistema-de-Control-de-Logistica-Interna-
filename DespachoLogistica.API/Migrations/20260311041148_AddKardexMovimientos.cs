using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DespachoLogistica.API.Migrations
{
    /// <inheritdoc />
    public partial class AddKardexMovimientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KardexMovimientos",
                columns: table => new
                {
                    MovimientoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoID = table.Column<int>(type: "int", nullable: false),
                    BodegaID = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockAntes = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockDespues = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KardexMovimientos", x => x.MovimientoID);
                    table.ForeignKey(
                        name: "FK_KardexMovimientos_Bodegas_BodegaID",
                        column: x => x.BodegaID,
                        principalTable: "Bodegas",
                        principalColumn: "BodegaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KardexMovimientos_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ProductoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KardexMovimientos_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KardexMovimientos_BodegaID",
                table: "KardexMovimientos",
                column: "BodegaID");

            migrationBuilder.CreateIndex(
                name: "IX_KardexMovimientos_ProductoID",
                table: "KardexMovimientos",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_KardexMovimientos_UsuarioID",
                table: "KardexMovimientos",
                column: "UsuarioID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KardexMovimientos");
        }
    }
}
