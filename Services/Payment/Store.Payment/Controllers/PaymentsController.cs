using Microsoft.AspNetCore.Mvc;
using Store.Payment.Services;

namespace Store.Payment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentGateway _paymentGateway;

        public PaymentsController(IPaymentGateway paymentGateway)
        {
            _paymentGateway = paymentGateway;
        }

        [HttpPost("Process")]
        public async Task<IActionResult> Process(PaymentProcessRequest request, CancellationToken cancellationToken)
        {
            var result = await _paymentGateway.ProcessPaymentAsync(request, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
