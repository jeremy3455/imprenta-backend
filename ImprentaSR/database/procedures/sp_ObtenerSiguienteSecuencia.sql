-- ============================================================
-- SP: sp_ObtenerSiguienteSecuencia
-- Propósito: Obtiene y reserva atómicamente el siguiente número
--            de secuencia autorizado por el SRI para un cliente
--            y tipo de documento, evitando duplicados por
--            condiciones de carrera (usa UPDLOCK/ROWLOCK).
-- ============================================================

USE ImprentaDB;
GO

CREATE OR ALTER PROCEDURE sp_ObtenerSiguienteSecuencia
    @ClienteId      INT,
    @TipoDocId      INT,
    @Establecimiento VARCHAR(3),
    @PuntoEmision   VARCHAR(3),
    @NumCompleto    VARCHAR(17) OUTPUT,
    @Exitoso        BIT         OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Exitoso = 0;

    DECLARE @SecuenciaId    INT;
    DECLARE @NuevaSecuencia INT;
    DECLARE @NumAuth        VARCHAR(50);
    DECLARE @FechaVenc      DATE;

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT
            @SecuenciaId = Id,
            @NuevaSecuencia = SecuenciaActual + 1,
            @NumAuth = NumAutorizacion,
            @FechaVenc = FechaVencimiento
        FROM SecuenciasSri WITH (UPDLOCK, ROWLOCK)
        WHERE ClienteId       = @ClienteId
          AND TipoDocumentoId = @TipoDocId
          AND Establecimiento = @Establecimiento
          AND PuntoEmision    = @PuntoEmision
          AND Activo          = 1;

        IF @SecuenciaId IS NULL
        BEGIN
            ROLLBACK;
            RAISERROR('No existe secuencia activa para este cliente y tipo de documento.', 16, 1);
            RETURN;
        END

        IF @FechaVenc < CAST(GETDATE() AS DATE)
        BEGIN
            ROLLBACK;
            RAISERROR('La autorización SRI está vencida.', 16, 1);
            RETURN;
        END

        UPDATE SecuenciasSri
        SET SecuenciaActual = @NuevaSecuencia
        WHERE Id = @SecuenciaId;

        SET @NumCompleto = @Establecimiento + '-' + @PuntoEmision + '-'
                         + RIGHT('000000000' + CAST(@NuevaSecuencia AS VARCHAR), 9);
        SET @Exitoso = 1;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END;
GO
