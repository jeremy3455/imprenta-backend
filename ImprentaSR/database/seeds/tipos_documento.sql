-- ============================================================
-- Seed: Tipos de documento iniciales
-- Proyecto: ImprentaDB
-- Nota: ejecutar después de crear las tablas (01_tables.sql)
-- ============================================================

USE ImprentaDB;
GO

INSERT INTO TiposDocumento (Codigo, Nombre, FormatoPlantilla) VALUES
('01', 'Factura',               'plantillas/factura.html'),
('02', 'Nota de venta',         'plantillas/nota_venta.html'),
('04', 'Nota de crédito',       'plantillas/nota_credito.html'),
('05', 'Nota de débito',        'plantillas/nota_debito.html'),
('06', 'Guía de remisión',      'plantillas/guia_remision.html'),
('07', 'Comprobante retención', 'plantillas/retencion.html');
GO
