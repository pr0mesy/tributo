using TaxCalculatorBR.Domain.Entities;
using TaxCalculatorBR.Domain.Enums;

namespace TaxCalculatorBR.Application.Dtos;

public record CalculateTaxRequest(
    string ProductName,
    decimal Price,
    ProductType Type,
    string OriginState,
    string DestinationState
);