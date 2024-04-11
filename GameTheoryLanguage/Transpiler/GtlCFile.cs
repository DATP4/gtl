class GtlCFile
{

    public void PrintFileToOutput(List<string> file)
    {
        string filepath = "Output/Output.cs";
        List<string> authors = file;
        File.WriteAllText(filepath, "");
        File.WriteAllLines(filepath, authors);
    }
}
