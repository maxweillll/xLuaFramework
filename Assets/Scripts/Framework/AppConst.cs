public enum GameMode
{
    EditorMode,
    PackageBundle,
    UpdateMode,
}

public enum GameEvent
{
    GameInit=10000,
    StartLua,
}

public class AppConst
{
    public const string BuildExtension = ".ab";
    public const string FileListName = "filelist.txt";

    public static bool OpenLog = true;
    //热更资源的地址 
    public const string ResourcesUrl = "http://192.168.99.47/AssetBundles";

    public static GameMode GameMode = GameMode.EditorMode;
}
