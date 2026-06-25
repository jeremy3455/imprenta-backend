# Reglas del proyecto (Backend - .NET Core)

## Documentación de código

Todo método público, clase, controlador o servicio nuevo (o modificado) debe llevar un comentario explicando su propósito, **antes** de hacer commit.

### Estilo de comentarios (XML Documentation Comments)

Usar el formato estándar de comentarios XML de C#:

```csharp
/// <summary>
/// Obtiene la lista de usuarios activos del sistema, paginada.
/// </summary>
/// <param name="page">Número de página a consultar (inicia en 1).</param>
/// <param name="pageSize">Cantidad de registros por página.</param>
/// <returns>Una lista paginada de usuarios activos.</returns>
public async Task<PagedResult<UserDto>> GetActiveUsersAsync(int page, int pageSize)
{
    // ...
}
```

### Qué debe llevar comentario

- **Controladores** (`*Controller.cs`): documentar cada endpoint (`summary`, `param`, `returns`, y `response` si aplica códigos HTTP relevantes).
- **Servicios** (`*Service.cs`): documentar cada método público con su propósito, parámetros y retorno.
- **Repositorios**: documentar qué consulta o acción realiza cada método.
- **Modelos/DTOs**: comentar propiedades si su nombre no es autoexplicativo.
- **Middlewares, filtros, validadores**: documentar su propósito y en qué punto del pipeline actúan.
- **Métodos privados**: comentar solo si la lógica es compleja o no es evidente.

### Qué NO hacer

- No dejar código sin comentar antes de hacer commit.
- No usar comentarios obvios o redundantes.
- No comentar código muerto como solución — eliminarlo en vez de dejarlo comentado.

## Convenciones generales

- Comentarios en español.
- Seguir las convenciones de nombres de .NET (PascalCase para clases/métodos, camelCase para variables locales).
- Mantener controladores delgados (lógica de negocio en servicios, no en el controller).
- Usar `async/await` correctamente, evitando bloqueos (`.Result`, `.Wait()`).

## Antes de hacer commit / push

1. Verificar que todo método público nuevo o modificado tenga su comentario XML correspondiente.
2. Si encuentras código sin comentar en archivos que estás tocando, agrégalo.
3. Escribir mensajes de commit claros, describiendo qué se cambió y por qué (no solo "fix" o "update").
4. Asegurarse de que el proyecto compile (`dotnet build`) antes de subir cambios.
