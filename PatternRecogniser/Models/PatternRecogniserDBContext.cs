using Microsoft.EntityFrameworkCore;
using System.IO;

namespace PatternRecogniser.Models
{
    public class PatternRecogniserDBContext : DbContext
    {
        public PatternRecogniserDBContext(DbContextOptions<PatternRecogniserDBContext> options)
            : base(options)
        { }

        public DbSet<Experiment> experiment { get; set; }
        public DbSet<ExperimentList> experimentList { get; set; }
        public DbSet<ExtendedModel> extendedModel { get; set; }
        public DbSet<ValidationSet> validationSet { get; set; }
        public DbSet<Pattern> pattern { get; set; }
        public DbSet<ModelTrainingExperiment> modelTrainingExperiment { get; set;}
        public DbSet<PatternRecognitionExperiment> patternRecognitionExperiment { get; set; }
        public DbSet<RecognisedPatterns> recognisedPatterns { get; set; }
        public DbSet<User> user { get; set; }

    }

}
