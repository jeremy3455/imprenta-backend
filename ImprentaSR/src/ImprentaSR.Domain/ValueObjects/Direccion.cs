namespace ImprentaSR.Domain.ValueObjects;

public record Direccion
{
    public string Calle { get; init; }
    public string Ciudad { get; init; }
    public string Estado { get; init; }
    public string CodigoPostal { get; init; }

    public Direccion(string calle, string ciudad, string estado, string codigoPostal)
    {
        Calle = calle;
        Ciudad = ciudad;
        Estado = estado;
        CodigoPostal = codigoPostal;
    }

    public override string ToString() => $"{Calle}, {Ciudad}, {Estado} {CodigoPostal}";
}
