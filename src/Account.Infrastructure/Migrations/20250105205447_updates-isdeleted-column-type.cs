﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdatesIsDeletedColumnType : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<bool>(
            name: "IsDeleted",
            table: "ApplicationUsers",
            type: "bit",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "IsDeleted",
            table: "ApplicationUsers",
            type: "int",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "bit");
    }
}
