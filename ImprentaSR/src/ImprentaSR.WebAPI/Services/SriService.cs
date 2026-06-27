using System.Text.Json;
using ImprentaSR.Application.DTOs;

namespace ImprentaSR.WebAPI.Services;

public class SriService
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SriService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SriRespuestaDto> ConsultarContribuyenteAsync(string numero)
    {
        var ruc13 = numero.Length == 10 ? numero + "001" : numero;

        try
        {
            var lista = await _httpClient.GetFromJsonAsync<List<SriResponse>>(
                $"ConsolidadoContribuyente/obtenerPorNumerosRuc?ruc={ruc13}", JsonOptions);

            var item = lista?.FirstOrDefault();

            if (item is null)
                return new SriRespuestaDto { NumeroCedulaRuc = numero, Encontrado = false };

            return new SriRespuestaDto
            {
                NumeroCedulaRuc = item.numeroRuc ?? numero,
                RazonSocial = item.razonSocial ?? "S/N",
                EstadoContribuyente = item.estadoContribuyenteRuc,
                TipoContribuyente = item.tipoContribuyente?.ToUpper(),
                Encontrado = true
            };
        }
        catch (HttpRequestException)
        {
            throw new InvalidOperationException(
                "El servicio del SRI no está disponible, ingresa los datos manualmente.");
        }
        catch (TaskCanceledException)
        {
            throw new InvalidOperationException(
                "El servicio del SRI no respondió a tiempo, ingresa los datos manualmente.");
        }
    }

    private record SriResponse
    {
        public string? numeroRuc { get; init; }
        public string? razonSocial { get; init; }
        public string? estadoContribuyenteRuc { get; init; }
        public string? tipoContribuyente { get; init; }
    }
}
