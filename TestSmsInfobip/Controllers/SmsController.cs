using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
namespace TestSmsInfobip.Controllers;

public class SmsController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public SmsController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendSms([FromBody] SmsRequest smsRequest)
    {
        // Configurar la URL base de Infobip
        _httpClient.BaseAddress = new Uri("https://rppq2m.api.infobip.com");

        // Agregar encabezados de autenticación
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("App", "1a9a4ba8f0a9001d1f2ca2d1b3d8a79e-73cc8e29-328b-4783-a77d-69070fd9defd");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Construir el cuerpo de la solicitud
        var body = new
        {
            messages = new[]
            {
                    new
                    {
                        destinations = new[]
                        {
                            new { to = smsRequest.To }
                        },
                        from = smsRequest.From,
                        text = smsRequest.Text
                    }
                }
        };

        // Serializar el cuerpo en JSON
        var jsonBody = JsonConvert.SerializeObject(body);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        // Hacer la solicitud POST a Infobip
        var response = await _httpClient.PostAsync("/sms/2/text/advanced", content);

        // Leer la respuesta
        var responseContent = await response.Content.ReadAsStringAsync();

        // Devolver la respuesta en formato JSON
        if (response.IsSuccessStatusCode)
        {
            return Ok(responseContent);
        }

        return StatusCode((int)response.StatusCode, responseContent);
    }
}

public class SmsRequest
{
    public string? To { get; set; }
    public string? From { get; set; }
    public string? Text { get; set; }
}
