using TaxCalculatorBR.Application.Dtos;
using TaxCalculatorBR.Domain.Entities;

namespace TaxCalculatorBR.Application.Mappers;

public class TaxMapper
{
    public static TaxResultResponse ToDto(TaxResult taxResult) =>
        new(taxResult.OriginState,
            taxResult.DestinationState,
            taxResult.IcmsValue,
            taxResult.IssValue,
            taxResult.IpiValue,
            taxResult.TotalTax,
            taxResult.FinalPrice
        );

    public static Product ToEntity(CalculateTaxRequest request) =>
        new()
        {
            Id = Guid.NewGuid(),
            Type = request.Type,
            Name = request.ProductName,
            Price = request.Price
        };
    
    // da pra fazer um UpdateEntity() dps se necessario

}