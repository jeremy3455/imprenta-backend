-- ============================================================
-- SP: sp_MarcarEnviadoSri
-- Propósito: Actualiza el estado de envío al SRI de los
--            documentos generados de un pedido, y refleja el
--            avance correspondiente en el estado del pedido.
-- ============================================================

USE ImprentaDB;
GO

CREATE OR ALTER PROCEDURE sp_MarcarEnviadoSri
    @PedidoId    INT,
    @EstadoSri   VARCHAR(20),
    @ClaveAcceso VARCHAR(49)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE DocumentosGenerados
    SET EnviadoSri  = 1,
        EstadoSri   = @EstadoSri,
        ClaveAcceso = @ClaveAcceso
    WHERE PedidoId = @PedidoId;

    UPDATE Pedidos
    SET Estado = CASE WHEN @EstadoSri = 'Autorizado' THEN 'Listo' ELSE 'EnProceso' END
    WHERE Id = @PedidoId;
END;
GO
