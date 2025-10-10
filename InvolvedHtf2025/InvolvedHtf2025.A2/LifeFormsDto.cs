namespace InvolvedHtf2025.A2;

public record LifeFormsDto(LifeFormDto[] LifeForms);

public record LifeFormDto(
    string Id,
    double BioluminescenceFrequency,
    string MovementPattern,
    string ThermalSignature,
    string SizeEstimate,
    string? SignalRhythm
);