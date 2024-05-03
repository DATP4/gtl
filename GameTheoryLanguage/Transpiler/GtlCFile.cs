public class GtlCFile
{
    public void PrintFileToOutput(List<string> file)
    {
        string filepath = "output/src/main.rs";
        List<string> authors = file;
        File.WriteAllText(filepath, "");
        File.WriteAllLines(filepath, authors);
        //File.Delete(filepath); 
    }

    public void PrintMovesToFile(List<string> moves)
    {
        string filepath = "output/src/library/movs.rs";
        List<string> authors = moves;
        File.WriteAllText(filepath, "");
        File.WriteAllLines(filepath, authors);
    }
}
