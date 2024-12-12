
int dayNumber = 12;

Type t = Type.GetType("Day"+dayNumber);
Day day = (Day)Activator.CreateInstance(t);

string testInput = Input.ReadTestInput(dayNumber);
string input = Input.ReadInput(dayNumber);

Console.WriteLine($"Star1 test Output: {day.Star1(testInput, true)}");
Console.WriteLine($"Star1 real Output: {day.Star1(input)}");

Console.WriteLine($"Star2 test Output: {day.Star2(testInput, true)}");
Console.WriteLine($"Star2 real Output: {day.Star2(input)}");