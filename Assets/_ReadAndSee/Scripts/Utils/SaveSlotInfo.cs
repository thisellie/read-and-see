public class SaveSlotInfo
{
    public string fileName;
    public string playerName;
    public string lastSavedTime;
    public bool isEmpty;

    public SaveSlotInfo(string file, string name, string time, bool empty)
    {
        fileName = file;
        playerName = name;
        lastSavedTime = time;
        isEmpty = empty;
    }
}