namespace ImprentaSR.Domain.ValueObjects;

/// <summary>
/// Value Object inmutable que representa una dirección física.
/// Se utiliza como propiedad incorporada (owned entity) en EF Core.
/// </summary>
public record Direccion
{
    /// <summary>Calle y número del domicilio.</summary>
    public string Calle { get; init; }

    /// <summary>Ciudad o localidad.</summary>
    public string Ciudad { get; init; }

    /// <summary>Estado, provincia o departamento.</summary>
    public string Estado { get; init; }

    /// <summary>Código postal.</summary>
    public string CodigoPostal { get; init; }

    /// <summary>
    /// Crea una nueva dirección.
    /// </summary>
    /// <param name="calle">Calle y número.</param>
    /// <param name="ciudad">Ciudad.</param>
    /// <param name="estado">Estado/provincia.</param>
    /// <param name="codigoPostal">Código postal.</param>
    public Direccion(string calle, string ciudad, string estado, string codigoPostal)
    {
        Calle = calle;
        Ciudad = ciudad;
        Estado = estado;
        CodigoPostal = codigoPostal;
    }

    /// <summary>Devuelve la dirección en formato legible: "Calle, Ciudad, Estado CP".</summary>
    public override string ToString() => $"{Calle}, {Ciudad}, {Estado} {CodigoPostal}";
}
