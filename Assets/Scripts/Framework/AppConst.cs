public enum GameMode
{
    EditorMode,
    PackgeBundle,
    UpdateMode,
}
public class AppConst
{
    public const string BuildExtension = ".ab";
    public const string FileListName = "filelist.txt";

    public static GameMode GameMode = GameMode.EditorMode;
}
