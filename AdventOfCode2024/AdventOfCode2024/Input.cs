public static class Input
{
    const string basePath = @"..\..\..\Day "; //from build folder back to project folder
    public static string ReadTestInput(int dayNumber)
    {
        return File.ReadAllText(basePath + dayNumber.ToString("00") + "/TestInput.txt");
    }
    public static string ReadInput(int dayNumber)
    {
        return File.ReadAllText(basePath + dayNumber.ToString("00") + "/Input.txt");
    }
}