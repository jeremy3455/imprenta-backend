# Base de datos — ImprentaDB

Este documento es la referencia oficial del esquema de base de datos para el proyecto. Antes de crear o modificar cualquier código que interactúe con la base, **revisa esta carpeta** para conocer la estructura real.

No se usa Entity Framework. Toda la persistencia se maneja con SQL puro: tablas, índices y stored procedures.

## Estructura de esta carpeta

```
database/
├── schema/
│   ├── 00_create_database.sql   ← creación de la base ImprentaDB
│   ├── 01_tables.sql            ← las 10 tablas, en orden de dependencias
│   └── 02_indexes.sql           ← todos los índices
├── procedures/
│   ├── sp_ObtenerSiguienteSecuencia.sql
│   ├── sp_ReporteProduccionMensual.sql
│   └── sp_MarcarEnviadoSri.sql
├── seeds/
│   └── tipos_documento.sql      ← datos iniciales de catálogo
└── changes/                     ← aquí van los nuevos scripts que se generen
```

## Orden de ejecución para crear la base desde cero

1. `schema/00_create_database.sql`
2. `schema/01_tables.sql`
3. `schema/02_indexes.sql`
4. `seeds/tipos_documento.sql`
5. Todos los archivos en `procedures/`

## Resumen de tablas

| Tabla | Propósito |
|---|---|
| `TiposDocumento` | Catálogo de tipos de documento (Factura, Nota de venta, etc.) |
| `Clientes` | Empresas/personas que solicitan pedidos |
| `Usuarios` | Usuarios del sistema (internos y clientes), incluye flujo de auth (login, OTP, bloqueo) |
| `SecuenciasSri` | Control de secuenciales autorizados por el SRI |
| `Cotizaciones` | Cotizaciones previas a un pedido |
| `Pedidos` | Pedidos de impresión solicitados |
| `DocumentosGenerados` | Documentos electrónicos (PDF/XML) generados por pedido |
| `LogsSri` | Bitácora de comunicación con el SRI |
| `OtpVerificacion` | Códigos OTP de un solo uso |
| `RefreshTokens` | Tokens de refresco de sesión JWT |

## Relaciones clave

- `Usuarios.ClienteId → Clientes.Id` (NULL = usuario interno, no asociado a un cliente)
- `Pedidos.ClienteId → Clientes.Id`
- `Pedidos.TipoDocumentoId → TiposDocumento.Id`
- `Pedidos.SecuenciaSriId → SecuenciasSri.Id`
- `Pedidos.CotizacionId → Cotizaciones.Id` (opcional)
- `Pedidos.CreadoPorId → Usuarios.Id` (opcional)
- `DocumentosGenerados.PedidoId → Pedidos.Id`
- `RefreshTokens.UsuarioId → Usuarios.Id`
- `SecuenciasSri.ClienteId → Clientes.Id`
- `SecuenciasSri.TipoDocumentoId → TiposDocumento.Id`

## Estados usados en el sistema

- **Pedidos.Estado**: `Recibido`, `EnProceso`, `Impreso`, `Listo`, `Entregado`, `Anulado`
- **Cotizaciones.Estado**: `Pendiente`, `Aprobada`, `Rechazada`
- **DocumentosGenerados.EstadoSri**: `Autorizado`, `Rechazado`, `Pendiente`
- **Usuarios.Rol**: `Admin`, `Operador`, `Cliente`

## Reglas para el agente

1. **Nunca inventes** nombres de tablas, columnas o stored procedures que no estén en esta carpeta. Si no existe, hay que crearlo.
2. Para crear o modificar tablas, índices o SPs, **genera un nuevo script** en `database/changes/` con el formato de nombre `YYYY-MM-DD_descripcion_breve.sql`. No ejecutes el script directamente — la persona del equipo lo revisa y lo corre manualmente.
3. Cada script de cambio debe llevar un comentario arriba explicando qué hace y por qué.
4. Sigue el mismo estilo de los SPs existentes: `SET NOCOUNT ON`, manejo de transacciones con `TRY/CATCH` para operaciones críticas, nombres de parámetros con prefijo `@`.
5. Después de que un script de `changes/` se ejecute y se confirme en la base real, actualiza el archivo correspondiente en `schema/` o `procedures/` para que sigan reflejando el estado actual.
