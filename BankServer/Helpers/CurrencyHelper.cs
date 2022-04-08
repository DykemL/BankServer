using BankServer.Models.DbEntities;

namespace BankServer.Helpers;

public class CurrencyHelper
{
    public const string Rubble = "Rubble";
    public const string Dollar = "Dollar";
    public const string Euro = "Euro";

    public static Currency? GetDefaultRubbleCurrency()
        => new()
        {
            Name = Rubble,
            NameRus = "Рубль",
            Power = 100
        };

    public static Currency? GetDefaultDollarCurrency()
        => new()
        {
            Name = Dollar,
            NameRus = "Доллар",
            Power = 5000
        };

    public static Currency? GetDefaultEuroCurrency()
        => new()
        {
            Name = Euro,
            NameRus = "Евро",
            Power = 6000
        };
}