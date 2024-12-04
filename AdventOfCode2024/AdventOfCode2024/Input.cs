﻿public static class Input
{
    const string basePath = @"..\..\..\Day"; //from build folder back to project folder
    public static string ReadTestInput(int dayNumber)
    {
        return File.ReadAllText(basePath + dayNumber + "/TestInput.txt");
    }
    public static string ReadInput(int dayNumber)
    {
        return File.ReadAllText(basePath + dayNumber + "/Input.txt");
    }
}