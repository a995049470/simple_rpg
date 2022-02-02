[System.Serializable]
public class Key<T> : IKey where T : Table, new()
{
    public int ID;
    private T value;
    public Key()
    {

    }
    public Key(int id)
    {
        this.ID = id;
    }
    public T Get()
    {
        return value;
    }
}

