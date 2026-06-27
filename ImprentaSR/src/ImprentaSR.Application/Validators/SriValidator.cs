using System.Text.RegularExpressions;
using ImprentaSR.Application.Interfaces;

namespace ImprentaSR.Application.Validators;

public class SriValidator : ISriValidator
{
    public string? ValidarNumero(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            return "Ingresa un número de cédula o RUC.";

        numero = Regex.Replace(numero, @"\D", "");

        if (numero.Length == 10)
            return ValidarCedula(numero) ? null
                : "Cédula inválida, verifica el dígito verificador.";

        if (numero.Length == 13)
            return ValidarRuc(numero) ? null
                : "RUC inválido.";

        return "El número debe tener 10 dígitos (cédula) o 13 dígitos (RUC).";
    }

    private static bool ValidarCedula(string cedula)
    {
        if (cedula.Length != 10 || !cedula.All(char.IsDigit))
            return false;

        var provincia = int.Parse(cedula[..2]);
        if (provincia < 1 || provincia > 24)
            return false;

        var digitoVerificador = int.Parse(cedula[9].ToString());
        var suma = 0;

        for (var i = 0; i < 9; i++)
        {
            var digito = int.Parse(cedula[i].ToString());
            if (i % 2 == 0)
            {
                digito *= 2;
                if (digito > 9) digito -= 9;
            }
            suma += digito;
        }

        var residuo = suma % 10;
        var calculado = residuo == 0 ? 0 : 10 - residuo;
        return calculado == digitoVerificador;
    }

    private static bool ValidarRuc(string ruc)
    {
        if (ruc.Length != 13 || !ruc.All(char.IsDigit))
            return false;

        var provincia = int.Parse(ruc[..2]);
        if (provincia < 1 || provincia > 24)
            return false;

        var ultimosTres = ruc[10..];
        if (ultimosTres != "001")
            return false;

        var tercerDigito = int.Parse(ruc[2].ToString());

        if (tercerDigito == 6)
            return ValidarRucModulo11(ruc, 9);
        if (tercerDigito == 9)
            return ValidarRucModulo11(ruc, 8);
        if (tercerDigito >= 0 && tercerDigito <= 5)
            return ValidarCedula(ruc[..10]);

        return false;
    }

    private static bool ValidarRucModulo11(string numero, int longitud)
    {
        var coeficientes = new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        if (longitud >= coeficientes.Length) return false;

        var digitoVerificador = int.Parse(numero[longitud].ToString());
        var suma = 0;

        for (var i = 0; i < longitud; i++)
            suma += int.Parse(numero[i].ToString()) * coeficientes[i];

        var residuo = suma % 11;
        var calculado = 11 - residuo;
        if (calculado == 11) calculado = 0;
        if (calculado == 10) calculado = 0;

        return calculado == digitoVerificador;
    }
}
