using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GymDogs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "CreatedAt", "Description", "LastUpdatedAt", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício fundamental para desenvolvimento do peitoral maior", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Supino Reto" },
                    { new Guid("11111111-1111-1111-1111-111111111112"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trabalha principalmente a porção superior do peitoral", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Supino Inclinado" },
                    { new Guid("11111111-1111-1111-1111-111111111113"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Foca na porção inferior do peitoral", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Supino Declinado" },
                    { new Guid("11111111-1111-1111-1111-111111111114"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício de isolamento para peitoral", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Crucifixo" },
                    { new Guid("11111111-1111-1111-1111-111111111115"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício de peso corporal para peitoral", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Flexão de Braço" },
                    { new Guid("11111111-1111-1111-1111-111111111116"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Máquina para isolamento do peitoral", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Peck Deck" },
                    { new Guid("22222222-2222-2222-2222-222222222221"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício fundamental para desenvolvimento das costas", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Barra Fixa" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trabalha latíssimo do dorso e romboides", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Remada Curvada" },
                    { new Guid("22222222-2222-2222-2222-222222222223"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Desenvolvimento do latíssimo do dorso", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Puxada Frontal" },
                    { new Guid("22222222-2222-2222-2222-222222222224"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Variação da puxada para costas", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Puxada Atrás" },
                    { new Guid("22222222-2222-2222-2222-222222222225"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício unilateral para costas", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Remada Unilateral" },
                    { new Guid("22222222-2222-2222-2222-222222222226"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício funcional para costas", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Remada no TRX" },
                    { new Guid("22222222-2222-2222-2222-222222222227"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Isolamento do serrátil anterior", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Serrote" },
                    { new Guid("33333333-3333-3333-3333-333333333331"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício fundamental para ombros", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Desenvolvimento com Halteres" },
                    { new Guid("33333333-3333-3333-3333-333333333332"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Isolamento do deltoide médio", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Elevação Lateral" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trabalha o deltoide anterior", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Elevação Frontal" },
                    { new Guid("33333333-3333-3333-3333-333333333334"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Isolamento do deltoide posterior", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Elevação Posterior" },
                    { new Guid("33333333-3333-3333-3333-333333333335"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Variação do desenvolvimento para ombros", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Desenvolvimento Arnold" },
                    { new Guid("33333333-3333-3333-3333-333333333336"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício para deltoide posterior", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Crucifixo Invertido" },
                    { new Guid("44444444-4444-4444-4444-444444444441"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício fundamental para bíceps", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rosca Direta" },
                    { new Guid("44444444-4444-4444-4444-444444444442"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trabalho alternado dos bíceps", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rosca Alternada" },
                    { new Guid("44444444-4444-4444-4444-444444444443"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trabalha bíceps e antebraço", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rosca Martelo" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Isolamento do bíceps", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rosca Concentrada" },
                    { new Guid("44444444-4444-4444-4444-444444444445"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Isolamento do bíceps no banco Scott", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rosca Scott" },
                    { new Guid("44444444-4444-4444-4444-444444444446"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Variação de rosca com 3 fases", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rosca 21" },
                    { new Guid("55555555-5555-5555-5555-555555555551"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício fundamental para tríceps", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tríceps Pulley" },
                    { new Guid("55555555-5555-5555-5555-555555555552"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Isolamento do tríceps deitado", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tríceps Testa" },
                    { new Guid("55555555-5555-5555-5555-555555555553"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Isolamento unilateral do tríceps", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tríceps Coice" },
                    { new Guid("55555555-5555-5555-5555-555555555554"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício de peso corporal para tríceps", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Paralelas" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Isolamento do tríceps com halteres", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tríceps Francês" },
                    { new Guid("55555555-5555-5555-5555-555555555556"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício composto para tríceps e peitoral", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mergulho" },
                    { new Guid("66666666-6666-6666-6666-666666666661"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício fundamental para pernas", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Agachamento Livre" },
                    { new Guid("66666666-6666-6666-6666-666666666662"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Agachamento com carga para quadríceps", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Agachamento com Barra" },
                    { new Guid("66666666-6666-6666-6666-666666666663"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício de máquina para pernas", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Leg Press" },
                    { new Guid("66666666-6666-6666-6666-666666666664"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Isolamento do quadríceps", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Extensão de Pernas" },
                    { new Guid("66666666-6666-6666-6666-666666666665"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício unilateral para pernas", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Afundo" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Variação de agachamento unilateral", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Agachamento Búlgaro" },
                    { new Guid("66666666-6666-6666-6666-666666666667"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Agachamento na máquina hack", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hack Squat" },
                    { new Guid("77777777-7777-7777-7777-777777777771"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Isolamento do posterior da coxa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mesa Flexora" },
                    { new Guid("77777777-7777-7777-7777-777777777772"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício para posterior e glúteos", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Stiff" },
                    { new Guid("77777777-7777-7777-7777-777777777773"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício composto para posterior e costas", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Levantamento Terra" },
                    { new Guid("77777777-7777-7777-7777-777777777774"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Isolamento do posterior na máquina", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cadeira Flexora" },
                    { new Guid("77777777-7777-7777-7777-777777777775"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício para posterior e glúteos", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Good Morning" },
                    { new Guid("88888888-8888-8888-8888-888888888881"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício fundamental para glúteos", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Elevação Pélvica" },
                    { new Guid("88888888-8888-8888-8888-888888888882"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Isolamento do glúteo médio", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Abdução de Quadril" },
                    { new Guid("88888888-8888-8888-8888-888888888883"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício unilateral para glúteos e pernas", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Avanço" },
                    { new Guid("88888888-8888-8888-8888-888888888884"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Variação de agachamento para glúteos", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Agachamento Sumô" },
                    { new Guid("88888888-8888-8888-8888-888888888885"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Isolamento do glúteo máximo", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kickback" },
                    { new Guid("99999999-9999-9999-9999-999999999991"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício fundamental para panturrilhas", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Panturrilha em Pé" },
                    { new Guid("99999999-9999-9999-9999-999999999992"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trabalha principalmente o sóleo", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Panturrilha Sentado" },
                    { new Guid("99999999-9999-9999-9999-999999999993"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Panturrilha na máquina leg press", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Panturrilha no Leg Press" },
                    { new Guid("99999999-9999-9999-9999-999999999994"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício unilateral para panturrilhas", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Panturrilha Unilateral" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício fundamental para abdômen", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Abdominal Reto" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício isométrico para core", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Prancha" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trabalha a porção inferior do abdômen", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Abdominal Infra" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trabalha os músculos laterais do abdômen", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Abdominal Oblíquo" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício funcional para core", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mountain Climber" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício para abdômen e oblíquos", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Abdominal Bicicleta" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício rotacional para core", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Russian Twist" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa8"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício de estabilização do core", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dead Bug" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício para antebraços", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rosca de Punho" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trabalha extensores do antebraço", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rosca de Punho Inversa" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exercício funcional para antebraços e grip", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Farmer's Walk" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111112"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111113"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111114"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111115"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111116"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222221"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222223"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222224"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222225"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222226"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222227"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333331"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333332"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333334"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333335"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333336"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444441"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444442"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444443"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444445"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444446"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555551"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555552"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555553"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555554"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555556"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666661"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666662"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666663"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666664"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666665"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666667"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777771"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777772"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777773"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777774"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777775"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888881"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888882"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888883"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888884"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888885"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999991"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999992"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999993"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999994"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa8"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"));
        }
    }
}
