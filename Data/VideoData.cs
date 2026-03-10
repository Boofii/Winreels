namespace Winreels.Data;

public class VideoData
{
    public readonly string path;
    public readonly string description;
    public readonly string[] tags;
    public string[] likes;

    public VideoData(string path, string description, string[] tags, string[] likes)
    {
        this.path = path;
        this.description = description;
        this.tags = tags;
        this.likes = likes;
    }
}