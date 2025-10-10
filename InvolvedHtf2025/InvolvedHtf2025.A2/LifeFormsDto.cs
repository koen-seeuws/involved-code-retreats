namespace InvolvedHtf2025.A2;

public record LifeFormsDto(LifeFormDto[] LifeForms);

public record LifeFormDto(
    string Id,
    decimal BioluminescenceFrequency,
    string MovementPattern,
    string ThermalSignature,
    string SizeEstimate,
    string? SignalRhythm
);