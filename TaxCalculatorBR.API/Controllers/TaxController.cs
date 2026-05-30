using Microsoft.AspNetCore.Mvc;
using TaxCalculatorBR.Application.Dtos;
using TaxCalculatorBR.Application.Mappers;
using TaxCalculatorBR.Domain.Entities;
using TaxCalculatorBR.Domain.Interfaces;

namespace TaxCalculatorBR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaxController : ControllerBase
{
    
    private readonly ITaxCalculatorService _service;

    public TaxController(ITaxCalculatorService taxCalculatorService)
    {
        _service = taxCalculatorService;
    }

    [HttpPost("calculate")]
    public ActionResult<TaxResultResponse> CreateTax([FromBody] CalculateTaxRequest request)
    {
        Product product = TaxMapper.ToEntity(request);
        TaxResult result = _service.Calculate(product, request.OriginState, request.DestinationState);
        
        return Ok(TaxMapper.ToDto(result));
        
    }
    

}