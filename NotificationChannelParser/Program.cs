using System.Text.RegularExpressions;

Dictionary<string, string> Channels = new()
{
    { "BE", "Backend" },
    { "FE", "Frontend" },
    { "QA", "Quality Assurance" },
    { "Urgent", "Urgent" }
};

string? Input;
List<string>? Output;
bool IsTryAgain;
MatchCollection? OpenOnly = null, CloseOnly = null, Empty = null;

do
{
    Console.Clear();
    Operation(out Output);
    Console.Write($"Do you want to try again?" +
        $"{(Output?.Count == 0 ? " (Recommend)" : "")}" +
        $" (Default: {(Output?.Count == 0 ? "Y": "N")}) " +
        $"[Y/N]: ");
    IsTryAgain = string.Equals(Console.ReadLine()?.Trim(), "Y", StringComparison.OrdinalIgnoreCase);
    IsTryAgain = Output?.Count == 0;
} while (IsTryAgain);

void Operation(out List<string>? Output)
{
    Output = [];
    do
    {
        Console.Clear();

        // Console Out Invalid Input Prompts
        OpenBracketOnlyInvalid(OpenOnly);
        CloseBracketOnlyInvalid(CloseOnly);
        EmptyBracketInvalid(Empty);

        Console.Write("Input: ");
        Input = Console.ReadLine()?.Trim();

        // check only [ or [text
        OpenOnly = Regex.Matches(Input ?? string.Empty, @"\[([^]]*)$");

        // check only text] or ]
        CloseOnly = Regex.Matches(Input ?? string.Empty, @"^([^[]*)\]");

        // check []
        Empty = Regex.Matches(Input ?? string.Empty, @"\[\]");

    } while (string.IsNullOrEmpty(Input) || OpenOnly.Count != 0 || CloseOnly.Count != 0 || Empty.Count != 0);

    // retreive tags that match [text]
    MatchCollection InputChannels = Regex.Matches(Input, @"\[(.*?)\]");

    foreach (Match match in InputChannels)
    {
        string ReceivedChannel = match.Value.Replace("[", "").Replace("]", "").Trim();
        if (Channels.ContainsKey(ReceivedChannel))
        {
            Output.Add(ReceivedChannel);
        }
    }

    // remove [text] from input text!
    Input = Regex.Replace(Input, @"\[(.*?)\]", "");
    Console.WriteLine($"Message: {(string.IsNullOrEmpty(Input) ? "No message received!" : Input.Trim())}");
    Console.WriteLine($"Receive channel: {(Output.Count == 0
        ? "No channel receieved!"
        : string.Join(", ", Output))}"
        );
}

void OpenBracketOnlyInvalid(MatchCollection? OpenOnly)
{
    if (OpenOnly != null)
    {
        foreach (Match match in OpenOnly)
        {
            Console.WriteLine($"Invalid: {match.Value} => must close the bracket \"]\"!");
        }
    }
}

void CloseBracketOnlyInvalid(MatchCollection? CloseOnly)
{
    if (CloseOnly != null)
    {
        foreach (Match match in CloseOnly)
        {
            Console.WriteLine($"Invalid: {match.Value}, must open the bracket \"[\"!");
        }
    }
}

void EmptyBracketInvalid(MatchCollection? Empty)
{
    if (Empty != null)
    {
        foreach (Match match in Empty)
        {
            Console.WriteLine($"Invalid: {match.Value}, must not be empty!");
        }
    }
}