[System.Serializable]
public class LocationResponse
{
    public Location location;

    [System.Serializable]
    public class Location
    {
        public string name;
    }
}
