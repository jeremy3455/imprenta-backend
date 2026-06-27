-- ============================================================
-- Migration: Agregar FormaPago, PedidoId y MontoTotal a Solicitudes
-- ============================================================

-- Agregar columnas si no existen (para bases existentes)
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Solicitudes' AND COLUMN_NAME = 'FormaPago')
BEGIN
    ALTER TABLE Solicitudes ADD FormaPago VARCHAR(20) NOT NULL DEFAULT 'EFECTIVO';
    ALTER TABLE Solicitudes ADD CONSTRAINT CK_Solicitudes_FormaPago CHECK (FormaPago IN ('EFECTIVO', 'TRANSFERENCIA'));
END;

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Solicitudes' AND COLUMN_NAME = 'PedidoId')
BEGIN
    ALTER TABLE Solicitudes ADD PedidoId INT NULL REFERENCES Pedidos(Id);
END;

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Solicitudes' AND COLUMN_NAME = 'MontoTotal')
BEGIN
    ALTER TABLE Solicitudes ADD MontoTotal DECIMAL(18,2) NULL;
END;
