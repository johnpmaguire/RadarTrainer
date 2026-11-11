namespace RadarTrainer.Models;

public sealed record RadarState(
    string Speed,
    string Heading,
    string Depth,
    string Latitude,
    string Longitude,
    double RangeNauticalMiles,
    IReadOnlyList<RadarContact> Contacts);
