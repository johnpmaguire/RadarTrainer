using RadarTrainer.Models;

namespace RadarTrainer.Services;

public static class DemoScenario
{
    public static RadarState Create() => new(
        Speed: "6.2",
        Heading: "004",
        Depth: "18.4",
        Latitude: "50° 47.250′ N",
        Longitude: "001° 06.800′ W",
        RangeNauticalMiles: 4,
        Contacts:
        [
            new("Contact A", 34, 2.7),
            new("Contact B", 110, 1.8),
            new("Contact C", 232, 2.4),
            new("Contact D", 320, 3.2)
        ]);
}
