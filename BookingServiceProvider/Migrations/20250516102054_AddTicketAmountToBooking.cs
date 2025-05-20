using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingServiceProvider.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketAmountToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TicketAmount",
                table: "Bookings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketAmount",
                table: "Bookings");
        }
    }
}
