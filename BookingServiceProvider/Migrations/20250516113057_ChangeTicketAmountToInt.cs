using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingServiceProvider.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTicketAmountToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TicketAmount",
                table: "Bookings",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "TicketAmount",
                table: "Bookings",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
