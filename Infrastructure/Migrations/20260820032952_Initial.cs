using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Street = table.Column<string>(type: "text", nullable: true),
                    Number = table.Column<string>(type: "text", nullable: true),
                    PostalCode = table.Column<string>(type: "text", nullable: true),
                    Latitud = table.Column<string>(type: "text", nullable: true),
                    Longitud = table.Column<string>(type: "text", nullable: true),
                    IdCity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    SecondName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    IdentityNumber = table.Column<string>(type: "text", nullable: true),
                    CodeArea = table.Column<string>(type: "text", nullable: true),
                    AccountActivate = table.Column<bool>(type: "boolean", nullable: false),
                    UserGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    IsOwnAccount = table.Column<bool>(type: "boolean", nullable: false),
                    ProviderAccount = table.Column<bool>(type: "boolean", nullable: false),
                    ManagedHotelId = table.Column<int>(type: "integer", nullable: true),
                    OwnedHotelIds = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Costs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CostPerTime = table.Column<decimal>(type: "numeric", nullable: false),
                    Hour = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Costs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimesAvialable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Time = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimesAvialable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodeArea = table.Column<int>(type: "integer", nullable: false),
                    PhoneNumber = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    MetaDescription = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    AddressHotelId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hotels_Address_AddressHotelId",
                        column: x => x.AddressHotelId,
                        principalTable: "Address",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    SecondName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    IdAddress = table.Column<string>(type: "text", nullable: true),
                    CodeArea = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    IdentityNumber = table.Column<string>(type: "text", nullable: false),
                    AccountActivate = table.Column<bool>(type: "boolean", nullable: false),
                    UserGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    IsOwnAccount = table.Column<bool>(type: "boolean", nullable: false),
                    ProviderAccount = table.Column<bool>(type: "boolean", nullable: false),
                    AddressId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Provincies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Provincies_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HotelPicture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Path = table.Column<string>(type: "text", nullable: true),
                    HotelId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelPicture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelPicture_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BedNumbers = table.Column<int>(type: "integer", nullable: false),
                    AvialableNow = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    HotelsId = table.Column<int>(type: "integer", nullable: true),
                    CostId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Costs_CostId",
                        column: x => x.CostId,
                        principalTable: "Costs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rooms_Hotels_HotelsId",
                        column: x => x.HotelsId,
                        principalTable: "Hotels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CheckInTimeId = table.Column<int>(type: "integer", nullable: false),
                    IdRoom = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    UserGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    DateReserved = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_TimesAvialable_CheckInTimeId",
                        column: x => x.CheckInTimeId,
                        principalTable: "TimesAvialable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PreBooking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdRoom = table.Column<int>(type: "integer", nullable: false),
                    TimeToExpire = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    UserId1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreBooking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreBooking_User_UserId1",
                        column: x => x.UserId1,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ProvinceId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Provincies_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provincies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoomPictures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    RoomId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomPictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomPictures_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "IdCity", "Latitud", "Longitud", "Number", "PostalCode", "Street" },
                values: new object[,]
                {
                    { 1, 1, "-34.6037", "-58.3816", "1234", "C1043", "Av. Corrientes" },
                    { 2, 23, "-32.9442", "-60.6505", "890", "S2000", "Bv. Oroño" },
                    { 3, 15, "-32.8895", "-68.8458", "456", "M5500", "Av. San Martín" },
                    { 4, 18, "-41.1335", "-71.3103", "11500", "R8400", "Av. Bustillo" },
                    { 5, 19, "-24.7829", "-65.4232", "786", "A4400", "Caseros" }
                });

            migrationBuilder.InsertData(
                table: "Costs",
                columns: new[] { "Id", "CostPerTime", "Hour" },
                values: new object[,]
                {
                    { 1, 45000m, 2 },
                    { 2, 78000m, 4 },
                    { 3, 120000m, 8 },
                    { 4, 165000m, 12 },
                    { 5, 52000m, 2 },
                    { 6, 86000m, 4 },
                    { 7, 134000m, 8 },
                    { 8, 182000m, 12 }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Argentina" },
                    { 2, "Chile" },
                    { 3, "Bolivia" },
                    { 4, "Brasil" },
                    { 5, "Uruguay" }
                });

            migrationBuilder.InsertData(
                table: "TimesAvialable",
                columns: new[] { "Id", "Time" },
                values: new object[,]
                {
                    { 1, "08:00" },
                    { 2, "10:00" },
                    { 3, "12:00" },
                    { 4, "14:00" },
                    { 5, "16:00" },
                    { 6, "18:00" },
                    { 7, "20:00" },
                    { 8, "22:00" }
                });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "AddressHotelId", "CodeArea", "Description", "Email", "MetaDescription", "Name", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, 1, 11, "Hotel urbano en el centro porteño.", "reservas@hotelobelisco.com", "Ideal para negocios y escapadas en Buenos Aires.", "Hotel Obelisco", 43219876 },
                    { 2, 2, 341, "Hotel moderno con vista al río Paraná.", "hola@rosarioriverside.com", "Alojamiento premium en Rosario.", "Rosario Riverside", 5588776 },
                    { 3, 3, 261, "Suites boutique cerca de bodegas y montaña.", "info@andessuites.com", "Descanso y vino en Mendoza.", "Andes Suites Mendoza", 4477551 },
                    { 4, 4, 294, "Hotel de montaña con vista al lago.", "contacto@patagoniaview.com", "Experiencia patagónica en Bariloche.", "Patagonia View Bariloche", 4522334 },
                    { 5, 5, 387, "Hotel cálido en el casco histórico salteño.", "reservas@saltacolonial.com", "Tradición y confort en Salta.", "Salta Colonial", 4123456 }
                });

            migrationBuilder.InsertData(
                table: "Provincies",
                columns: new[] { "Id", "CountryId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Buenos Aires" },
                    { 2, 1, "Catamarca" },
                    { 3, 1, "Chaco" },
                    { 4, 1, "Chubut" },
                    { 5, 1, "Córdoba" },
                    { 6, 1, "Corrientes" },
                    { 7, 1, "Entre Ríos" },
                    { 8, 1, "Formosa" },
                    { 9, 1, "Jujuy" },
                    { 10, 1, "La Pampa" },
                    { 11, 1, "La Rioja" },
                    { 12, 1, "Mendoza" },
                    { 13, 1, "Misiones" },
                    { 14, 1, "Neuquén" },
                    { 15, 1, "Río Negro" },
                    { 16, 1, "Salta" },
                    { 17, 1, "San Juan" },
                    { 18, 1, "San Luis" },
                    { 19, 1, "Santa Cruz" },
                    { 20, 1, "Santa Fe" },
                    { 21, 1, "Santiago del Estero" },
                    { 22, 1, "Tierra del Fuego" },
                    { 23, 1, "Tucumán" },
                    { 24, 1, "Ciudad Autónoma de Buenos Aires" }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Name", "ProvinceId" },
                values: new object[,]
                {
                    { 1, "Buenos Aires", 24 },
                    { 2, "La Plata", 1 },
                    { 3, "Mar del Plata", 1 },
                    { 4, "Bahía Blanca", 1 },
                    { 5, "San Fernando del Valle de Catamarca", 2 },
                    { 6, "Resistencia", 3 },
                    { 7, "Comodoro Rivadavia", 4 },
                    { 8, "Córdoba", 5 },
                    { 9, "Corrientes", 6 },
                    { 10, "Paraná", 7 },
                    { 11, "Formosa", 8 },
                    { 12, "San Salvador de Jujuy", 9 },
                    { 13, "Santa Rosa", 10 },
                    { 14, "La Rioja", 11 },
                    { 15, "Mendoza", 12 },
                    { 16, "Posadas", 13 },
                    { 17, "Neuquén", 14 },
                    { 18, "San Carlos de Bariloche", 15 },
                    { 19, "Salta", 16 },
                    { 20, "San Juan", 17 },
                    { 21, "San Luis", 18 },
                    { 22, "Río Gallegos", 19 },
                    { 23, "Rosario", 20 },
                    { 24, "Santa Fe", 20 },
                    { 25, "Santiago del Estero", 21 },
                    { 26, "Ushuaia", 22 },
                    { 27, "San Miguel de Tucumán", 23 },
                    { 28, "Puerto Iguazú", 13 },
                    { 29, "Villa Carlos Paz", 5 },
                    { 30, "El Calafate", 19 }
                });

            migrationBuilder.InsertData(
                table: "HotelPicture",
                columns: new[] { "Id", "HotelId", "Path" },
                values: new object[,]
                {
                    { 1, 1, "assets/hotels/obelisco.jpg" },
                    { 2, 2, "assets/hotels/rosario-riverside.jpg" },
                    { 3, 3, "assets/hotels/andes-suites.jpg" },
                    { 4, 4, "assets/hotels/patagonia-view.jpg" },
                    { 5, 5, "assets/hotels/salta-colonial.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "AvialableNow", "BedNumbers", "CostId", "Description", "HotelsId", "Name" },
                values: new object[,]
                {
                    { 1, true, 1, 1, "Habitación individual con escritorio.", 1, "Single Business" },
                    { 2, true, 2, 2, "Habitación doble con desayuno incluido.", 1, "Doble Ejecutiva" },
                    { 3, true, 2, 6, "Suite con vista al río.", 2, "Suite Paraná" },
                    { 4, false, 3, 7, "Ideal para familias y estadías cortas.", 2, "Familiar Rosario" },
                    { 5, true, 2, 6, "Habitación premium con ambientación mendocina.", 3, "Suite Malbec" },
                    { 6, true, 4, 8, "Amplia habitación para grupos pequeños.", 3, "Familiar Cordillera" },
                    { 7, true, 2, 7, "Vista al lago y detalles patagónicos.", 4, "Lago Superior" },
                    { 8, true, 3, 8, "Suite con living y balcón.", 4, "Suite Nahuel" },
                    { 9, true, 1, 1, "Opción práctica para viajeros solos.", 5, "Colonial Single" },
                    { 10, true, 2, 5, "Decoración regional y patio interno.", 5, "Tradición Norteña" }
                });

            migrationBuilder.InsertData(
                table: "RoomPictures",
                columns: new[] { "Id", "Name", "RoomId" },
                values: new object[,]
                {
                    { 1, "assets/rooms/single-business.jpg", 1 },
                    { 2, "assets/rooms/doble-ejecutiva.jpg", 2 },
                    { 3, "assets/rooms/suite-parana.jpg", 3 },
                    { 4, "assets/rooms/familiar-rosario.jpg", 4 },
                    { 5, "assets/rooms/suite-malbec.jpg", 5 },
                    { 6, "assets/rooms/familiar-cordillera.jpg", 6 },
                    { 7, "assets/rooms/lago-superior.jpg", 7 },
                    { 8, "assets/rooms/suite-nahuel.jpg", 8 },
                    { 9, "assets/rooms/colonial-single.jpg", 9 },
                    { 10, "assets/rooms/tradicion-nortena.jpg", 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CheckInTimeId",
                table: "Bookings",
                column: "CheckInTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ProvinceId",
                table: "Cities",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelPicture_HotelId",
                table: "HotelPicture",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_AddressHotelId",
                table: "Hotels",
                column: "AddressHotelId");

            migrationBuilder.CreateIndex(
                name: "IX_PreBooking_UserId1",
                table: "PreBooking",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Provincies_CountryId",
                table: "Provincies",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomPictures_RoomId",
                table: "RoomPictures",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CostId",
                table: "Rooms",
                column: "CostId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_HotelsId",
                table: "Rooms",
                column: "HotelsId");

            migrationBuilder.CreateIndex(
                name: "IX_User_AddressId",
                table: "User",
                column: "AddressId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "HotelPicture");

            migrationBuilder.DropTable(
                name: "PreBooking");

            migrationBuilder.DropTable(
                name: "RoomPictures");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "TimesAvialable");

            migrationBuilder.DropTable(
                name: "Provincies");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Costs");

            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropTable(
                name: "Address");
        }
    }
}
