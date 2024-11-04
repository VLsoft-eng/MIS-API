using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "banned_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_banned_tokens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ICD_10",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    createTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    parentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ICD_10", x => x.id);
                    table.ForeignKey(
                        name: "FK_ICD_10_ICD_10_parentId",
                        column: x => x.parentId,
                        principalTable: "ICD_10",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "patient",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    birthday = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    gender = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "speciality",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_speciality", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "doctor",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    hashed_password = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    birthday = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    speciality_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctor", x => x.id);
                    table.ForeignKey(
                        name: "FK_doctor_speciality_speciality_id",
                        column: x => x.speciality_id,
                        principalTable: "speciality",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inspection",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    anamnesis = table.Column<string>(type: "text", nullable: false),
                    complaints = table.Column<string>(type: "text", nullable: false),
                    treatment = table.Column<string>(type: "text", nullable: false),
                    conclustion = table.Column<int>(type: "integer", nullable: false),
                    next_visit_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    death_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_notified = table.Column<bool>(type: "boolean", nullable: false),
                    doctor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(name: "patient_id)", type: "uuid", nullable: false),
                    previous_inspection_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inspection", x => x.id);
                    table.ForeignKey(
                        name: "FK_inspection_doctor_doctor_id",
                        column: x => x.doctor_id,
                        principalTable: "doctor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inspection_inspection_previous_inspection_id",
                        column: x => x.previous_inspection_id,
                        principalTable: "inspection",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_inspection_patient_patient_id)",
                        column: x => x.patient_id,
                        principalTable: "patient",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "consultation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    speciality_id = table.Column<Guid>(type: "uuid", nullable: false),
                    inspection_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consultation", x => x.id);
                    table.ForeignKey(
                        name: "FK_consultation_inspection_inspection_id",
                        column: x => x.inspection_id,
                        principalTable: "inspection",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_consultation_speciality_speciality_id",
                        column: x => x.speciality_id,
                        principalTable: "speciality",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "diagnosis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    icd_diagnosis_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    diagnosis_type = table.Column<int>(name: "diagnosis_type ", type: "integer", nullable: false),
                    inspection_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_diagnosis", x => x.id);
                    table.ForeignKey(
                        name: "FK_diagnosis_ICD_10_icd_diagnosis_id",
                        column: x => x.icd_diagnosis_id,
                        principalTable: "ICD_10",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_diagnosis_inspection_inspection_id",
                        column: x => x.inspection_id,
                        principalTable: "inspection",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    consultation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comment", x => x.id);
                    table.ForeignKey(
                        name: "FK_comment_comment_parent_id",
                        column: x => x.parent_id,
                        principalTable: "comment",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_comment_consultation_consultation_id",
                        column: x => x.consultation_id,
                        principalTable: "consultation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comment_doctor_author_id",
                        column: x => x.author_id,
                        principalTable: "doctor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_banned_tokens_id",
                table: "banned_tokens",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_banned_tokens_token_value",
                table: "banned_tokens",
                column: "token_value",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_comment_author_id",
                table: "comment",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_comment_consultation_id",
                table: "comment",
                column: "consultation_id");

            migrationBuilder.CreateIndex(
                name: "IX_comment_parent_id",
                table: "comment",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_consultation_inspection_id",
                table: "consultation",
                column: "inspection_id");

            migrationBuilder.CreateIndex(
                name: "IX_consultation_speciality_id",
                table: "consultation",
                column: "speciality_id");

            migrationBuilder.CreateIndex(
                name: "IX_diagnosis_icd_diagnosis_id",
                table: "diagnosis",
                column: "icd_diagnosis_id");

            migrationBuilder.CreateIndex(
                name: "IX_diagnosis_inspection_id",
                table: "diagnosis",
                column: "inspection_id");

            migrationBuilder.CreateIndex(
                name: "IX_doctor_email",
                table: "doctor",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_doctor_phone",
                table: "doctor",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_doctor_speciality_id",
                table: "doctor",
                column: "speciality_id");

            migrationBuilder.CreateIndex(
                name: "IX_ICD_10_parentId",
                table: "ICD_10",
                column: "parentId");

            migrationBuilder.CreateIndex(
                name: "IX_inspection_doctor_id",
                table: "inspection",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "IX_inspection_patient_id)",
                table: "inspection",
                column: "patient_id)");

            migrationBuilder.CreateIndex(
                name: "IX_inspection_previous_inspection_id",
                table: "inspection",
                column: "previous_inspection_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "banned_tokens");

            migrationBuilder.DropTable(
                name: "comment");

            migrationBuilder.DropTable(
                name: "diagnosis");

            migrationBuilder.DropTable(
                name: "consultation");

            migrationBuilder.DropTable(
                name: "ICD_10");

            migrationBuilder.DropTable(
                name: "inspection");

            migrationBuilder.DropTable(
                name: "doctor");

            migrationBuilder.DropTable(
                name: "patient");

            migrationBuilder.DropTable(
                name: "speciality");
        }
    }
}
