# Órdenes para el agente — Proyecto Imprenta

Este archivo es el punto de entrada central. Cada vez que trabajes en el proyecto, **lee los siguientes archivos según el área que vayas a tocar**:

## 📁 Estructura del proyecto

```
C:\Users\USER\OneDrive\Desktop\Imprenta\
├── database/          ← Scripts SQL (esquema, procedimientos, seeds, cambios)
├── docs/              ← Documentación general del proyecto
├── imprenta-back/     ← API .NET (Clean Architecture + Dapper + SQL Server)
└── Imprenta-Front/    ← Aplicación Angular 21 (standalone components)
```

## 📋 Orden de lectura obligatoria antes de cada intervención

### Si trabajas en la **Base de Datos** (`database/`)
1. `database/README.md` — estructura del esquema, tablas, relaciones y reglas
2. `database/schema/` — scripts DDL (tablas, índices)
3. `database/procedures/` — stored procedures existentes
4. `database/changes/` — scripts de cambios pendientes

### Si trabajas en la **Documentación** (`docs/`)
1. `docs/README.md` — índice de documentación y convenciones
2. `docs/fixes/` — registro de correcciones y features aplicadas

### Si trabajas en el **Backend** (`imprenta-back/`)
1. `imprenta-back/REGLAS.md` — reglas de código, comentarios y commits
2. `database/README.md` — para conocer la estructura real de la BD
3. `ImprentaSR/src/` — código fuente de la API

### Si trabajas en el **Frontend** (`Imprenta-Front/`)
1. `docs/README.md` — para contexto general del proyecto
2. `database/README.md` — para conocer la estructura de datos
3. `src/` — código fuente Angular

## 🚀 Estado actual

- El **frontend** (Angular) se levanta automáticamente con `ng serve`.
- El **backend** (.NET) requiere `dotnet run` desde `imprenta-back/ImprentaSR/`.
- La **base de datos** es SQL Server, sin Entity Framework (solo Dapper + SQL puro).

## ⚠️ Reglas importantes

1. **Nunca inventes** nombres de tablas, columnas, endpoints o servicios que no estén documentados.
2. **Siempre revisa** el archivo correspondiente antes de crear o modificar algo.
3. **Commits claros** en español, describiendo qué se cambió y por qué.
4. **Backend**: sigue Clean Architecture (Application → Domain → Infrastructure → WebAPI).
5. **Frontend**: componentes standalone, servicios con inyección de dependencias.
