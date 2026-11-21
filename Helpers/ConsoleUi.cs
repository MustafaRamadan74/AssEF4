using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace BankManagement.Helpers;

public static class ConsoleUi
{
    public static void Success(string message) => WriteColored(message, ConsoleColor.Green);

    public static void Info(string message) => WriteColored(message, ConsoleColor.Cyan);

    public static void Error(string message) => WriteColored(message, ConsoleColor.Red);

    private static void WriteColored(string message, ConsoleColor color)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = previous;
    }

    public static void Clear()
    {
        try
        {
            Console.Clear();
        }
        catch (IOException)
        {
        }
    }

    public static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press any key to return to the menu...");
        Console.ReadKey(true);
    }

    public static string ReadText(string label, int maxLength)
    {
        while (true)
        {
            Console.Write($"{label,-15}: ");
            var value = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(value))
            {
                Error("  This field is required.");
                continue;
            }

            if (value.Length > maxLength)
            {
                Error($"  Maximum length is {maxLength} characters.");
                continue;
            }

            return value;
        }
    }

    public static string ReadCode(string label, int maxLength)
    {
        while (true)
        {
            var value = ReadText(label, maxLength);

            if (value.Any(char.IsWhiteSpace))
            {
                Error("  Spaces are not allowed.");
                continue;
            }

            return value.ToUpperInvariant();
        }
    }

    public static string ReadDigits(string label, int minLength, int maxLength)
    {
        while (true)
        {
            Console.Write($"{label,-15}: ");
            var value = Console.ReadLine()?.Trim() ?? string.Empty;

            if (value.Length >= minLength && value.Length <= maxLength && value.All(c => c is >= '0' and <= '9'))
            {
                return value;
            }

            Error($"  Please enter {minLength}-{maxLength} digits only.");
        }
    }

    public static DateTime ReadPastDate(string label)
    {
        while (true)
        {
            Console.Write($"{label,-15}: (yyyy-MM-dd) ");
            var text = Console.ReadLine()?.Trim();

            if (DateTime.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                && date <= DateTime.Today
                && date.Year >= 1900)
            {
                return date;
            }

            Error("  Invalid date. Use yyyy-MM-dd and a date in the past.");
        }
    }

    public static string ReadEmail(string label, int maxLength)
    {
        while (true)
        {
            var value = ReadText(label, maxLength);

            if (MailAddress.TryCreate(value, out var address) && address.Address == value)
            {
                return value;
            }

            Error("  Invalid email address.");
        }
    }

    public static string ReadPhone(string label)
    {
        while (true)
        {
            var value = ReadText(label, 20);

            if (Regex.IsMatch(value, "^\\+?[0-9]{7,15}$"))
            {
                return value;
            }

            Error("  Invalid phone number. Use 7-15 digits (optionally starting with +).");
        }
    }

    public static int ReadPositiveInt(string label)
    {
        while (true)
        {
            Console.Write($"{label,-15}: ");

            if (int.TryParse(Console.ReadLine(), out var number) && number > 0)
            {
                return number;
            }

            Error("  Please enter a positive whole number.");
        }
    }

    public static T ReadEnum<T>(string title) where T : struct, Enum
    {
        var values = Enum.GetValues<T>();

        while (true)
        {
            Console.WriteLine($"{title}:");
            for (var i = 0; i < values.Length; i++)
            {
                Console.WriteLine($"    {i + 1}) {values[i]}");
            }

            Console.Write("  Choice: ");

            if (int.TryParse(Console.ReadLine(), out var number) && number >= 1 && number <= values.Length)
            {
                return values[number - 1];
            }

            Error($"  Please enter a number between 1 and {values.Length}.");
        }
    }
}
