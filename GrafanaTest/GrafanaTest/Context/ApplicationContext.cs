using GrafanaTest.Context.Models;
using Microsoft.EntityFrameworkCore;

namespace GrafanaTest.Context;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> dbContextOptions)
        : base(dbContextOptions)
    {

    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<Dependent> Dependents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       => optionsBuilder.AddInterceptors(new GrafanaInterceptor());

}
