using Microsoft.EntityFrameworkCore;
using MyJetWallet.Sdk.Postgres;
using Service.DisclaimerEngine.Domain.Models;

namespace Service.DisclaimerEngine.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        public const string Schema = "disclaimers";

        public const string DisclaimerTableName = "disclaimers";
        public const string QuestionsTableName = "questions";
        public const string AnswersTableName = "answers";
        public const string ContextsTableName = "contexts";


        public DbSet<Disclaimer> Disclaimers { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<DisclaimerContext> Contexts { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.Entity<Disclaimer>().ToTable(DisclaimerTableName);
            modelBuilder.Entity<Disclaimer>().HasKey(e => e.Id);
            
            modelBuilder.Entity<Disclaimer>().HasIndex(e => e.Type);

            modelBuilder.Entity<Question>().ToTable(QuestionsTableName);
            modelBuilder.Entity<Question>().HasKey(e => e.Id);
            modelBuilder.Entity<Question>().HasOne<Disclaimer>().WithMany(t => t.Questions).HasForeignKey(t=>t.DisclaimerId);


            modelBuilder.Entity<DisclaimerContext>().ToTable(ContextsTableName);
            modelBuilder.Entity<DisclaimerContext>().HasKey(e => new {e.ClientId, e.DisclaimerId});
            modelBuilder.Entity<DisclaimerContext>().HasIndex(e => e.ClientId);
            modelBuilder.Entity<DisclaimerContext>().HasIndex(e => e.DisclaimerId);
            
            modelBuilder.Entity<Answer>().ToTable(AnswersTableName);
            modelBuilder.Entity<Answer>().HasKey(e => new{e.ClientId, e.QuestionId});
            modelBuilder.Entity<Answer>().HasOne<DisclaimerContext>().WithMany(t => t.Answers).HasForeignKey(t=>new{t.ClientId, t.DisclaimerId});

            base.OnModelCreating(modelBuilder);
        }

        public async Task<int> UpsertAsync(IEnumerable<Disclaimer> entities)
        {
            var result = await Disclaimers.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            result = await Questions.UpsertRange(entities.SelectMany(t => t.Questions)).AllowIdentityMatch().RunAsync();
            return result;
        }
        
        public async Task<int> UpsertAsync(IEnumerable<DisclaimerContext> entities)
        {
            var result = await Contexts.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            result = await Answers.UpsertRange(entities.SelectMany(t => t.Answers)).RunAsync();
            return result;
        }

        
    }
}
