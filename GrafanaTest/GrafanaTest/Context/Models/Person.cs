namespace GrafanaTest.Context.Models;

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateOnly BirthDate { get; set; }
    public IEnumerable<Dependent> Dependents { get; set; }
}
