using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDLS.Model.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "DrivingLicense",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("DrivingLicense_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ForumTopic",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ForumTopic_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    image = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Notification_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionCategory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("QuestionCategory_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionTopic",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("QuestionTopic_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ReportCategory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ReportCategory_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Role_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SignCategory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SignCategory_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SimulationCategory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SimulationCategory_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SimulationChapter",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    index = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SimulationChapter_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SimulationDifficultyLevel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SimulationDifficultyLevel_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfig",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SystemConfig_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    colorCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Tag_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionChapter",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    drivingLicenseId = table.Column<Guid>(type: "uuid", nullable: false),
                    index = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("QuestionChapter_pkey", x => x.id);
                    table.ForeignKey(
                        name: "QuestionChapter_drivingLicenseId_fkey",
                        column: x => x.drivingLicenseId,
                        principalTable: "DrivingLicense",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    drivingLicenseId = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Vehicle_pkey", x => x.id);
                    table.ForeignKey(
                        name: "Vehicle_drivingLicenseId_fkey",
                        column: x => x.drivingLicenseId,
                        principalTable: "DrivingLicense",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    roleId = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    avatar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    dateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    licenseType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("User_pkey", x => x.id);
                    table.ForeignKey(
                        name: "User_roleId_fkey",
                        column: x => x.roleId,
                        principalTable: "Role",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "TrafficSign",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    signCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    index = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    code = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    vectorData = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    image = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("TrafficSign_pkey", x => x.id);
                    table.ForeignKey(
                        name: "TrafficSign_signCategoryId_fkey",
                        column: x => x.signCategoryId,
                        principalTable: "SignCategory",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "SimulationScenario",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    simulationChapterId = table.Column<Guid>(type: "uuid", nullable: false),
                    simulationCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    simulationDifficultyLevelId = table.Column<Guid>(type: "uuid", nullable: false),
                    index = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    video = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    totalTime = table.Column<double>(type: "double precision", nullable: false),
                    startPoint = table.Column<double>(type: "double precision", nullable: false),
                    endPoint = table.Column<double>(type: "double precision", nullable: false),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SimulationScenario_pkey", x => x.id);
                    table.ForeignKey(
                        name: "SimulationScenario_simulationCategoryId_fkey",
                        column: x => x.simulationCategoryId,
                        principalTable: "SimulationCategory",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "SimulationScenario_simulationChapterId_fkey",
                        column: x => x.simulationChapterId,
                        principalTable: "SimulationChapter",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "SimulationScenario_simulationDifficultyLevelId_fkey",
                        column: x => x.simulationDifficultyLevelId,
                        principalTable: "SimulationDifficultyLevel",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "QuestionLesson",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    questionChapterId = table.Column<Guid>(type: "uuid", nullable: false),
                    index = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    content = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("QuestionLesson_pkey", x => x.id);
                    table.ForeignKey(
                        name: "QuestionLesson_questionChapterId_fkey",
                        column: x => x.questionChapterId,
                        principalTable: "QuestionChapter",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Exam",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    duration = table.Column<double>(type: "double precision", nullable: true),
                    passScore = table.Column<int>(type: "integer", nullable: true),
                    isRandom = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Exam_pkey", x => x.id);
                    table.ForeignKey(
                        name: "Exam_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ForumPost",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    forumTopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    viewCount = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ForumPost_pkey", x => x.id);
                    table.ForeignKey(
                        name: "ForumPost_forumTopicId_fkey",
                        column: x => x.forumTopicId,
                        principalTable: "ForumTopic",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "ForumPost_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    orderCode = table.Column<long>(type: "bigint", nullable: true),
                    method = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    amount = table.Column<int>(type: "integer", nullable: true),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    response = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Payment_pkey", x => x.id);
                    table.ForeignKey(
                        name: "Payment_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "SituationExam",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    duration = table.Column<double>(type: "double precision", nullable: true),
                    passScore = table.Column<int>(type: "integer", nullable: true),
                    isRandom = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    userId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SituationExam_pkey", x => x.id);
                    table.ForeignKey(
                        name: "Situation_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "UserLicense",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    drivingLicenseId = table.Column<Guid>(type: "uuid", nullable: false),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("UserLicense_pkey", x => x.id);
                    table.ForeignKey(
                        name: "UserLicense_drivingLicenseId_fkey",
                        column: x => x.drivingLicenseId,
                        principalTable: "DrivingLicense",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "UserLicense_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNotification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    notificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("UserNotification_pkey", x => x.id);
                    table.ForeignKey(
                        name: "UserNotification_notificationId_fkey",
                        column: x => x.notificationId,
                        principalTable: "Notification",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "UserNotification_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedTrafficSign",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    trafficSignId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SavedTrafficSign_pkey", x => x.id);
                    table.ForeignKey(
                        name: "SavedTrafficSign_trafficSignId_fkey",
                        column: x => x.trafficSignId,
                        principalTable: "TrafficSign",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "SavedTrafficSign_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonImage",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    questionLessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("LessonImage_pkey", x => x.id);
                    table.ForeignKey(
                        name: "LessonImage_questionLessonId_fkey",
                        column: x => x.questionLessonId,
                        principalTable: "QuestionLesson",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "LessonProgress",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    questionLessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("LessonProgress_pkey", x => x.id);
                    table.ForeignKey(
                        name: "LessonProgress_questionLessonId_fkey",
                        column: x => x.questionLessonId,
                        principalTable: "QuestionLesson",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "LessonProgress_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    questionLessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    questionTopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    questionCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    parentId = table.Column<Guid>(type: "uuid", nullable: true),
                    index = table.Column<int>(type: "integer", nullable: true),
                    content = table.Column<string>(type: "text", nullable: false),
                    image = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    explanation = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Question_pkey", x => x.id);
                    table.ForeignKey(
                        name: "Question_parentId_fkey",
                        column: x => x.parentId,
                        principalTable: "Question",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Question_questionCategoryId_fkey",
                        column: x => x.questionCategoryId,
                        principalTable: "QuestionCategory",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Question_questionLessonId_fkey",
                        column: x => x.questionLessonId,
                        principalTable: "QuestionLesson",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Question_questionTopicId_fkey",
                        column: x => x.questionTopicId,
                        principalTable: "QuestionTopic",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ExamSession",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    examId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: true),
                    totalDuration = table.Column<double>(type: "double precision", nullable: true),
                    isPassed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ExamSession_pkey", x => x.id);
                    table.ForeignKey(
                        name: "ExamSession_examId_fkey",
                        column: x => x.examId,
                        principalTable: "Exam",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "ExamSession_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ForumComment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    replyId = table.Column<Guid>(type: "uuid", nullable: true),
                    forumPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ForumComment_pkey", x => x.id);
                    table.ForeignKey(
                        name: "ForumComment_forumPostId_fkey",
                        column: x => x.forumPostId,
                        principalTable: "ForumPost",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForumComment_replyId_fkey",
                        column: x => x.replyId,
                        principalTable: "ForumComment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForumComment_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "PostImage",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    forumPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PostImage_pkey", x => x.id);
                    table.ForeignKey(
                        name: "PostImage_forumPostId_fkey",
                        column: x => x.forumPostId,
                        principalTable: "ForumPost",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "PostReact",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    forumPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    reactType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PostReact_pkey", x => x.id);
                    table.ForeignKey(
                        name: "PostReact_forumPostId_fkey",
                        column: x => x.forumPostId,
                        principalTable: "ForumPost",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "PostReact_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SimulationExam",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    situationExamId = table.Column<Guid>(type: "uuid", nullable: false),
                    simulationId = table.Column<Guid>(type: "uuid", nullable: false),
                    baseScore = table.Column<int>(type: "integer", nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SimulationExam_pkey", x => x.id);
                    table.ForeignKey(
                        name: "SimulationExam_simulationId_fkey",
                        column: x => x.simulationId,
                        principalTable: "SimulationScenario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "SimulationExam_situationExamId_fkey",
                        column: x => x.situationExamId,
                        principalTable: "SituationExam",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SimulationSession",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    situationExamId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    totalScore = table.Column<int>(type: "integer", nullable: true),
                    totalDuration = table.Column<double>(type: "double precision", nullable: true),
                    isPassed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SimulationSession_pkey", x => x.id);
                    table.ForeignKey(
                        name: "SimulationSession_situationExamId_fkey",
                        column: x => x.situationExamId,
                        principalTable: "SituationExam",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "SimulationSession_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Answer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    questionId = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    isCorrect = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Answer_pkey", x => x.id);
                    table.ForeignKey(
                        name: "Answer_questionId_fkey",
                        column: x => x.questionId,
                        principalTable: "Question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamQuestion",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    questionId = table.Column<Guid>(type: "uuid", nullable: false),
                    examId = table.Column<Guid>(type: "uuid", nullable: false),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ExamQuestion_pkey", x => x.id);
                    table.ForeignKey(
                        name: "ExamQuestion_examId_fkey",
                        column: x => x.examId,
                        principalTable: "Exam",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ExamQuestion_questionId_fkey",
                        column: x => x.questionId,
                        principalTable: "Question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LearningProgress",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    questionId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("LearningProgress_pkey", x => x.id);
                    table.ForeignKey(
                        name: "LearningProgress_questionId_fkey",
                        column: x => x.questionId,
                        principalTable: "Question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "LearningProgress_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionTag",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    questionId = table.Column<Guid>(type: "uuid", nullable: false),
                    tagId = table.Column<Guid>(type: "uuid", nullable: false),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("QuestionTag_pkey", x => x.id);
                    table.ForeignKey(
                        name: "QuestionTag_questionId_fkey",
                        column: x => x.questionId,
                        principalTable: "Question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "QuestionTag_tagId_fkey",
                        column: x => x.tagId,
                        principalTable: "Tag",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedQuestion",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    questionId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SavedQuestion_pkey", x => x.id);
                    table.ForeignKey(
                        name: "SavedQuestion_questionId_fkey",
                        column: x => x.questionId,
                        principalTable: "Question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "SavedQuestion_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentVote",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    forumCommentId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CommentVote_pkey", x => x.id);
                    table.ForeignKey(
                        name: "CommentVote_forumCommentId_fkey",
                        column: x => x.forumCommentId,
                        principalTable: "ForumComment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "CommentVote_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    simulationId = table.Column<Guid>(type: "uuid", nullable: true),
                    forumPostId = table.Column<Guid>(type: "uuid", nullable: true),
                    forumCommentId = table.Column<Guid>(type: "uuid", nullable: true),
                    questionId = table.Column<Guid>(type: "uuid", nullable: true),
                    reportCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    image = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Report_pkey", x => x.id);
                    table.ForeignKey(
                        name: "Report_forumCommentId_fkey",
                        column: x => x.forumCommentId,
                        principalTable: "ForumComment",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Report_forumPostId_fkey",
                        column: x => x.forumPostId,
                        principalTable: "ForumPost",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Report_questionId_fkey",
                        column: x => x.questionId,
                        principalTable: "Question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Report_reportCategoryId_fkey",
                        column: x => x.reportCategoryId,
                        principalTable: "ReportCategory",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Report_simulationId_fkey",
                        column: x => x.simulationId,
                        principalTable: "SimulationScenario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Report_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "SimulationSessionDetail",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    simulationExamId = table.Column<Guid>(type: "uuid", nullable: false),
                    simulationSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    durationSecond = table.Column<double>(type: "double precision", nullable: true),
                    score = table.Column<int>(type: "integer", nullable: true),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SimulationSessionDetail_pkey", x => x.id);
                    table.ForeignKey(
                        name: "SimulationSessionDetail_simulationExamId_fkey",
                        column: x => x.simulationExamId,
                        principalTable: "SimulationExam",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "SimulationSessionDetail_simulationSessionId_fkey",
                        column: x => x.simulationSessionId,
                        principalTable: "SimulationSession",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamDetail",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    answerId = table.Column<Guid>(type: "uuid", nullable: false),
                    examSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ExamDetail_pkey", x => x.id);
                    table.ForeignKey(
                        name: "ExamDetail_answerId_fkey",
                        column: x => x.answerId,
                        principalTable: "Answer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ExamDetail_examSessionId_fkey",
                        column: x => x.examSessionId,
                        principalTable: "ExamSession",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resolve",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    reportId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    createAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Resolve_pkey", x => x.id);
                    table.ForeignKey(
                        name: "Resolve_reportId_fkey",
                        column: x => x.reportId,
                        principalTable: "Report",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Resolve_userId_fkey",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answer_questionId",
                table: "Answer",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentVote_forumCommentId",
                table: "CommentVote",
                column: "forumCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentVote_userId",
                table: "CommentVote",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_userId",
                table: "Exam",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamDetail_answerId",
                table: "ExamDetail",
                column: "answerId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamDetail_examSessionId",
                table: "ExamDetail",
                column: "examSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestion_examId",
                table: "ExamQuestion",
                column: "examId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestion_questionId",
                table: "ExamQuestion",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSession_examId",
                table: "ExamSession",
                column: "examId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSession_userId",
                table: "ExamSession",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumComment_forumPostId",
                table: "ForumComment",
                column: "forumPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumComment_replyId",
                table: "ForumComment",
                column: "replyId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumComment_userId",
                table: "ForumComment",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumPost_forumTopicId",
                table: "ForumPost",
                column: "forumTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumPost_userId",
                table: "ForumPost",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgress_questionId",
                table: "LearningProgress",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgress_userId",
                table: "LearningProgress",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonImage_questionLessonId",
                table: "LessonImage",
                column: "questionLessonId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgress_questionLessonId",
                table: "LessonProgress",
                column: "questionLessonId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgress_userId",
                table: "LessonProgress",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_userId",
                table: "Payment",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_PostImage_forumPostId",
                table: "PostImage",
                column: "forumPostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReact_forumPostId",
                table: "PostReact",
                column: "forumPostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReact_userId",
                table: "PostReact",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_parentId",
                table: "Question",
                column: "parentId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_questionCategoryId",
                table: "Question",
                column: "questionCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_questionLessonId",
                table: "Question",
                column: "questionLessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_questionTopicId",
                table: "Question",
                column: "questionTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionChapter_drivingLicenseId",
                table: "QuestionChapter",
                column: "drivingLicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionLesson_questionChapterId",
                table: "QuestionLesson",
                column: "questionChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTag_questionId",
                table: "QuestionTag",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTag_tagId",
                table: "QuestionTag",
                column: "tagId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_forumCommentId",
                table: "Report",
                column: "forumCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_forumPostId",
                table: "Report",
                column: "forumPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_questionId",
                table: "Report",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_reportCategoryId",
                table: "Report",
                column: "reportCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_simulationId",
                table: "Report",
                column: "simulationId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_userId",
                table: "Report",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Resolve_reportId",
                table: "Resolve",
                column: "reportId");

            migrationBuilder.CreateIndex(
                name: "IX_Resolve_userId",
                table: "Resolve",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedQuestion_questionId",
                table: "SavedQuestion",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedQuestion_userId",
                table: "SavedQuestion",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedTrafficSign_trafficSignId",
                table: "SavedTrafficSign",
                column: "trafficSignId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedTrafficSign_userId",
                table: "SavedTrafficSign",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationExam_simulationId",
                table: "SimulationExam",
                column: "simulationId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationExam_situationExamId",
                table: "SimulationExam",
                column: "situationExamId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationScenario_simulationCategoryId",
                table: "SimulationScenario",
                column: "simulationCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationScenario_simulationChapterId",
                table: "SimulationScenario",
                column: "simulationChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationScenario_simulationDifficultyLevelId",
                table: "SimulationScenario",
                column: "simulationDifficultyLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationSession_situationExamId",
                table: "SimulationSession",
                column: "situationExamId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationSession_userId",
                table: "SimulationSession",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationSessionDetail_simulationExamId",
                table: "SimulationSessionDetail",
                column: "simulationExamId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationSessionDetail_simulationSessionId",
                table: "SimulationSessionDetail",
                column: "simulationSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SituationExam_userId",
                table: "SituationExam",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_TrafficSign_signCategoryId",
                table: "TrafficSign",
                column: "signCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_User_roleId",
                table: "User",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLicense_drivingLicenseId",
                table: "UserLicense",
                column: "drivingLicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLicense_userId",
                table: "UserLicense",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotification_notificationId",
                table: "UserNotification",
                column: "notificationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotification_userId",
                table: "UserNotification",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_drivingLicenseId",
                table: "Vehicle",
                column: "drivingLicenseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentVote");

            migrationBuilder.DropTable(
                name: "ExamDetail");

            migrationBuilder.DropTable(
                name: "ExamQuestion");

            migrationBuilder.DropTable(
                name: "LearningProgress");

            migrationBuilder.DropTable(
                name: "LessonImage");

            migrationBuilder.DropTable(
                name: "LessonProgress");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "PostImage");

            migrationBuilder.DropTable(
                name: "PostReact");

            migrationBuilder.DropTable(
                name: "QuestionTag");

            migrationBuilder.DropTable(
                name: "Resolve");

            migrationBuilder.DropTable(
                name: "SavedQuestion");

            migrationBuilder.DropTable(
                name: "SavedTrafficSign");

            migrationBuilder.DropTable(
                name: "SimulationSessionDetail");

            migrationBuilder.DropTable(
                name: "SystemConfig");

            migrationBuilder.DropTable(
                name: "UserLicense");

            migrationBuilder.DropTable(
                name: "UserNotification");

            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropTable(
                name: "Answer");

            migrationBuilder.DropTable(
                name: "ExamSession");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "TrafficSign");

            migrationBuilder.DropTable(
                name: "SimulationExam");

            migrationBuilder.DropTable(
                name: "SimulationSession");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Exam");

            migrationBuilder.DropTable(
                name: "ForumComment");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "ReportCategory");

            migrationBuilder.DropTable(
                name: "SignCategory");

            migrationBuilder.DropTable(
                name: "SimulationScenario");

            migrationBuilder.DropTable(
                name: "SituationExam");

            migrationBuilder.DropTable(
                name: "ForumPost");

            migrationBuilder.DropTable(
                name: "QuestionCategory");

            migrationBuilder.DropTable(
                name: "QuestionLesson");

            migrationBuilder.DropTable(
                name: "QuestionTopic");

            migrationBuilder.DropTable(
                name: "SimulationCategory");

            migrationBuilder.DropTable(
                name: "SimulationChapter");

            migrationBuilder.DropTable(
                name: "SimulationDifficultyLevel");

            migrationBuilder.DropTable(
                name: "ForumTopic");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "QuestionChapter");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "DrivingLicense");
        }
    }
}
