using System.Net.Http.Json;
using System.Text.Json;

namespace HTF2024.Puzzles;

public class PuzzleA3(HttpClient httpClient) : IPuzzle
{
    public async Task Run(bool forReal = false)
    {
        Console.WriteLine("Running A3");
        var sampleOrPuzzle = forReal ? "puzzle" : "sample";

        await httpClient.GetAsync("a/hard/start");

        var quatralianNumbersDto = await httpClient.GetFromJsonAsync<QuatralianNumbersDto>($"a/hard/{sampleOrPuzzle}");
        if (quatralianNumbersDto == null) throw new Exception();

        List<int> decimalNumbers = new List<int>();

        Console.WriteLine($"Quatralian numbers: {JsonSerializer.Serialize(quatralianNumbersDto.quatralianNumbers).Replace("\\u00B7","·")}");
        
        foreach (var quatralianNumber in quatralianNumbersDto.quatralianNumbers)
        {
            //Convert base 20 to base 10
            var decimalNumber = 0;
            var digits = quatralianNumber.Split(' ').Reverse().ToList();
            foreach (var (digit, index) in digits.Select((digit, index) => (digit, index)))
            {
                var digitInDecimal =
                    digit.ToCharArray().Count(dp => dp == '|') * 5 +
                    digit.ToCharArray().Count(dp => dp == '·');
                
                decimalNumber += digitInDecimal * (int)Math.Pow(20, index);
            }

            //Sum base 10 numbers
            decimalNumbers.Add(decimalNumber);
        }

        Console.WriteLine($"Decimal number: {JsonSerializer.Serialize(decimalNumbers)}");
        
        var sum = decimalNumbers.Sum();
        
        Console.WriteLine($"Decimal sum: {sum}");
        
        // Convert base 10 to base 20
        var result = string.Empty;

        while (sum > 0)
        {
            var remainder = sum % 20;
            sum /= 20;
    
            var quatralianReminder = string.Empty;

            // Handle case when remainder is 0
            if (remainder == 0)
            {
                quatralianReminder = "Ⱄ";
            }
            else
            {
                // Build the symbol based on remainder value
                int bars = remainder / 5;       // Number of '|' symbols
                int dots = remainder % 5;       // Number of '·' symbols

                // Append '|' symbols
                quatralianReminder = new string('|', bars);

                // Append '·' symbols
                quatralianReminder += new string('·', dots);
            }
    
            // Append the converted part to the result
            result = $"{quatralianReminder} {result}".Trim();
        }


        Console.WriteLine($"Quatralian sum: {result}");

        var response = await httpClient.PostAsJsonAsync($"a/hard/{sampleOrPuzzle}", result);

        try
        {
            response.EnsureSuccessStatusCode();
            Console.WriteLine($"A3 {sampleOrPuzzle} solved");
        }
        catch
        {
            Console.WriteLine($"A3 {sampleOrPuzzle} not solved");
        }
    }
}

class QuatralianNumbersDto
{
    public List<string> quatralianNumbers { get; set; }
}