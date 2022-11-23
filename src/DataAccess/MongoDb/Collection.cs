namespace LShort.Lyoko.DataAccess.MongoDb;

[AttributeUsage(AttributeTargets.Class)]
public class Collection : Attribute
{
    public string Name { get; }

    public Collection(string name)
    {
        this.Name = name;
    }
}