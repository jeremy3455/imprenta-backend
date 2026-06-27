-- ============================================================
-- Migration: Solicitudes de Clientes y Notificaciones
-- Descripción: Agrega tablas para que clientes soliciten
-- productos y el admin reciba notificaciones.
-- ============================================================

-- ── Solicitudes ──────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Solicitudes')
BEGIN
    CREATE TABLE Solicitudes (
        Id             INT            IDENTITY(1,1) PRIMARY KEY,
        ClienteId      INT            NOT NULL REFERENCES Clientes(Id),
        Estado         VARCHAR(20)    NOT NULL DEFAULT 'Pendiente',
        Observacion    NVARCHAR(500)  NULL,
        FechaSolicitud DATETIME2      NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT CK_Solicitudes_Estado
            CHECK (Estado IN ('Pendiente', 'Aprobada', 'Rechazada'))
    );

    CREATE INDEX IX_Solicitudes_ClienteId ON Solicitudes(ClienteId);
    CREATE INDEX IX_Solicitudes_Estado    ON Solicitudes(Estado);
END;

-- ── DetalleSolicitud ────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DetalleSolicitud')
BEGIN
    CREATE TABLE DetalleSolicitud (
        Id          INT           IDENTITY(1,1) PRIMARY KEY,
        SolicitudId INT           NOT NULL REFERENCES Solicitudes(Id),
        ProductoId  INT           NOT NULL REFERENCES Productos(Id),
        Cantidad    INT           NOT NULL,
        Observacion NVARCHAR(500) NULL
    );

    CREATE INDEX IX_DetalleSolicitud_SolicitudId ON DetalleSolicitud(SolicitudId);
END;

-- ── Notificaciones ──────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Notificaciones')
BEGIN
    CREATE TABLE Notificaciones (
        Id          INT            IDENTITY(1,1) PRIMARY KEY,
        UsuarioId   INT            NOT NULL REFERENCES Usuarios(Id),
        Mensaje     NVARCHAR(500)  NOT NULL,
        Tipo        VARCHAR(50)    NOT NULL DEFAULT 'General',
        ReferenciaId INT           NULL,
        Leida       BIT            NOT NULL DEFAULT 0,
        Fecha       DATETIME2      NOT NULL DEFAULT GETUTCDATE()
    );

    CREATE INDEX IX_Notificaciones_UsuarioId ON Notificaciones(UsuarioId);
    CREATE INDEX IX_Notificaciones_Leida     ON Notificaciones(Leida);
END;
