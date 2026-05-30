using TaxCalculatorBR.Domain.Entities;

namespace TaxCalculatorBR.Domain.Interfaces;

public interface ITaxCalculator
{
    decimal Calculate(Product product, string originState, string destinationState);
}