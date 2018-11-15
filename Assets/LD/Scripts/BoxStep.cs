public enum ItemType
{
    highStep = 0,
    lowChasm = 1
}

public class LandformItem
{
    public ItemType type;
    public float minX;
    public float maxX;
}


[System.Serializable]
public class FlatBoxStep : LandformItem
{
    public float maxY;
}

[System.Serializable]
public class Chasm : LandformItem
{
    public float depth;
}

[System.Serializable]
public class LandformInterval
{
    public float minX;
    public float maxX;
    public float y;
}


