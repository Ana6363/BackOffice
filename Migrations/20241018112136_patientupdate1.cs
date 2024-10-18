using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackOffice.Migrations
{
    /// <inheritdoc />
    public partial class patientupdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Patients",
                newName: "RecordNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecordNumber",
                table: "Patients",
                newName: "Id");
        }
    }
}
