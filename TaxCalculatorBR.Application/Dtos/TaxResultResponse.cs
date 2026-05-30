using TaxCalculatorBR.Domain.Entities;

namespace TaxCalculatorBR.Application.Dtos;

public record TaxResultResponse(
    string OriginState,
    string DestinationState,
    decimal IcmsValue,
    decimal IssValue,
    decimal IpiValue,
    decimal TotalTax,
    decimal FinalPrice
);