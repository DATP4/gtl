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
}
