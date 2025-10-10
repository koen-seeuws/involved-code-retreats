using System.Text.RegularExpressions;
using InvolvedHtf2025.A1;
using InvolvedHtf2025.Common;

var path = 'A';
var difficulty = Difficulty.Easy;
var sample = false;

var client = await Htf2025ClientFactory.GetAuthenticatedClient();

await client.Start(path, difficulty);

var data = await client.GetData<DnaStrandsDto>(path, difficulty, sample);

var nucleotideBaseValues = new Dictionary<char, int>()
{
    { 'A', 1 },
    { 'T', 2 },
    { 'C', 3 },
    { 'G', 4 },
};

var answer = 0;

foreach (var dnaStrand in data.DnaStrands)
{
    var containsMoreThan4ConsecutiveNucleotides = Regex.IsMatch(dnaStrand, @"(.)\1{3,}");

    if (containsMoreThan4ConsecutiveNucleotides)
        continue;

    var nucleotides = dnaStrand.ToUpper().ToCharArray();

    var containsInvalidNucleotides = nucleotides.Any(nucleotide => !nucleotideBaseValues.ContainsKey(nucleotide));

    if (containsInvalidNucleotides)
        continue;

    var evenNumberOfNucleotides = nucleotides.Length % 2 == 0;

    if (!evenNumberOfNucleotides)
        continue;

    var previousNucleotide = nucleotides.First();
    answer += nucleotideBaseValues[previousNucleotide];
    var multiplier = 1;

    foreach (var nucleotide in nucleotides.Skip(1))
    {
        if (nucleotide == previousNucleotide)
            multiplier++;
        else
            multiplier = 1;

        answer += nucleotideBaseValues[nucleotide] * multiplier;
        previousNucleotide = nucleotide;
    }
}

await client.SendAnswer(path, difficulty, answer, sample);