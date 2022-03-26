using BankServer.Models.DbEntities;

namespace BankServer.Providers;

public class CurrencyHelper
{
    public const string Rubble = "Rubble";
    public const string Dollar = "Dollar";
    public const string Euro = "Euro";

    public static Currency? GetDefaultRubbleCurrency()
        => new()
        {
            Name = Rubble,
            Power = 1
        };

    public static Currency? GetDefaultDollarCurrency()
        => new()
        {
            Name = Dollar,
            Power = 100
        };

    public static Currency? GetDefaultEuroCurrency()
        => new()
        {
            Name = Euro,
            Power = 110
        };
}