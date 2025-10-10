namespace InvolvedHtf2025.A2;

public record ConfigurationDto(CategoryDto[] Categories);

public record CategoryDto(string Name, int SpectralWeight, Dictionary<string, RuleDto> Rules);



public record RuleDto(decimal? Min, decimal? Max, string? Value, string? Pattern, decimal Weight);