namespace RadarTrainer.Models;

public sealed record MarpaSnapshot(
    RadarState Radar,
    double TargetTrueBearingDegrees,
    double TargetCourseDegrees,
    double TargetSpeedKnots,
    double? CpaNauticalMiles,
    double? TcpaHours);
