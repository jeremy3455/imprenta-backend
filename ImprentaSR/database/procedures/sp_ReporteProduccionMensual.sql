-- ============================================================
-- SP: sp_ReporteProduccionMensual
-- Propósito: Genera un reporte agregado de pedidos por cliente
--            y tipo de documento para un mes/año específico.
--            Permite filtrar por un cliente puntual o ver todos.
-- ============================================================

USE ImprentaDB;
GO

CREATE OR ALTER PROCEDURE sp_ReporteProduccionMensual
    @Anio   INT,
    @Mes    INT,
    @ClienteId INT = NULL   -- NULL = todos los clientes
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.RazonSocial,
        c.Ruc,
        td.Nombre                           AS TipoDocumento,
        COUNT(p.Id)                         AS TotalPedidos,
        SUM(p.Cantidad)                     AS TotalDocumentos,
        SUM(p.Total)                        AS IngresoTotal,
        SUM(CASE WHEN p.Estado = 'Entregado' THEN 1 ELSE 0 END) AS Entregados,
        SUM(CASE WHEN p.Estado = 'Anulado'   THEN 1 ELSE 0 END) AS Anulados
    FROM Pedidos p
    INNER JOIN Clientes        c  ON c.Id  = p.ClienteId
    INNER JOIN TiposDocumento  td ON td.Id = p.TipoDocumentoId
    WHERE YEAR(p.FechaPedido)  = @Anio
      AND MONTH(p.FechaPedido) = @Mes
      AND (@ClienteId IS NULL OR p.ClienteId = @ClienteId)
    GROUP BY c.RazonSocial, c.Ruc, td.Nombre
    ORDER BY IngresoTotal DESC;
END;
GO
