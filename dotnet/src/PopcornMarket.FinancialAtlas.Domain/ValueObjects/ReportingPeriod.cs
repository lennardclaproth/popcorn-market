using Popcorn.FinancialAtlas.Domain.Enums;

namespace Popcorn.FinancialAtlas.Domain.ValueObjects;

public record ReportingPeriod
{
    public int Year { get; }
    public PeriodType Type { get; }
    public int PeriodNumber { get; }
    
    public ReportingPeriod(int year, PeriodType type, int periodNumber)
    {
        if (periodNumber < 1 || periodNumber > (type == PeriodType.Quarterly ? 4 : type == PeriodType.HalfYearly ? 2 : 1))
            throw new ArgumentOutOfRangeException(nameof(periodNumber), "Invalid period number for type.");

        Year = year;
        Type = type;
        PeriodNumber = periodNumber;
    }
    
    public int ToComparableValue() =>
        Type switch
        {
            PeriodType.Yearly => Year,
            PeriodType.HalfYearly => Year * 2 + (PeriodNumber - 1),
            PeriodType.Quarterly => Year * 4 + (PeriodNumber - 1),
            _ => throw new InvalidOperationException("Unsupported type")
        };

    public override string ToString() =>
        Type switch
        {
            PeriodType.Yearly => $"{Year}",
            PeriodType.HalfYearly => $"{Year} H{PeriodNumber}",
            PeriodType.Quarterly => $"{Year} Q{PeriodNumber}",
            _ => $"{Year} P{PeriodNumber}"
        };

    public int CompareTo(ReportingPeriod? other)
    {
        if (other is null) return 1;
        return ToComparableValue().CompareTo(other.ToComparableValue());
    }

    public static bool operator <(ReportingPeriod left, ReportingPeriod right) =>
        left.ToComparableValue() < right.ToComparableValue();

    public static bool operator >(ReportingPeriod left, ReportingPeriod right) =>
        left.ToComparableValue() > right.ToComparableValue();
}
