using GymDogs.Domain.Exercises;
using GymDogs.Domain.ExerciseSets;
using GymDogs.Domain.FolderExercises;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.Users;
using GymDogs.Domain.WorkoutFolders;
using Microsoft.EntityFrameworkCore;

namespace GymDogs.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<WorkoutFolder> WorkoutFolders => Set<WorkoutFolder>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<FolderExercise> FolderExercises => Set<FolderExercise>();
    public DbSet<ExerciseSet> ExerciseSets => Set<ExerciseSet>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Seed Basic Exercises
        SeedExercises(modelBuilder);
    }

    /// <summary>
    /// Seeds the database with initial exercise data.
    /// Populates the Exercise table with 60 predefined exercises covering all muscle groups.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance</param>
    private void SeedExercises(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Using HasData with anonymous objects - EF Core maps automatically
        modelBuilder.Entity<Exercise>().HasData(
            // Peitoral
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Supino Reto", Description = "Exercício fundamental para desenvolvimento do peitoral maior", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111112"), Name = "Supino Inclinado", Description = "Trabalha principalmente a porção superior do peitoral", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111113"), Name = "Supino Declinado", Description = "Foca na porção inferior do peitoral", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111114"), Name = "Crucifixo", Description = "Exercício de isolamento para peitoral", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111115"), Name = "Flexão de Braço", Description = "Exercício de peso corporal para peitoral", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111116"), Name = "Peck Deck", Description = "Máquina para isolamento do peitoral", CreatedAt = seedDate, LastUpdatedAt = seedDate },

            // Costas
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222221"), Name = "Barra Fixa", Description = "Exercício fundamental para desenvolvimento das costas", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Remada Curvada", Description = "Trabalha latíssimo do dorso e romboides", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222223"), Name = "Puxada Frontal", Description = "Desenvolvimento do latíssimo do dorso", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222224"), Name = "Puxada Atrás", Description = "Variação da puxada para costas", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222225"), Name = "Remada Unilateral", Description = "Exercício unilateral para costas", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222226"), Name = "Remada no TRX", Description = "Exercício funcional para costas", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222227"), Name = "Serrote", Description = "Isolamento do serrátil anterior", CreatedAt = seedDate, LastUpdatedAt = seedDate },

            // Ombros
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333331"), Name = "Desenvolvimento com Halteres", Description = "Exercício fundamental para ombros", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333332"), Name = "Elevação Lateral", Description = "Isolamento do deltoide médio", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Elevação Frontal", Description = "Trabalha o deltoide anterior", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333334"), Name = "Elevação Posterior", Description = "Isolamento do deltoide posterior", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333335"), Name = "Desenvolvimento Arnold", Description = "Variação do desenvolvimento para ombros", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333336"), Name = "Crucifixo Invertido", Description = "Exercício para deltoide posterior", CreatedAt = seedDate, LastUpdatedAt = seedDate },

            // Bíceps
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444441"), Name = "Rosca Direta", Description = "Exercício fundamental para bíceps", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444442"), Name = "Rosca Alternada", Description = "Trabalho alternado dos bíceps", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444443"), Name = "Rosca Martelo", Description = "Trabalha bíceps e antebraço", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Rosca Concentrada", Description = "Isolamento do bíceps", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444445"), Name = "Rosca Scott", Description = "Isolamento do bíceps no banco Scott", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444446"), Name = "Rosca 21", Description = "Variação de rosca com 3 fases", CreatedAt = seedDate, LastUpdatedAt = seedDate },

            // Tríceps
            new { Id = Guid.Parse("55555555-5555-5555-5555-555555555551"), Name = "Tríceps Pulley", Description = "Exercício fundamental para tríceps", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("55555555-5555-5555-5555-555555555552"), Name = "Tríceps Testa", Description = "Isolamento do tríceps deitado", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("55555555-5555-5555-5555-555555555553"), Name = "Tríceps Coice", Description = "Isolamento unilateral do tríceps", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("55555555-5555-5555-5555-555555555554"), Name = "Paralelas", Description = "Exercício de peso corporal para tríceps", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Tríceps Francês", Description = "Isolamento do tríceps com halteres", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("55555555-5555-5555-5555-555555555556"), Name = "Mergulho", Description = "Exercício composto para tríceps e peitoral", CreatedAt = seedDate, LastUpdatedAt = seedDate },

            // Pernas - Quadríceps
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666661"), Name = "Agachamento Livre", Description = "Exercício fundamental para pernas", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666662"), Name = "Agachamento com Barra", Description = "Agachamento com carga para quadríceps", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666663"), Name = "Leg Press", Description = "Exercício de máquina para pernas", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666664"), Name = "Extensão de Pernas", Description = "Isolamento do quadríceps", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666665"), Name = "Afundo", Description = "Exercício unilateral para pernas", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Agachamento Búlgaro", Description = "Variação de agachamento unilateral", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666667"), Name = "Hack Squat", Description = "Agachamento na máquina hack", CreatedAt = seedDate, LastUpdatedAt = seedDate },

            // Pernas - Posterior
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777771"), Name = "Mesa Flexora", Description = "Isolamento do posterior da coxa", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777772"), Name = "Stiff", Description = "Exercício para posterior e glúteos", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777773"), Name = "Levantamento Terra", Description = "Exercício composto para posterior e costas", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777774"), Name = "Cadeira Flexora", Description = "Isolamento do posterior na máquina", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777775"), Name = "Good Morning", Description = "Exercício para posterior e glúteos", CreatedAt = seedDate, LastUpdatedAt = seedDate },

            // Glúteos
            new { Id = Guid.Parse("88888888-8888-8888-8888-888888888881"), Name = "Elevação Pélvica", Description = "Exercício fundamental para glúteos", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("88888888-8888-8888-8888-888888888882"), Name = "Abdução de Quadril", Description = "Isolamento do glúteo médio", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("88888888-8888-8888-8888-888888888883"), Name = "Avanço", Description = "Exercício unilateral para glúteos e pernas", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("88888888-8888-8888-8888-888888888884"), Name = "Agachamento Sumô", Description = "Variação de agachamento para glúteos", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("88888888-8888-8888-8888-888888888885"), Name = "Kickback", Description = "Isolamento do glúteo máximo", CreatedAt = seedDate, LastUpdatedAt = seedDate },

            // Panturrilhas
            new { Id = Guid.Parse("99999999-9999-9999-9999-999999999991"), Name = "Panturrilha em Pé", Description = "Exercício fundamental para panturrilhas", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("99999999-9999-9999-9999-999999999992"), Name = "Panturrilha Sentado", Description = "Trabalha principalmente o sóleo", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("99999999-9999-9999-9999-999999999993"), Name = "Panturrilha no Leg Press", Description = "Panturrilha na máquina leg press", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("99999999-9999-9999-9999-999999999994"), Name = "Panturrilha Unilateral", Description = "Exercício unilateral para panturrilhas", CreatedAt = seedDate, LastUpdatedAt = seedDate },

            // Abdômen
            new { Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAA1"), Name = "Abdominal Reto", Description = "Exercício fundamental para abdômen", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAA2"), Name = "Prancha", Description = "Exercício isométrico para core", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAA3"), Name = "Abdominal Infra", Description = "Trabalha a porção inferior do abdômen", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAA4"), Name = "Abdominal Oblíquo", Description = "Trabalha os músculos laterais do abdômen", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAA5"), Name = "Mountain Climber", Description = "Exercício funcional para core", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAA6"), Name = "Abdominal Bicicleta", Description = "Exercício para abdômen e oblíquos", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAA7"), Name = "Russian Twist", Description = "Exercício rotacional para core", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAA8"), Name = "Dead Bug", Description = "Exercício de estabilização do core", CreatedAt = seedDate, LastUpdatedAt = seedDate },

            // Antebraços
            new { Id = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBB1"), Name = "Rosca de Punho", Description = "Exercício para antebraços", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBB2"), Name = "Rosca de Punho Inversa", Description = "Trabalha extensores do antebraço", CreatedAt = seedDate, LastUpdatedAt = seedDate },
            new { Id = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBB3"), Name = "Farmer's Walk", Description = "Exercício funcional para antebraços e grip", CreatedAt = seedDate, LastUpdatedAt = seedDate }
        );
    }
}
