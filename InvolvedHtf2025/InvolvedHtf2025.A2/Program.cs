using System.Net.Http.Json;
using System.Text.Json;
using InvolvedHtf2025.A2;
using InvolvedHtf2025.Common;

var path = 'A';
var difficulty = Difficulty.Medium;
var sample = false;

var client = await Htf2025ClientFactory.GetAuthenticatedClient();

await client.Start(path, difficulty);

var configuration = await client.GetFromJsonAsync<ConfigurationDto>($"{path}/{difficulty}/config");
if (configuration is null)
    throw new InvalidOperationException("Failed to get configuration");

var data = await client.GetData<LifeFormsDto>(path, difficulty, sample);

var unclassified = new CategoryDto("Unclassified", 0, null!);
var classifications = configuration.Categories.ToDictionary(c => c, c => new List<LifeFormDto>());
classifications[unclassified] = [];

foreach (var lifeForm in data.LifeForms)
{
    var categoryScores = new List<(CategoryDto, decimal)>();

    foreach (var category in configuration.Categories)
    {
        var rules = new Dictionary<string, RuleDto>(category.Rules, StringComparer.OrdinalIgnoreCase);
        var score = 0.0m;

        if (
            rules.TryGetValue(nameof(lifeForm.BioluminescenceFrequency), out var bioluminescenceFrequencyRule) &&
            bioluminescenceFrequencyRule is { Min: not null, Max: not null }
        )
        {
            if (lifeForm.BioluminescenceFrequency >= bioluminescenceFrequencyRule.Min &&
                lifeForm.BioluminescenceFrequency <= bioluminescenceFrequencyRule.Max)
            {
                score += bioluminescenceFrequencyRule.Weight;
            }
        }

        if (
            rules.TryGetValue(nameof(lifeForm.MovementPattern), out var movementPatternRule) &&
            !string.IsNullOrWhiteSpace(movementPatternRule.Value)
        )
        {
            if (lifeForm.MovementPattern.Equals(movementPatternRule.Value, StringComparison.OrdinalIgnoreCase))
            {
                score += movementPatternRule.Weight;
            }
        }

        /*
        if (
            rules.TryGetValue(nameof(lifeForm.SignalRhythm), out var signalRhythmRule) &&
            !string.IsNullOrWhiteSpace(signalRhythmRule.Pattern)
        )
        {
            if (
                !string.IsNullOrWhiteSpace(lifeForm.SignalRhythm) &&
                lifeForm.SignalRhythm.Equals(signalRhythmRule.Pattern, StringComparison.OrdinalIgnoreCase)
            )
            {
                score += signalRhythmRule.Weight;
            }
        }
        */

        if (
            rules.TryGetValue(nameof(lifeForm.SizeEstimate), out var sizeEstimateRule) &&
            !string.IsNullOrWhiteSpace(sizeEstimateRule.Value)
        )
        {
            if (
                !string.IsNullOrWhiteSpace(lifeForm.SizeEstimate) &&
                lifeForm.SizeEstimate.Equals(sizeEstimateRule.Value, StringComparison.OrdinalIgnoreCase)
            )
            {
                score += sizeEstimateRule.Weight;
            }
        }
        
        if (
            rules.TryGetValue(nameof(lifeForm.ThermalSignature), out var thermalSignatureRule) &&
            !string.IsNullOrWhiteSpace(thermalSignatureRule.Value)
        )
        {
            if (
                !string.IsNullOrWhiteSpace(lifeForm.ThermalSignature) &&
                lifeForm.ThermalSignature.Equals(thermalSignatureRule.Value, StringComparison.OrdinalIgnoreCase)
            )
            {
                score += thermalSignatureRule.Weight;
            }
        }
        
        if (score > 0)
            categoryScores.Add((category, score));
    }

    if (categoryScores.Count == 0)
    {
        classifications[unclassified].Add(lifeForm);
    }
    else
    {
        var bestCategory = categoryScores
            .OrderByDescending(cs => cs.Item2)
            .ThenBy(cs => cs.Item1.Name)
            .First()
            .Item1;
        classifications[bestCategory].Add(lifeForm);
    }
}

var highestCategory = classifications
    .OrderByDescending(c => c.Key.SpectralWeight * c.Value.Count)
    .ThenBy(c => c.Key.Name)
    .First()
    .Key;

var highestSpectralWeight = highestCategory.SpectralWeight * classifications[highestCategory].Count;

var totalSpectralWeight = classifications.Sum(c => c.Key.SpectralWeight * c.Value.Count);

var answer = $"{highestCategory.Name}_{highestSpectralWeight}_{totalSpectralWeight}";

Console.WriteLine(answer);

await client.SendAnswer(path, difficulty, answer, sample);