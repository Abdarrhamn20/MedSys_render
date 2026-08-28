using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChartAccounts",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AccountName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AccountNameAr = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AccountType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Asset"),
                    ParentAccountID = table.Column<int>(type: "integer", nullable: true),
                    OpeningBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartAccounts", x => x.AccountID);
                    table.ForeignKey(
                        name: "FK_ChartAccounts_ChartAccounts_ParentAccountID",
                        column: x => x.ParentAccountID,
                        principalTable: "ChartAccounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HealthServices",
                columns: table => new
                {
                    ServiceID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ServiceNameAr = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthServices", x => x.ServiceID);
                });

            migrationBuilder.CreateTable(
                name: "InventoryCategories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CategoryNameAr = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ParentCategoryID = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryCategories", x => x.CategoryID);
                    table.ForeignKey(
                        name: "FK_InventoryCategories_InventoryCategories_ParentCategoryID",
                        column: x => x.ParentCategoryID,
                        principalTable: "InventoryCategories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabDevices",
                columns: table => new
                {
                    LabDeviceID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DeviceCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DeviceModel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ConnectionType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabDevices", x => x.LabDeviceID);
                });

            migrationBuilder.CreateTable(
                name: "Medications",
                columns: table => new
                {
                    MedicationID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NameAr = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DosageForm = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    QuantityInStock = table.Column<int>(type: "integer", nullable: false),
                    MinStockLevel = table.Column<int>(type: "integer", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Manufacturer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medications", x => x.MedicationID);
                });

            migrationBuilder.CreateTable(
                name: "Priorities",
                columns: table => new
                {
                    PriorityID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LevelName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    LevelNameAr = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false),
                    ColorCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Icon = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Priorities", x => x.PriorityID);
                });

            migrationBuilder.CreateTable(
                name: "RadiologyTemplates",
                columns: table => new
                {
                    TemplateID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TemplateName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Modality = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BodyPart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DefaultReportText = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyTemplates", x => x.TemplateID);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    SettingKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SettingValue = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.SettingKey);
                });

            migrationBuilder.CreateTable(
                name: "TriageQuestions",
                columns: table => new
                {
                    QuestionID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionText = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    QuestionTextAr = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriageQuestions", x => x.QuestionID);
                });

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    WardID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WardName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    WardNameAr = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GenderType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FloorNumber = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.WardID);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    WarehouseID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WarehouseName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    WarehouseNameAr = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    WarehouseCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.WarehouseID);
                });

            migrationBuilder.CreateTable(
                name: "Treasuries",
                columns: table => new
                {
                    TreasuryID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TreasuryName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TreasuryNameAr = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TreasuryCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AccountID = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Treasuries", x => x.TreasuryID);
                    table.ForeignKey(
                        name: "FK_Treasuries_ChartAccounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "ChartAccounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabTests",
                columns: table => new
                {
                    LabTestID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsPanel = table.Column<bool>(type: "boolean", nullable: false),
                    PanelID = table.Column<int>(type: "integer", nullable: true),
                    DeviceID = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTests", x => x.LabTestID);
                    table.ForeignKey(
                        name: "FK_LabTests_LabDevices_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "LabDevices",
                        principalColumn: "LabDeviceID");
                    table.ForeignKey(
                        name: "FK_LabTests_LabTests_PanelID",
                        column: x => x.PanelID,
                        principalTable: "LabTests",
                        principalColumn: "LabTestID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    ItemID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ItemNameAr = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CategoryID = table.Column<int>(type: "integer", nullable: false),
                    MedicationID = table.Column<int>(type: "integer", nullable: true),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ReorderLevel = table.Column<int>(type: "integer", nullable: false),
                    Manufacturer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.ItemID);
                    table.ForeignKey(
                        name: "FK_InventoryItems_InventoryCategories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "InventoryCategories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Medications_MedicationID",
                        column: x => x.MedicationID,
                        principalTable: "Medications",
                        principalColumn: "MedicationID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    RoomID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WardID = table.Column<int>(type: "integer", nullable: false),
                    RoomNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RoomType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DailyRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MaxBeds = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.RoomID);
                    table.ForeignKey(
                        name: "FK_Rooms_Wards_WardID",
                        column: x => x.WardID,
                        principalTable: "Wards",
                        principalColumn: "WardID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Patient"),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AssignedTreasuryID = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_Users_Treasuries_AssignedTreasuryID",
                        column: x => x.AssignedTreasuryID,
                        principalTable: "Treasuries",
                        principalColumn: "TreasuryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabReferenceRanges",
                columns: table => new
                {
                    RangeID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LabTestID = table.Column<int>(type: "integer", nullable: false),
                    Gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    MinAge = table.Column<int>(type: "integer", nullable: false),
                    MaxAge = table.Column<int>(type: "integer", nullable: false),
                    NormalMin = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NormalMax = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    RangeNotes = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabReferenceRanges", x => x.RangeID);
                    table.ForeignKey(
                        name: "FK_LabReferenceRanges_LabTests_LabTestID",
                        column: x => x.LabTestID,
                        principalTable: "LabTests",
                        principalColumn: "LabTestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Beds",
                columns: table => new
                {
                    BedID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoomID = table.Column<int>(type: "integer", nullable: false),
                    BedNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beds", x => x.BedID);
                    table.ForeignKey(
                        name: "FK_Beds_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "RoomID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityID = table.Column<int>(type: "integer", nullable: false),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    Details = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.LogID);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorCommissions",
                columns: table => new
                {
                    CommissionID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DoctorID = table.Column<int>(type: "integer", nullable: false),
                    Specialty = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ServiceID = table.Column<int>(type: "integer", nullable: true),
                    CommissionType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorCommissions", x => x.CommissionID);
                    table.ForeignKey(
                        name: "FK_DoctorCommissions_Users_DoctorID",
                        column: x => x.DoctorID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorProfiles",
                columns: table => new
                {
                    DoctorID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    Specialty = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LicenseNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EmergencyReady = table.Column<bool>(type: "boolean", nullable: false),
                    Bio = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    AvailableDays = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    WorkStartTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    WorkEndTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ConsultationDurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    ConsultationFee = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorProfiles", x => x.DoctorID);
                    table.ForeignKey(
                        name: "FK_DoctorProfiles_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProfiles",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(type: "integer", nullable: true),
                    EmployeeNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Position = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    HireDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NationalID = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CompensationModel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "FixedSalary"),
                    BaseSalary = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    BankAccount = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProfiles", x => x.EmployeeID);
                    table.ForeignKey(
                        name: "FK_EmployeeProfiles_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntries",
                columns: table => new
                {
                    JournalEntryID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntryNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SourceModule = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SourceReferenceID = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Draft"),
                    CreatedByUserID = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PostedByUserID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntries", x => x.JournalEntryID);
                    table.ForeignKey(
                        name: "FK_JournalEntries_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalEntries_Users_PostedByUserID",
                        column: x => x.PostedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabOrders",
                columns: table => new
                {
                    LabOrderID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PatientUserID = table.Column<int>(type: "integer", nullable: false),
                    DoctorID = table.Column<int>(type: "integer", nullable: false),
                    LabTestID = table.Column<int>(type: "integer", nullable: false),
                    ResultValue = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ResultStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ResultNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TechnicianNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    VerificationQRCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabOrders", x => x.LabOrderID);
                    table.ForeignKey(
                        name: "FK_LabOrders_LabTests_LabTestID",
                        column: x => x.LabTestID,
                        principalTable: "LabTests",
                        principalColumn: "LabTestID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabOrders_Users_DoctorID",
                        column: x => x.DoctorID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabOrders_Users_PatientUserID",
                        column: x => x.PatientUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicationRequests",
                columns: table => new
                {
                    RequestID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MedicationName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DoctorUserID = table.Column<int>(type: "integer", nullable: false),
                    DoctorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationRequests", x => x.RequestID);
                    table.ForeignKey(
                        name: "FK_MedicationRequests_Users_DoctorUserID",
                        column: x => x.DoctorUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientProfiles",
                columns: table => new
                {
                    PatientID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FatherName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    GrandfatherName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FamilyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FileNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    MergedIntoPatientID = table.Column<int>(type: "integer", nullable: true),
                    MergedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BloodType = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    ChronicDiseases = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Allergies = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    GeneralNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EmergencyContact = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmergencyPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    RiskLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, defaultValue: "Stable"),
                    RiskLevelUpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RiskLevelUpdatedByUserID = table.Column<int>(type: "integer", nullable: true),
                    RiskLevelNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientProfiles", x => x.PatientID);
                    table.ForeignKey(
                        name: "FK_PatientProfiles_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RadiologyOrders",
                columns: table => new
                {
                    RadiologyOrderID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PatientUserID = table.Column<int>(type: "integer", nullable: false),
                    DoctorID = table.Column<int>(type: "integer", nullable: false),
                    Modality = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BodyPart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ReportText = table.Column<string>(type: "text", nullable: true),
                    ImagePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RadiologistID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyOrders", x => x.RadiologyOrderID);
                    table.ForeignKey(
                        name: "FK_RadiologyOrders_Users_DoctorID",
                        column: x => x.DoctorID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadiologyOrders_Users_PatientUserID",
                        column: x => x.PatientUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadiologyOrders_Users_RadiologistID",
                        column: x => x.RadiologistID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockCounts",
                columns: table => new
                {
                    StockCountID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StockCountNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CountDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    WarehouseID = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Draft"),
                    Notes = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    CreatedByUserID = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PostedByUserID = table.Column<int>(type: "integer", nullable: true),
                    PostedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReversedByUserID = table.Column<int>(type: "integer", nullable: true),
                    ReversedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockCounts", x => x.StockCountID);
                    table.ForeignKey(
                        name: "FK_StockCounts_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockCounts_Users_PostedByUserID",
                        column: x => x.PostedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockCounts_Users_ReversedByUserID",
                        column: x => x.ReversedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockCounts_Warehouses_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: table => new
                {
                    MovementID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MovementNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    MovementType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MovementDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    WarehouseID = table.Column<int>(type: "integer", nullable: false),
                    ToWarehouseID = table.Column<int>(type: "integer", nullable: true),
                    ReferenceType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ReferenceID = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Draft"),
                    CreatedByUserID = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PostedByUserID = table.Column<int>(type: "integer", nullable: true),
                    PostedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovements", x => x.MovementID);
                    table.ForeignKey(
                        name: "FK_StockMovements_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Users_PostedByUserID",
                        column: x => x.PostedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Warehouses_ToWarehouseID",
                        column: x => x.ToWarehouseID,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Warehouses_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RelatedEntityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RelatedEntityID = table.Column<int>(type: "integer", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_UserNotifications_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebPushSubscriptions",
                columns: table => new
                {
                    SubscriptionID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    Endpoint = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    P256DH = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Auth = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebPushSubscriptions", x => x.SubscriptionID);
                    table.ForeignKey(
                        name: "FK_WebPushSubscriptions_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomAssessmentTemplates",
                columns: table => new
                {
                    TemplateID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DoctorID = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SchemaJson = table.Column<string>(type: "text", nullable: false),
                    TemplateType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsStandard = table.Column<bool>(type: "boolean", nullable: false),
                    MaxScore = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomAssessmentTemplates", x => x.TemplateID);
                    table.ForeignKey(
                        name: "FK_CustomAssessmentTemplates_DoctorProfiles_DoctorID",
                        column: x => x.DoctorID,
                        principalTable: "DoctorProfiles",
                        principalColumn: "DoctorID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeCourses",
                columns: table => new
                {
                    CourseID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeID = table.Column<int>(type: "integer", nullable: false),
                    CourseName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CourseDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CertificateNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeCourses", x => x.CourseID);
                    table.ForeignKey(
                        name: "FK_EmployeeCourses_EmployeeProfiles_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "EmployeeProfiles",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeLeaves",
                columns: table => new
                {
                    LeaveID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeID = table.Column<int>(type: "integer", nullable: false),
                    LeaveType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Days = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    ApprovedByUserID = table.Column<int>(type: "integer", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeLeaves", x => x.LeaveID);
                    table.ForeignKey(
                        name: "FK_EmployeeLeaves_EmployeeProfiles_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "EmployeeProfiles",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeLeaves_Users_ApprovedByUserID",
                        column: x => x.ApprovedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntryLines",
                columns: table => new
                {
                    JournalEntryLineID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JournalEntryID = table.Column<int>(type: "integer", nullable: false),
                    AccountID = table.Column<int>(type: "integer", nullable: false),
                    Debit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Credit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntryLines", x => x.JournalEntryLineID);
                    table.ForeignKey(
                        name: "FK_JournalEntryLines_ChartAccounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "ChartAccounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalEntryLines_JournalEntries_JournalEntryID",
                        column: x => x.JournalEntryID,
                        principalTable: "JournalEntries",
                        principalColumn: "JournalEntryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalaryRecords",
                columns: table => new
                {
                    SalaryRecordID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeID = table.Column<int>(type: "integer", nullable: false),
                    PeriodYear = table.Column<int>(type: "integer", nullable: false),
                    PeriodMonth = table.Column<int>(type: "integer", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Bonus = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Deduction = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    GrossSalary = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NetSalary = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Draft"),
                    JournalEntryID = table.Column<int>(type: "integer", nullable: true),
                    CreatedByUserID = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryRecords", x => x.SalaryRecordID);
                    table.ForeignKey(
                        name: "FK_SalaryRecords_EmployeeProfiles_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "EmployeeProfiles",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalaryRecords_JournalEntries_JournalEntryID",
                        column: x => x.JournalEntryID,
                        principalTable: "JournalEntries",
                        principalColumn: "JournalEntryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalaryRecords_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabOrderItems",
                columns: table => new
                {
                    LabOrderItemID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LabOrderID = table.Column<int>(type: "integer", nullable: false),
                    LabTestID = table.Column<int>(type: "integer", nullable: false),
                    ResultValue = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ResultStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TechnicianNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabOrderItems", x => x.LabOrderItemID);
                    table.ForeignKey(
                        name: "FK_LabOrderItems_LabOrders_LabOrderID",
                        column: x => x.LabOrderID,
                        principalTable: "LabOrders",
                        principalColumn: "LabOrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabOrderItems_LabTests_LabTestID",
                        column: x => x.LabTestID,
                        principalTable: "LabTests",
                        principalColumn: "LabTestID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Admissions",
                columns: table => new
                {
                    AdmissionID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PatientID = table.Column<int>(type: "integer", nullable: false),
                    DoctorID = table.Column<int>(type: "integer", nullable: false),
                    BedID = table.Column<int>(type: "integer", nullable: false),
                    AdmissionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DischargeDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AdmissionReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DischargeSummary = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admissions", x => x.AdmissionID);
                    table.ForeignKey(
                        name: "FK_Admissions_Beds_BedID",
                        column: x => x.BedID,
                        principalTable: "Beds",
                        principalColumn: "BedID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Admissions_DoctorProfiles_DoctorID",
                        column: x => x.DoctorID,
                        principalTable: "DoctorProfiles",
                        principalColumn: "DoctorID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Admissions_PatientProfiles_PatientID",
                        column: x => x.PatientID,
                        principalTable: "PatientProfiles",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    AppID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PatientID = table.Column<int>(type: "integer", nullable: false),
                    DoctorID = table.Column<int>(type: "integer", nullable: false),
                    PriorityID = table.Column<int>(type: "integer", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AppointmentTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    TriageScore = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AppointmentType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    QueueNumber = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.AppID);
                    table.ForeignKey(
                        name: "FK_Appointments_DoctorProfiles_DoctorID",
                        column: x => x.DoctorID,
                        principalTable: "DoctorProfiles",
                        principalColumn: "DoctorID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_PatientProfiles_PatientID",
                        column: x => x.PatientID,
                        principalTable: "PatientProfiles",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Priorities_PriorityID",
                        column: x => x.PriorityID,
                        principalTable: "Priorities",
                        principalColumn: "PriorityID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockCountItems",
                columns: table => new
                {
                    StockCountItemID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StockCountID = table.Column<int>(type: "integer", nullable: false),
                    ItemID = table.Column<int>(type: "integer", nullable: false),
                    SystemQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CountedQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockCountItems", x => x.StockCountItemID);
                    table.ForeignKey(
                        name: "FK_StockCountItems_InventoryItems_ItemID",
                        column: x => x.ItemID,
                        principalTable: "InventoryItems",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockCountItems_StockCounts_StockCountID",
                        column: x => x.StockCountID,
                        principalTable: "StockCounts",
                        principalColumn: "StockCountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockMovementItems",
                columns: table => new
                {
                    StockMovementItemID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MovementID = table.Column<int>(type: "integer", nullable: false),
                    ItemID = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovementItems", x => x.StockMovementItemID);
                    table.ForeignKey(
                        name: "FK_StockMovementItems_InventoryItems_ItemID",
                        column: x => x.ItemID,
                        principalTable: "InventoryItems",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovementItems_StockMovements_MovementID",
                        column: x => x.MovementID,
                        principalTable: "StockMovements",
                        principalColumn: "MovementID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientAssessments",
                columns: table => new
                {
                    AssessmentID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PatientUserID = table.Column<int>(type: "integer", nullable: false),
                    TemplateID = table.Column<int>(type: "integer", nullable: false),
                    AnswersJson = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientAssessments", x => x.AssessmentID);
                    table.ForeignKey(
                        name: "FK_PatientAssessments_CustomAssessmentTemplates_TemplateID",
                        column: x => x.TemplateID,
                        principalTable: "CustomAssessmentTemplates",
                        principalColumn: "TemplateID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientAssessments_Users_PatientUserID",
                        column: x => x.PatientUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CultureSensitivities",
                columns: table => new
                {
                    CultureSensitivityID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LabOrderItemID = table.Column<int>(type: "integer", nullable: false),
                    Organism = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    GramStain = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CultureStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    QuantitativeResult = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CultureSensitivities", x => x.CultureSensitivityID);
                    table.ForeignKey(
                        name: "FK_CultureSensitivities_LabOrderItems_LabOrderItemID",
                        column: x => x.LabOrderItemID,
                        principalTable: "LabOrderItems",
                        principalColumn: "LabOrderItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InpatientCareOrders",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdmissionID = table.Column<int>(type: "integer", nullable: false),
                    HealthServiceID = table.Column<int>(type: "integer", nullable: true),
                    OrderType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    OrderDescription = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Frequency = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ScheduledTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InpatientCareOrders", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_InpatientCareOrders_Admissions_AdmissionID",
                        column: x => x.AdmissionID,
                        principalTable: "Admissions",
                        principalColumn: "AdmissionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InpatientCareOrders_HealthServices_HealthServiceID",
                        column: x => x.HealthServiceID,
                        principalTable: "HealthServices",
                        principalColumn: "ServiceID");
                    table.ForeignKey(
                        name: "FK_InpatientCareOrders_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InpatientDailyLogs",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdmissionID = table.Column<int>(type: "integer", nullable: false),
                    LoggedByUserID = table.Column<int>(type: "integer", nullable: false),
                    LogDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Temperature = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BloodPressure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PulseRate = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    OxygenLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DoctorNotes = table.Column<string>(type: "text", nullable: true),
                    NursingNotes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InpatientDailyLogs", x => x.LogID);
                    table.ForeignKey(
                        name: "FK_InpatientDailyLogs_Admissions_AdmissionID",
                        column: x => x.AdmissionID,
                        principalTable: "Admissions",
                        principalColumn: "AdmissionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InpatientDailyLogs_Users_LoggedByUserID",
                        column: x => x.LoggedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                columns: table => new
                {
                    RecordID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppID = table.Column<int>(type: "integer", nullable: false),
                    Diagnosis = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DiagnosisAr = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TreatmentPlan = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DoctorNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Symptoms = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Recommendations = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RequiresFollowUp = table.Column<bool>(type: "boolean", nullable: false),
                    FollowUpDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FollowUpNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecords", x => x.RecordID);
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Appointments_AppID",
                        column: x => x.AppID,
                        principalTable: "Appointments",
                        principalColumn: "AppID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelemedicineSessions",
                columns: table => new
                {
                    SessionID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppointmentID = table.Column<int>(type: "integer", nullable: false),
                    RoomCode = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedByUserID = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SessionNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemedicineSessions", x => x.SessionID);
                    table.ForeignKey(
                        name: "FK_TelemedicineSessions_Appointments_AppointmentID",
                        column: x => x.AppointmentID,
                        principalTable: "Appointments",
                        principalColumn: "AppID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensitivityResults",
                columns: table => new
                {
                    SensitivityResultID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CultureSensitivityID = table.Column<int>(type: "integer", nullable: false),
                    AntibioticName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Interpretation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ZoneDiameter = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensitivityResults", x => x.SensitivityResultID);
                    table.ForeignKey(
                        name: "FK_SensitivityResults_CultureSensitivities_CultureSensitivityID",
                        column: x => x.CultureSensitivityID,
                        principalTable: "CultureSensitivities",
                        principalColumn: "CultureSensitivityID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InpatientCareExecutions",
                columns: table => new
                {
                    ExecutionID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderID = table.Column<int>(type: "integer", nullable: false),
                    ExecutedByUserID = table.Column<int>(type: "integer", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    VitalTemperature = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    VitalBloodPressure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    VitalPulse = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    VitalOxygen = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InpatientCareExecutions", x => x.ExecutionID);
                    table.ForeignKey(
                        name: "FK_InpatientCareExecutions_InpatientCareOrders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "InpatientCareOrders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InpatientCareExecutions_Users_ExecutedByUserID",
                        column: x => x.ExecutedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    AttachmentID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecordID = table.Column<int>(type: "integer", nullable: true),
                    PatientID = table.Column<int>(type: "integer", nullable: true),
                    FileName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FileType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FileURL = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.AttachmentID);
                    table.ForeignKey(
                        name: "FK_Attachments_MedicalRecords_RecordID",
                        column: x => x.RecordID,
                        principalTable: "MedicalRecords",
                        principalColumn: "RecordID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Attachments_PatientProfiles_PatientID",
                        column: x => x.PatientID,
                        principalTable: "PatientProfiles",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    PrescriptionID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecordID = table.Column<int>(type: "integer", nullable: false),
                    MedicationID = table.Column<int>(type: "integer", nullable: true),
                    MedicationName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Dosage = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Duration = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Instructions = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Frequency = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    DispenseStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.PrescriptionID);
                    table.ForeignKey(
                        name: "FK_Prescriptions_MedicalRecords_RecordID",
                        column: x => x.RecordID,
                        principalTable: "MedicalRecords",
                        principalColumn: "RecordID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prescriptions_Medications_MedicationID",
                        column: x => x.MedicationID,
                        principalTable: "Medications",
                        principalColumn: "MedicationID");
                });

            migrationBuilder.CreateTable(
                name: "PsychiatricRecords",
                columns: table => new
                {
                    RecordID = table.Column<int>(type: "integer", nullable: false),
                    Appearance = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Behavior = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Speech = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    MoodAndAffect = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ThoughtProcess = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ThoughtContent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Perception = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Cognition = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    InsightAndJudgment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsSpeechToTextUsed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsychiatricRecords", x => x.RecordID);
                    table.ForeignKey(
                        name: "FK_PsychiatricRecords_MedicalRecords_RecordID",
                        column: x => x.RecordID,
                        principalTable: "MedicalRecords",
                        principalColumn: "RecordID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SoapNotes",
                columns: table => new
                {
                    SoapNoteID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecordID = table.Column<int>(type: "integer", nullable: false),
                    Subjective = table.Column<string>(type: "text", maxLength: 2147483647, nullable: true),
                    Objective = table.Column<string>(type: "text", maxLength: 2147483647, nullable: true),
                    Assessment = table.Column<string>(type: "text", maxLength: 2147483647, nullable: true),
                    Plan = table.Column<string>(type: "text", maxLength: 2147483647, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoapNotes", x => x.SoapNoteID);
                    table.ForeignKey(
                        name: "FK_SoapNotes_MedicalRecords_RecordID",
                        column: x => x.RecordID,
                        principalTable: "MedicalRecords",
                        principalColumn: "RecordID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DispenseRecords",
                columns: table => new
                {
                    DispenseID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrescriptionID = table.Column<int>(type: "integer", nullable: false),
                    MedicationID = table.Column<int>(type: "integer", nullable: true),
                    QuantityDispensed = table.Column<int>(type: "integer", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DispensedByUserID = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Notes = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    DispensedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispenseRecords", x => x.DispenseID);
                    table.ForeignKey(
                        name: "FK_DispenseRecords_Medications_MedicationID",
                        column: x => x.MedicationID,
                        principalTable: "Medications",
                        principalColumn: "MedicationID");
                    table.ForeignKey(
                        name: "FK_DispenseRecords_Prescriptions_PrescriptionID",
                        column: x => x.PrescriptionID,
                        principalTable: "Prescriptions",
                        principalColumn: "PrescriptionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DispenseRecords_Users_DispensedByUserID",
                        column: x => x.DispensedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PatientUserID = table.Column<int>(type: "integer", nullable: false),
                    AppointmentID = table.Column<int>(type: "integer", nullable: true),
                    DispenseRecordID = table.Column<int>(type: "integer", nullable: true),
                    LabOrderID = table.Column<int>(type: "integer", nullable: true),
                    RadiologyOrderID = table.Column<int>(type: "integer", nullable: true),
                    InvoiceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Tax = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TransactionReference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DoctorShare = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ClinicShare = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DoctorID = table.Column<int>(type: "integer", nullable: true),
                    DoctorCommissionID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceID);
                    table.ForeignKey(
                        name: "FK_Invoices_Appointments_AppointmentID",
                        column: x => x.AppointmentID,
                        principalTable: "Appointments",
                        principalColumn: "AppID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_DispenseRecords_DispenseRecordID",
                        column: x => x.DispenseRecordID,
                        principalTable: "DispenseRecords",
                        principalColumn: "DispenseID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_DoctorCommissions_DoctorCommissionID",
                        column: x => x.DoctorCommissionID,
                        principalTable: "DoctorCommissions",
                        principalColumn: "CommissionID");
                    table.ForeignKey(
                        name: "FK_Invoices_LabOrders_LabOrderID",
                        column: x => x.LabOrderID,
                        principalTable: "LabOrders",
                        principalColumn: "LabOrderID");
                    table.ForeignKey(
                        name: "FK_Invoices_RadiologyOrders_RadiologyOrderID",
                        column: x => x.RadiologyOrderID,
                        principalTable: "RadiologyOrders",
                        principalColumn: "RadiologyOrderID");
                    table.ForeignKey(
                        name: "FK_Invoices_Users_DoctorID",
                        column: x => x.DoctorID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_Invoices_Users_PatientUserID",
                        column: x => x.PatientUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vouchers",
                columns: table => new
                {
                    VoucherID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VoucherNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    VoucherType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    VoucherDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TreasuryID = table.Column<int>(type: "integer", nullable: false),
                    ToTreasuryID = table.Column<int>(type: "integer", nullable: true),
                    AccountID = table.Column<int>(type: "integer", nullable: true),
                    PatientUserID = table.Column<int>(type: "integer", nullable: true),
                    InvoiceID = table.Column<int>(type: "integer", nullable: true),
                    AppointmentID = table.Column<int>(type: "integer", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Draft"),
                    CreatedByUserID = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PostedByUserID = table.Column<int>(type: "integer", nullable: true),
                    PostedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.VoucherID);
                    table.ForeignKey(
                        name: "FK_Vouchers_Appointments_AppointmentID",
                        column: x => x.AppointmentID,
                        principalTable: "Appointments",
                        principalColumn: "AppID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vouchers_ChartAccounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "ChartAccounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vouchers_Invoices_InvoiceID",
                        column: x => x.InvoiceID,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vouchers_Treasuries_ToTreasuryID",
                        column: x => x.ToTreasuryID,
                        principalTable: "Treasuries",
                        principalColumn: "TreasuryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vouchers_Treasuries_TreasuryID",
                        column: x => x.TreasuryID,
                        principalTable: "Treasuries",
                        principalColumn: "TreasuryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vouchers_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vouchers_Users_PatientUserID",
                        column: x => x.PatientUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vouchers_Users_PostedByUserID",
                        column: x => x.PostedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ChartAccounts",
                columns: new[] { "AccountID", "AccountCode", "AccountName", "AccountNameAr", "AccountType", "CreatedAt", "IsActive", "OpeningBalance", "ParentAccountID" },
                values: new object[,]
                {
                    { 1, "1000", "Assets", "الأصول", "Asset", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3725), true, 0.00m, null },
                    { 6, "2000", "Liabilities", "الخصوم", "Liability", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3741), true, 0.00m, null },
                    { 10, "3000", "Equity", "حقوق الملكية", "Equity", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3748), true, 0.00m, null },
                    { 13, "4000", "Revenues", "الإيرادات", "Revenue", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3760), true, 0.00m, null },
                    { 19, "5000", "Expenses", "المصروفات", "Expense", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3776), true, 0.00m, null }
                });

            migrationBuilder.InsertData(
                table: "CustomAssessmentTemplates",
                columns: new[] { "TemplateID", "CreatedAt", "Description", "DoctorID", "IsActive", "IsStandard", "MaxScore", "SchemaJson", "TemplateType", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "استبيان عالمي معياري لقياس شدة أعراض الاكتئاب خلال آخر أسبوعين. يتألف من 9 أسئلة ويستغرق 3 دقائق.", null, true, true, 27, "{\n  \"questions\": [\n    { \"id\": 1, \"text\": \"نشاط أو اهتمام أقل بالأشياء عادةً ما تستمتع بها\", \"type\": \"scoring\", \"options\": [\"لا إطلاقاً = 0\", \"عدة أيام = 1\", \"أكثر من نصف الأيام = 2\", \"تقريباً كل يوم = 3\"], \"weights\": [0, 1, 2, 3] },\n    { \"id\": 2, \"text\": \"شعور بالاكتئاب أو اليأس أو قلة الأمل\", \"type\": \"scoring\", \"options\": [\"لا إطلاقاً = 0\", \"عدة أيام = 1\", \"أكثر من نصف الأيام = 2\", \"تقريباً كل يوم = 3\"], \"weights\": [0, 1, 2, 3] },\n    { \"id\": 3, \"text\": \"صعوبة في النوم أو البقاء نائماً أو النوم المفرط\", \"type\": \"scoring\", \"options\": [\"لا إطلاقاً = 0\", \"عدة أيام = 1\", \"أكثر من نصف الأيام = 2\", \"تقريباً كل يوم = 3\"], \"weights\": [0, 1, 2, 3] },\n    { \"id\": 4, \"text\": \"الشعور بالإرهايد أو ضعف الطاقة\", \"type\": \"scoring\", \"options\": [\"لا إطلاقاً = 0\", \"عدة أيام = 1\", \"أكثر من نصف الأيام = 2\", \"تقريباً كل يوم = 3\"], \"weights\": [0, 1, 2, 3] },\n    { \"id\": 5, \"text\": \"قلة الشهية أو الإفراط في الأكل\", \"type\": \"scoring\", \"options\": [\"لا إطلاقاً = 0\", \"عدة أيام = 1\", \"أكثر من نصف الأيام = 2\", \"تقريباً كل يوم = 3\"], \"weights\": [0, 1, 2, 3] },\n    { \"id\": 6, \"text\": \"تقدير سلبى لذاتك (أشعر أنني فاشل أو لقد خيّبت ظروف عائلتي)\", \"type\": \"scoring\", \"options\": [\"لا إطلاقاً = 0\", \"عدة أيام = 1\", \"أكثر من نصف الأيام = 2\", \"تقريباً كل يوم = 3\"], \"weights\": [0, 1, 2, 3] },\n    { \"id\": 7, \"text\": \"صعوبة في التركيز على الأنشطة مثل القراءة أو التلفاز\", \"type\": \"scoring\", \"options\": [\"لا إطلاقاً = 0\", \"عدة أيام = 1\", \"أكثر من نصف الأيام = 2\", \"تقريباً كل يوم = 3\"], \"weights\": [0, 1, 2, 3] },\n    { \"id\": 8, \"text\": \"تتحرك أو تتحدث ببطء لدرجة ملاحظة الآخرين، أو العكس، تتحرك بضجر أكثر من المعتاد\", \"type\": \"scoring\", \"options\": [\"لا إطلاقاً = 0\", \"عدة أيام = 1\", \"أكثر من نصف الأيام = 2\", \"تقريباً كل يوم = 3\"], \"weights\": [0, 1, 2, 3] },\n    { \"id\": 9, \"text\": \"أفكار بأنك قد تتأذى أو أنك قد تؤذى نفسك بطريقة ما\", \"type\": \"scoring\", \"options\": [\"لا إطلاقاً = 0\", \"عدة أيام = 1\", \"أكثر من نصف الأيام = 2\", \"تقريباً كل يوم = 3\"], \"weights\": [0, 1, 2, 3] }\n  ],\n  \"scoring\": {\n    \"min\": 0,\n    \"max\": 27,\n    \"ranges\": [\n      { \"min\": 0,  \"max\": 4,  \"label\": \"الحد الأدنى من أعراض الاكتئاب\",        \"color\": \"#2DC653\", \"recommendation\": \"لا يتطلب تدخلاً علاجياً، مراقبة دورية.\" },\n      { \"min\": 5,  \"max\": 9,  \"label\": \"أعراض اكتئاب خفيفة\",                 \"color\": \"#FF9F1C\", \"recommendation\": \"يُوصى بالمتابعة مع طبيب مختص للدعم النفسي.\" },\n      { \"min\": 10, \"max\": 14, \"label\": \"أعراض اكتئاب متوسطة\",                \"color\": \"#FF6B35\", \"recommendation\": \"توصية بتقييم سريري وعلاج دوائي محتمل.\" },\n      { \"min\": 15, \"max\": 19, \"label\": \"أعراض اكتئاب متوسطة الشدة\",          \"color\": \"#E63946\", \"recommendation\": \"توصية بعلاج دوائي فوري + علاج سلوكي معرفي.\" },\n      { \"min\": 20, \"max\": 27, \"label\": \"أعراض اكتئاب شديدة\",                 \"color\": \"#9B2D30\", \"recommendation\": \"توصية عاجلة بتدخل طبي نفسي مكثف وتقييم خطر السلوك الانتحاري.\" }\n    ]\n  }\n}", "PHQ9", "مقياس الصحة العامة للاكتئاب (PHQ-9)" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "استبيان عالمي معياري لقياس شدة القلق والتوتر خلال آخر أسبوعين. يتألف من 7 أسئلة ويستغرق دقيقتين.", null, true, true, 21, "{\n  \"questions\": [\n    { \"id\": 1, \"text\": \"الشعور بالتوتر أو القلق أو العصبية\", \"type\": \"scoring\", \"options\": [\"لا إطلاقاً = 0\", \"عدة أيام = 1\", \"أكثر من نصف الأيام = 2\", \"تقريباً كل يوم = 3\"], \"weights\": [0, 1, 2, 3] },\n    { \"id\": 2, \"text\": \"عدم القدرة على إيقاف القلق أو التحكم به\", \"type\": \"scoring\", \"options\": [\"لا إطلاقاً = 0\", \"عدة أيام = 1\", \"أكثر من نصف الأيام = 2\", \"تقريباً كل يوم = 3\"], \"weights\": [0, 1, 2, 3] },\n    { \"id\": 3, \"text\": \"القلق المفرط على أشياء مختلفة\", \"type\": \"scoring\", \"options\": [\"لا إطلاقاً = 0\", \"عدة أيام = 1\", \"أكثر من نصف الأيام = 2\", \"تقريباً كل يوم = 3\"], \"weights\": [0, 1, 2, 3] },\n    { \"id\": 4, \"text\": \"صعوبة في الاسترخاء\", \"type\": \"scoring\", \"options\": [\"لا إطلاقاً = 0\", \"عدة أيام = 1\", \"أكثر من نصف الأيام = 2\", \"تقريباً كل يوم = 3\"], \"weights\": [0, 1, 2, 3] },\n    { \"id\": 5, \"text\": \"الشعور بالضجر لدرجة يصعب الجلوس في مكان\", \"type\": \"scoring\", \"options\": [\"لا إطلاقاً = 0\", \"عدة أيام = 1\", \"أكثر من نصف الأيام = 2\", \"تقريباً كل يوم = 3\"], \"weights\": [0, 1, 2, 3] },\n    { \"id\": 6, \"text\": \"الشعور بالانزعاج أو توقع حدوث شيء سيء بسهولة\", \"type\": \"scoring\", \"options\": [\"لا إطلاقاً = 0\", \"عدة أيام = 1\", \"أكثر من نصف الأيام = 2\", \"تقريباً كل يوم = 3\"], \"weights\": [0, 1, 2, 3] },\n    { \"id\": 7, \"text\": \"الشعور بالخوف أو الرعب بدون سبب واضح\", \"type\": \"scoring\", \"options\": [\"لا إطلاقاً = 0\", \"عدة أيام = 1\", \"أكثر من نصف الأيام = 2\", \"تقريباً كل يوم = 3\"], \"weights\": [0, 1, 2, 3] }\n  ],\n  \"scoring\": {\n    \"min\": 0,\n    \"max\": 21,\n    \"ranges\": [\n      { \"min\": 0,  \"max\": 4,  \"label\": \"الحد الأدنى من أعراض القلق\",        \"color\": \"#2DC653\", \"recommendation\": \"لا يتطلب تدخلاً علاجياً، مراقبة دورية.\" },\n      { \"min\": 5,  \"max\": 9,  \"label\": \"أعراض قلق خفيفة\",                 \"color\": \"#FF9F1C\", \"recommendation\": \"يُوصى بالمتابعة مع طبيب مختص للدعم النفسي.\" },\n      { \"min\": 10, \"max\": 14, \"label\": \"أعراض قلق متوسطة\",                \"color\": \"#FF6B35\", \"recommendation\": \"توصية بتقييم سريري وعلاج دوائي محتمل.\" },\n      { \"min\": 15, \"max\": 21, \"label\": \"أعراض قلق شديدة\",                 \"color\": \"#E63946\", \"recommendation\": \"توصية عاجلة بتدخل طبي نفسي مكثف وتقييم خطر الحالة.\" }\n    ]\n  }\n}", "GAD7", "مقياس القلق المعمم (GAD-7)" }
                });

            migrationBuilder.InsertData(
                table: "Priorities",
                columns: new[] { "PriorityID", "ColorCode", "Icon", "LevelName", "LevelNameAr", "Weight" },
                values: new object[,]
                {
                    { 1, "#2DC653", "fa-check-circle", "Normal", "عادي", 1 },
                    { 2, "#FF9F1C", "fa-exclamation-triangle", "Urgent", "عاجل", 2 },
                    { 3, "#E63946", "fa-ambulance", "Emergency", "طوارئ", 3 }
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "SettingKey", "SettingValue", "UpdatedAt" },
                values: new object[,]
                {
                    { "CancelWindowHours", "6", new DateTime(2026, 8, 28, 16, 45, 56, 953, DateTimeKind.Local).AddTicks(7699) },
                    { "DefaultCommissionRatio", "50", new DateTime(2026, 8, 28, 16, 45, 56, 953, DateTimeKind.Local).AddTicks(7697) },
                    { "EnableMobilePWA", "true", new DateTime(2026, 8, 28, 16, 45, 56, 953, DateTimeKind.Local).AddTicks(7676) },
                    { "MaxBookingDaysAhead", "30", new DateTime(2026, 8, 28, 16, 45, 56, 953, DateTimeKind.Local).AddTicks(7702) },
                    { "MaxFutureAppointmentsPerPatient", "5", new DateTime(2026, 8, 28, 16, 45, 56, 953, DateTimeKind.Local).AddTicks(7701) },
                    { "SlotBufferMinutes", "5", new DateTime(2026, 8, 28, 16, 45, 56, 953, DateTimeKind.Local).AddTicks(7704) }
                });

            migrationBuilder.InsertData(
                table: "TriageQuestions",
                columns: new[] { "QuestionID", "Category", "IsActive", "QuestionText", "QuestionTextAr", "SortOrder", "Weight" },
                values: new object[,]
                {
                    { 1, "Cardiac", true, "Do you have chest pain?", "هل تعاني من ألم في الصدر؟", 1, 25 },
                    { 2, "Respiratory", true, "Do you have difficulty breathing?", "هل تعاني من صعوبة في التنفس؟", 2, 25 },
                    { 3, "General", true, "Do you have severe bleeding?", "هل تعاني من نزيف حاد؟", 3, 20 },
                    { 4, "General", true, "Do you have a high fever (above 39°C)?", "هل لديك حرارة مرتفعة (فوق 39 درجة)؟", 4, 15 },
                    { 5, "Neurological", true, "Do you feel dizziness or loss of consciousness?", "هل تشعر بدوخة أو فقدان للوعي؟", 5, 20 },
                    { 6, "General", true, "Do you have severe abdominal pain?", "هل تعاني من ألم شديد في البطن؟", 6, 15 },
                    { 7, "Neurological", true, "Do you have a persistent headache?", "هل تعاني من صداع مستمر؟", 7, 10 },
                    { 8, "General", true, "Have you had a recent injury or accident?", "هل تعرضت لإصابة أو حادث مؤخراً؟", 8, 15 },
                    { 9, "General", true, "Do you have nausea or vomiting?", "هل تعاني من غثيان أو قيء؟", 9, 8 },
                    { 10, "General", true, "Do you have any chronic diseases?", "هل لديك أمراض مزمنة؟", 10, 5 },
                    { 11, "Psychiatric", true, "Do you feel depressed or hopeless?", "هل تشعر باكتئاب أو يأس أو فقدان أمل؟", 11, 20 },
                    { 12, "Psychiatric", true, "Do you feel anxious or nervous most of the time?", "هل تشعر بقلق أو توتر معظم الوقت؟", 12, 15 },
                    { 13, "Psychiatric", true, "Do you have thoughts of harming yourself or others?", "هل لديك أفكار بإيذاء نفسك أو الآخرين؟", 13, 30 },
                    { 14, "Psychiatric", true, "Do you see or hear things that others do not?", "هل ترى أو تسمع أشياء لا يراها أو يسمعها الآخرون؟", 14, 25 },
                    { 15, "Psychiatric", true, "Do you have trouble sleeping or changes in appetite?", "هل تعاني من اضطرابات في النوم أو الشهية؟", 15, 10 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "AssignedTreasuryID", "CreatedAt", "Email", "FullName", "IsActive", "Password", "Phone", "Role" },
                values: new object[] { 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@medical.com", "مدير النظام", true, "$2a$11$HL3jH5enP.qhQRongvmAbO3shF9L2Hh25aK4U17IXSD/T9h3OpHMO", "0500000000", "Admin" });

            migrationBuilder.InsertData(
                table: "Wards",
                columns: new[] { "WardID", "FloorNumber", "GenderType", "IsActive", "WardName", "WardNameAr" },
                values: new object[,]
                {
                    { 1, 2, "Mixed", true, "Surgical Ward", "جناح الجراحة العامة" },
                    { 2, 2, "Mixed", true, "Internal Medicine Ward", "جناح الباطنية والمرضى الداخليين" },
                    { 3, 3, "Mixed", true, "Intensive Care Unit (ICU)", "قسم العناية المركزة (ICU)" }
                });

            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "WarehouseID", "CreatedAt", "IsActive", "Location", "WarehouseCode", "WarehouseName", "WarehouseNameAr" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "الطابق الأرضي", "WARE-01", "Main Warehouse", "المخزن الرئيسي" });

            migrationBuilder.InsertData(
                table: "ChartAccounts",
                columns: new[] { "AccountID", "AccountCode", "AccountName", "AccountNameAr", "AccountType", "CreatedAt", "IsActive", "OpeningBalance", "ParentAccountID" },
                values: new object[,]
                {
                    { 2, "1010", "Cash on Hand", "الصندوق (النقدية)", "Asset", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3731), true, 0.00m, 1 },
                    { 3, "1020", "Bank Accounts", "البنوك", "Asset", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3733), true, 0.00m, 1 },
                    { 4, "1030", "Accounts Receivable (Patients)", "حسابات قبض (مرضى)", "Asset", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3735), true, 0.00m, 1 },
                    { 5, "1100", "Inventory", "المخزون (أدوية ومواد)", "Asset", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3739), true, 0.00m, 1 },
                    { 7, "2010", "Accounts Payable (Suppliers)", "حسابات دائنة (موردون)", "Liability", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3742), true, 0.00m, 6 },
                    { 8, "2020", "Accrued Salaries", "رواتب ومستحقات مستحقة", "Liability", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3744), true, 0.00m, 6 },
                    { 9, "2030", "Accrued Doctor Commissions", "عمولات أطباء مستحقة", "Liability", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3746), true, 0.00m, 6 },
                    { 11, "3010", "Owner's Capital", "رأس المال", "Equity", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3750), true, 0.00m, 10 },
                    { 12, "3020", "Retained Earnings", "أرباح أو خسائر مرحّلة", "Equity", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3759), true, 0.00m, 10 },
                    { 14, "4010", "Consultation Revenue", "إيرادات الكشوفات والعيادة", "Revenue", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3762), true, 0.00m, 13 },
                    { 15, "4020", "Pharmacy Revenue", "إيرادات الصيدلية", "Revenue", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3764), true, 0.00m, 13 },
                    { 16, "4030", "Laboratory Revenue", "إيرادات المختبر", "Revenue", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3765), true, 0.00m, 13 },
                    { 17, "4040", "Radiology Revenue", "إيرادات الأشعة", "Revenue", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3767), true, 0.00m, 13 },
                    { 18, "4050", "Inpatient Revenue", "إيرادات الإيواء والتنويم", "Revenue", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3768), true, 0.00m, 13 },
                    { 20, "5010", "Salaries Expense", "مصروف رواتب الموظفين", "Expense", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3778), true, 0.00m, 19 },
                    { 21, "5020", "Doctor Commissions Expense", "مصروف عمولات الأطباء", "Expense", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3779), true, 0.00m, 19 },
                    { 22, "5030", "Rent Expense", "مصروف الإيجار", "Expense", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3781), true, 0.00m, 19 },
                    { 23, "5040", "Utilities Expense", "مصروف الكهرباء والماء", "Expense", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3783), true, 0.00m, 19 },
                    { 24, "5050", "Maintenance Expense", "مصروف الصيانة والتجهيزات", "Expense", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3825), true, 0.00m, 19 },
                    { 25, "5060", "General Expense", "مصروفات عامة ومتنوعة", "Expense", new DateTime(2026, 8, 28, 16, 45, 56, 964, DateTimeKind.Local).AddTicks(3834), true, 0.00m, 19 }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "RoomID", "DailyRate", "IsActive", "MaxBeds", "RoomNumber", "RoomType", "WardID" },
                values: new object[,]
                {
                    { 1, 500m, true, 1, "101-VIP", "VIP", 1 },
                    { 2, 200m, true, 2, "102-A", "General", 1 },
                    { 3, 350m, true, 1, "201-A", "Private", 2 },
                    { 4, 1000m, true, 1, "ICU-01", "ICU", 3 }
                });

            migrationBuilder.InsertData(
                table: "Beds",
                columns: new[] { "BedID", "BedNumber", "Notes", "RoomID", "Status" },
                values: new object[,]
                {
                    { 1, "B101-1", "سرير عناية فاخر", 1, "Vacant" },
                    { 2, "B102-1", "سرير عادي جانبي", 2, "Vacant" },
                    { 3, "B102-2", "سرير عادي نافذة", 2, "Vacant" },
                    { 4, "B201-1", "سرير خاص مفرد", 3, "Vacant" },
                    { 5, "BICU-1", "سرير عناية مركزة مجهز بمراقبة حيوية", 4, "Vacant" }
                });

            migrationBuilder.InsertData(
                table: "Treasuries",
                columns: new[] { "TreasuryID", "AccountID", "CreatedAt", "IsActive", "TreasuryCode", "TreasuryName", "TreasuryNameAr" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "CASH-01", "Main Cash", "الصندوق الرئيسي" },
                    { 2, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "BANK-01", "Main Bank", "الحساب البنكي الرئيسي" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_BedID",
                table: "Admissions",
                column: "BedID");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_DoctorID",
                table: "Admissions",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_PatientID",
                table: "Admissions",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorID",
                table: "Appointments",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientID",
                table: "Appointments",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PriorityID",
                table: "Appointments",
                column: "PriorityID");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_PatientID",
                table: "Attachments",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_RecordID",
                table: "Attachments",
                column: "RecordID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserID",
                table: "AuditLogs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Beds_RoomID",
                table: "Beds",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_ChartAccounts_AccountCode",
                table: "ChartAccounts",
                column: "AccountCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChartAccounts_ParentAccountID",
                table: "ChartAccounts",
                column: "ParentAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_CultureSensitivities_LabOrderItemID",
                table: "CultureSensitivities",
                column: "LabOrderItemID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomAssessmentTemplates_DoctorID",
                table: "CustomAssessmentTemplates",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_DispenseRecords_DispensedByUserID",
                table: "DispenseRecords",
                column: "DispensedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_DispenseRecords_MedicationID",
                table: "DispenseRecords",
                column: "MedicationID");

            migrationBuilder.CreateIndex(
                name: "IX_DispenseRecords_PrescriptionID",
                table: "DispenseRecords",
                column: "PrescriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorCommissions_DoctorID",
                table: "DoctorCommissions",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorProfiles_UserID",
                table: "DoctorProfiles",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCourses_EmployeeID",
                table: "EmployeeCourses",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaves_ApprovedByUserID",
                table: "EmployeeLeaves",
                column: "ApprovedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaves_EmployeeID",
                table: "EmployeeLeaves",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfiles_EmployeeNumber",
                table: "EmployeeProfiles",
                column: "EmployeeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfiles_UserID",
                table: "EmployeeProfiles",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InpatientCareExecutions_ExecutedByUserID",
                table: "InpatientCareExecutions",
                column: "ExecutedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_InpatientCareExecutions_OrderID",
                table: "InpatientCareExecutions",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_InpatientCareOrders_AdmissionID",
                table: "InpatientCareOrders",
                column: "AdmissionID");

            migrationBuilder.CreateIndex(
                name: "IX_InpatientCareOrders_CreatedByUserID",
                table: "InpatientCareOrders",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_InpatientCareOrders_HealthServiceID",
                table: "InpatientCareOrders",
                column: "HealthServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_InpatientDailyLogs_AdmissionID",
                table: "InpatientDailyLogs",
                column: "AdmissionID");

            migrationBuilder.CreateIndex(
                name: "IX_InpatientDailyLogs_LoggedByUserID",
                table: "InpatientDailyLogs",
                column: "LoggedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCategories_ParentCategoryID",
                table: "InventoryCategories",
                column: "ParentCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_CategoryID",
                table: "InventoryItems",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_ItemCode",
                table: "InventoryItems",
                column: "ItemCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_MedicationID",
                table: "InventoryItems",
                column: "MedicationID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_AppointmentID",
                table: "Invoices",
                column: "AppointmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_DispenseRecordID",
                table: "Invoices",
                column: "DispenseRecordID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_DoctorCommissionID",
                table: "Invoices",
                column: "DoctorCommissionID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_DoctorID",
                table: "Invoices",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_LabOrderID",
                table: "Invoices",
                column: "LabOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PatientUserID",
                table: "Invoices",
                column: "PatientUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_RadiologyOrderID",
                table: "Invoices",
                column: "RadiologyOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_CreatedByUserID",
                table: "JournalEntries",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_EntryNumber",
                table: "JournalEntries",
                column: "EntryNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_PostedByUserID",
                table: "JournalEntries",
                column: "PostedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_AccountID",
                table: "JournalEntryLines",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_JournalEntryID",
                table: "JournalEntryLines",
                column: "JournalEntryID");

            migrationBuilder.CreateIndex(
                name: "IX_LabDevices_DeviceCode",
                table: "LabDevices",
                column: "DeviceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabOrderItems_LabOrderID_LabTestID",
                table: "LabOrderItems",
                columns: new[] { "LabOrderID", "LabTestID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabOrderItems_LabTestID",
                table: "LabOrderItems",
                column: "LabTestID");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrders_DoctorID",
                table: "LabOrders",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrders_LabTestID",
                table: "LabOrders",
                column: "LabTestID");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrders_PatientUserID",
                table: "LabOrders",
                column: "PatientUserID");

            migrationBuilder.CreateIndex(
                name: "IX_LabReferenceRanges_LabTestID",
                table: "LabReferenceRanges",
                column: "LabTestID");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_DeviceID",
                table: "LabTests",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_PanelID",
                table: "LabTests",
                column: "PanelID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_AppID",
                table: "MedicalRecords",
                column: "AppID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicationRequests_DoctorUserID",
                table: "MedicationRequests",
                column: "DoctorUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAssessments_PatientUserID",
                table: "PatientAssessments",
                column: "PatientUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAssessments_TemplateID",
                table: "PatientAssessments",
                column: "TemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_PatientProfiles_FileNumber",
                table: "PatientProfiles",
                column: "FileNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientProfiles_UserID",
                table: "PatientProfiles",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_MedicationID",
                table: "Prescriptions",
                column: "MedicationID");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_RecordID",
                table: "Prescriptions",
                column: "RecordID");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyOrders_DoctorID",
                table: "RadiologyOrders",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyOrders_PatientUserID",
                table: "RadiologyOrders",
                column: "PatientUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyOrders_RadiologistID",
                table: "RadiologyOrders",
                column: "RadiologistID");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_WardID",
                table: "Rooms",
                column: "WardID");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryRecords_CreatedByUserID",
                table: "SalaryRecords",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryRecords_EmployeeID_PeriodYear_PeriodMonth",
                table: "SalaryRecords",
                columns: new[] { "EmployeeID", "PeriodYear", "PeriodMonth" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalaryRecords_JournalEntryID",
                table: "SalaryRecords",
                column: "JournalEntryID");

            migrationBuilder.CreateIndex(
                name: "IX_SensitivityResults_CultureSensitivityID",
                table: "SensitivityResults",
                column: "CultureSensitivityID");

            migrationBuilder.CreateIndex(
                name: "IX_SoapNotes_RecordID",
                table: "SoapNotes",
                column: "RecordID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockCountItems_ItemID",
                table: "StockCountItems",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_StockCountItems_StockCountID_ItemID",
                table: "StockCountItems",
                columns: new[] { "StockCountID", "ItemID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockCounts_CreatedByUserID",
                table: "StockCounts",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_StockCounts_PostedByUserID",
                table: "StockCounts",
                column: "PostedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_StockCounts_ReversedByUserID",
                table: "StockCounts",
                column: "ReversedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_StockCounts_StockCountNumber",
                table: "StockCounts",
                column: "StockCountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockCounts_WarehouseID",
                table: "StockCounts",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovementItems_ItemID",
                table: "StockMovementItems",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovementItems_MovementID",
                table: "StockMovementItems",
                column: "MovementID");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_CreatedByUserID",
                table: "StockMovements",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_MovementNumber",
                table: "StockMovements",
                column: "MovementNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_PostedByUserID",
                table: "StockMovements",
                column: "PostedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ToWarehouseID",
                table: "StockMovements",
                column: "ToWarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_WarehouseID",
                table: "StockMovements",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_TelemedicineSessions_AppointmentID",
                table: "TelemedicineSessions",
                column: "AppointmentID");

            migrationBuilder.CreateIndex(
                name: "IX_TelemedicineSessions_RoomCode",
                table: "TelemedicineSessions",
                column: "RoomCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Treasuries_AccountID",
                table: "Treasuries",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Treasuries_TreasuryCode",
                table: "Treasuries",
                column: "TreasuryCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_UserID_IsRead",
                table: "UserNotifications",
                columns: new[] { "UserID", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_AssignedTreasuryID",
                table: "Users",
                column: "AssignedTreasuryID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_AccountID",
                table: "Vouchers",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_AppointmentID",
                table: "Vouchers",
                column: "AppointmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_CreatedByUserID",
                table: "Vouchers",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_InvoiceID",
                table: "Vouchers",
                column: "InvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_PatientUserID",
                table: "Vouchers",
                column: "PatientUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_PostedByUserID",
                table: "Vouchers",
                column: "PostedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_ToTreasuryID",
                table: "Vouchers",
                column: "ToTreasuryID");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_TreasuryID",
                table: "Vouchers",
                column: "TreasuryID");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_VoucherNumber",
                table: "Vouchers",
                column: "VoucherNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_WarehouseCode",
                table: "Warehouses",
                column: "WarehouseCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebPushSubscriptions_Endpoint",
                table: "WebPushSubscriptions",
                column: "Endpoint",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebPushSubscriptions_UserID",
                table: "WebPushSubscriptions",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "EmployeeCourses");

            migrationBuilder.DropTable(
                name: "EmployeeLeaves");

            migrationBuilder.DropTable(
                name: "InpatientCareExecutions");

            migrationBuilder.DropTable(
                name: "InpatientDailyLogs");

            migrationBuilder.DropTable(
                name: "JournalEntryLines");

            migrationBuilder.DropTable(
                name: "LabReferenceRanges");

            migrationBuilder.DropTable(
                name: "MedicationRequests");

            migrationBuilder.DropTable(
                name: "PatientAssessments");

            migrationBuilder.DropTable(
                name: "PsychiatricRecords");

            migrationBuilder.DropTable(
                name: "RadiologyTemplates");

            migrationBuilder.DropTable(
                name: "SalaryRecords");

            migrationBuilder.DropTable(
                name: "SensitivityResults");

            migrationBuilder.DropTable(
                name: "SoapNotes");

            migrationBuilder.DropTable(
                name: "StockCountItems");

            migrationBuilder.DropTable(
                name: "StockMovementItems");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "TelemedicineSessions");

            migrationBuilder.DropTable(
                name: "TriageQuestions");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropTable(
                name: "Vouchers");

            migrationBuilder.DropTable(
                name: "WebPushSubscriptions");

            migrationBuilder.DropTable(
                name: "InpatientCareOrders");

            migrationBuilder.DropTable(
                name: "CustomAssessmentTemplates");

            migrationBuilder.DropTable(
                name: "EmployeeProfiles");

            migrationBuilder.DropTable(
                name: "JournalEntries");

            migrationBuilder.DropTable(
                name: "CultureSensitivities");

            migrationBuilder.DropTable(
                name: "StockCounts");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "StockMovements");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Admissions");

            migrationBuilder.DropTable(
                name: "HealthServices");

            migrationBuilder.DropTable(
                name: "LabOrderItems");

            migrationBuilder.DropTable(
                name: "InventoryCategories");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "DispenseRecords");

            migrationBuilder.DropTable(
                name: "DoctorCommissions");

            migrationBuilder.DropTable(
                name: "RadiologyOrders");

            migrationBuilder.DropTable(
                name: "Beds");

            migrationBuilder.DropTable(
                name: "LabOrders");

            migrationBuilder.DropTable(
                name: "Prescriptions");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "LabTests");

            migrationBuilder.DropTable(
                name: "MedicalRecords");

            migrationBuilder.DropTable(
                name: "Medications");

            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropTable(
                name: "LabDevices");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "DoctorProfiles");

            migrationBuilder.DropTable(
                name: "PatientProfiles");

            migrationBuilder.DropTable(
                name: "Priorities");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Treasuries");

            migrationBuilder.DropTable(
                name: "ChartAccounts");
        }
    }
}
