using RadarTrainer.Models;

namespace RadarTrainer.Services;

public static class TargetAcquisition
{
    public const double BearingToleranceDegrees = 2;
    public const double RangeToleranceNauticalMiles = 0.2;
    private const double ComparisonEpsilon = 1e-10;

    public static bool IsWithinTolerance(RadarContact contact, double relativeBearing, double range)
    {
        var bearingDifference = Math.Abs((relativeBearing - contact.RelativeBearingDegrees) % 360);
        bearingDifference = Math.Min(bearingDifference, 360 - bearingDifference);
        return bearingDifference <= BearingToleranceDegrees + ComparisonEpsilon &&
               Math.Abs(range - contact.RangeNauticalMiles) <= RangeToleranceNauticalMiles + ComparisonEpsilon;
    }
}
