namespace CodeFirst_EF.Repositories
{
    public interface IEntity
    {
        string Id { get; set; }
        string Word { get; set; }
        int Count { get; set; }
    }
}