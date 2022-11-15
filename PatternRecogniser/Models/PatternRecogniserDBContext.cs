using Microsoft.EntityFrameworkCore;

namespace PatternRecogniser.Models
{
    public class PatternRecogniserDBContext : DbContext
    {
        public PatternRecogniserDBContext(DbContextOptions<PatternRecogniserDBContext> options)
            : base(options)
        { }

        public DbSet<Authentication> authentication { get; set; }
        public DbSet<Experiment> experiment { get; set; }
        public DbSet<ExperimentList> experimentList { get; set; }
        public DbSet<ExtendedModel> extendedModel { get; set; }
        public DbSet<ValidationSet> validationSet { get; set; }
        public DbSet<Pattern> pattern { get; set; }
        public DbSet<PatternData> patternData { get; set; }
        public DbSet<PatternRecognitionExperiment> patternRecognitionExperiment { get; set; }
        public DbSet<RecognizedPatterns> recognisedPatterns { get; set; }
        public DbSet<User> user { get; set; }

    }

}
