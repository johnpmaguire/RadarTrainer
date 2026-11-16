using System.Globalization;
using RadarTrainer.Models;

namespace RadarTrainer.Services;

public static class MarpaScenario
{
    public static readonly TimeSpan Duration = TimeSpan.FromMinutes(5);

    private const double OwnCourse = 30;
    private const double OwnSpeed = 4.2;
    private const double TargetCourse = 40;
    private const double TargetSpeed = 12;
    private const double InitialRange = 3.8;
    private const double InitialRelativeBearing = 200;
    private const double InitialLatitude = 50 + 47.250 / 60;
    private const double InitialLongitude = -(1 + 6.800 / 60);

    public static MarpaSnapshot Create(TimeSpan elapsed)
    {
        var hours = Math.Clamp(elapsed.TotalHours, 0, Duration.TotalHours);
        var initialBearing = DegreesToRadians(OwnCourse + InitialRelativeBearing);
        var ownCourse = DegreesToRadians(OwnCourse);
        var targetCourse = DegreesToRadians(TargetCourse);
        var ownEastSpeed = OwnSpeed * Math.Sin(ownCourse);
        var ownNorthSpeed = OwnSpeed * Math.Cos(ownCourse);
        var relativeEastSpeed = TargetSpeed * Math.Sin(targetCourse) - ownEastSpeed;
        var relativeNorthSpeed = TargetSpeed * Math.Cos(targetCourse) - ownNorthSpeed;
        var ownEast = ownEastSpeed * hours;
        var ownNorth = ownNorthSpeed * hours;
        var relativeEast = InitialRange * Math.Sin(initialBearing) + relativeEastSpeed * hours;
        var relativeNorth = InitialRange * Math.Cos(initialBearing) + relativeNorthSpeed * hours;
        var range = Math.Sqrt(relativeEast * relativeEast + relativeNorth * relativeNorth);
        var trueBearing = (Math.Atan2(relativeEast, relativeNorth) * 180 / Math.PI + 360) % 360;
        var relativeBearing = (trueBearing - OwnCourse + 360) % 360;
        var relativeSpeedSquared = relativeEastSpeed * relativeEastSpeed + relativeNorthSpeed * relativeNorthSpeed;
        double? tcpaHours = relativeSpeedSquared > 0
            ? -(relativeEast * relativeEastSpeed + relativeNorth * relativeNorthSpeed) / relativeSpeedSquared
            : null;
        double? cpa = tcpaHours.HasValue
            ? Math.Sqrt(Math.Pow(relativeEast + relativeEastSpeed * tcpaHours.Value, 2) + Math.Pow(relativeNorth + relativeNorthSpeed * tcpaHours.Value, 2))
            : null;
        var latitude = InitialLatitude + ownNorth / 60;
        var longitude = InitialLongitude + ownEast / (60 * Math.Cos(DegreesToRadians(InitialLatitude)));

        var radar = new RadarState(
            Speed: OwnSpeed.ToString("0.0", CultureInfo.InvariantCulture),
            Heading: OwnCourse.ToString("000", CultureInfo.InvariantCulture),
            Depth: "18.4",
            Latitude: FormatCoordinate(latitude, true),
            Longitude: FormatCoordinate(longitude, false),
            RangeNauticalMiles: 4,
            Contacts: [new RadarContact("Contact A", relativeBearing, range)]);
        return new MarpaSnapshot(radar, trueBearing, TargetCourse, TargetSpeed, cpa, tcpaHours);
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;

    private static string FormatCoordinate(double coordinate, bool latitude)
    {
        var totalMinutes = Math.Round(Math.Abs(coordinate) * 60, 3, MidpointRounding.AwayFromZero);
        var degrees = (int)(totalMinutes / 60);
        var minutes = totalMinutes - degrees * 60;
        var hemisphere = latitude ? (coordinate < 0 ? "S" : "N") : (coordinate < 0 ? "W" : "E");
        var degreeFormat = latitude ? "00" : "000";
        return $"{degrees.ToString(degreeFormat, CultureInfo.InvariantCulture)}° {minutes.ToString("00.000", CultureInfo.InvariantCulture)}′ {hemisphere}";
    }
}
