-- ============================================================
-- Script: Índices
-- Proyecto: ImprentaDB
-- Nota: ejecutar después de 01_tables.sql
-- ============================================================

USE ImprentaDB;
GO

-- Clientes
CREATE UNIQUE INDEX IX_Clientes_Ruc      ON Clientes(Ruc);
CREATE INDEX        IX_Clientes_Activo   ON Clientes(Activo) WHERE Activo = 1;

-- Usuarios
CREATE INDEX IX_Usuarios_ClienteId  ON Usuarios(ClienteId);
CREATE INDEX IX_Usuarios_Rol        ON Usuarios(Rol);

-- SecuenciasSri
CREATE INDEX IX_SecuenciasSri_ClienteId
    ON SecuenciasSri(ClienteId, TipoDocumentoId);

-- Cotizaciones
CREATE INDEX IX_Cotizaciones_ClienteId_Estado
    ON Cotizaciones(ClienteId, Estado, CreadaEn DESC);

-- Pedidos — los más críticos
CREATE INDEX IX_Pedidos_ClienteId
    ON Pedidos(ClienteId);
CREATE INDEX IX_Pedidos_Estado_Fecha
    ON Pedidos(Estado, FechaPedido DESC);         -- filtro principal del dashboard
CREATE INDEX IX_Pedidos_SecuenciaSriId
    ON Pedidos(SecuenciaSriId);
CREATE INDEX IX_Pedidos_FechaPedido
    ON Pedidos(FechaPedido DESC);                 -- reportes por rango de fecha

-- Índice cubriente para listado de pedidos del cliente (evita lookup a tabla)
CREATE INDEX IX_Pedidos_Cliente_Cobertura
    ON Pedidos(ClienteId, Estado, FechaPedido DESC)
    INCLUDE (NumPedido, Cantidad, Total);

-- DocumentosGenerados
CREATE INDEX IX_Docs_PedidoId       ON DocumentosGenerados(PedidoId);
CREATE INDEX IX_Docs_EnviadoSri     ON DocumentosGenerados(EnviadoSri) WHERE EnviadoSri = 0;
CREATE INDEX IX_Docs_ClaveAcceso    ON DocumentosGenerados(ClaveAcceso);

-- LogsSri
CREATE INDEX IX_LogsSri_ClienteId_Fecha
    ON LogsSri(ClienteId, CreadoEn DESC);

-- OtpVerificacion
CREATE INDEX IX_Otp_Correo_Expira
    ON OtpVerificacion(Correo, ExpiraEn)
    WHERE Usado = 0;

-- RefreshTokens
CREATE INDEX IX_RefreshTokens_Token   ON RefreshTokens(Token);
CREATE INDEX IX_RefreshTokens_Usuario ON RefreshTokens(UsuarioId);
GO
