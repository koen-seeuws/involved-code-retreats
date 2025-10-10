namespace InvolvedHtf2025.A2;

public record ConfigurationDto(CategoryDto[] Categories);

public record CategoryDto(string Name, int SpectralWeight, RuleDto[] Rules);

public record RuleDto(
    TraitDto? BioluminescenceFrequency,
    TraitDto? MovementPattern,
    TraitDto? ThermalSignature,
    TraitDto? SizeEstimate,
    TraitDto? SignalRhythm
);

public record TraitDto(int? Min, int? Max, string? Value, string? Pattern, double Weight);