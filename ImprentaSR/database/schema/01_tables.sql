-- ============================================================
-- Script: Creación de tablas
-- Proyecto: ImprentaDB
-- Orden: tablas sin dependencias primero, luego las que
--        referencian a otras (respeta las foreign keys)
-- ============================================================

USE ImprentaDB;
GO

-- ------------------------------------------------------------
-- TiposDocumento
-- Catálogo de tipos de documento (Factura, Nota de venta, etc.)
-- ------------------------------------------------------------
CREATE TABLE TiposDocumento (
    ID               INT           IDENTITY(1,1) PRIMARY KEY,
    Codigo           VARCHAR(10)   NOT NULL UNIQUE,
    Nombre           NVARCHAR(100) NOT NULL,
    FormatoPlantilla NVARCHAR(200) NULL,
    Activo           BIT           NOT NULL DEFAULT 1,
    CreadoEn         DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ------------------------------------------------------------
-- Clientes
-- Empresas o personas que solicitan pedidos a la imprenta
-- ------------------------------------------------------------
CREATE TABLE Clientes (
    Id          INT            IDENTITY(1,1) PRIMARY KEY,
    Ruc         VARCHAR(13)    NOT NULL UNIQUE,
    RazonSocial NVARCHAR(300)  NOT NULL,
    Direccion   NVARCHAR(500)  NULL,
    Telefono    VARCHAR(20)    NULL,
    Email       VARCHAR(200)   NULL,
    Activo      BIT            NOT NULL DEFAULT 1,
    CreadoEn    DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ------------------------------------------------------------
-- Usuarios
-- Usuarios del sistema (internos y clientes con acceso al portal)
-- ------------------------------------------------------------
CREATE TABLE Usuarios (
    Id                     INT           IDENTITY(1,1) PRIMARY KEY,
    ClienteId              INT           NULL REFERENCES Clientes(Id),  -- NULL = usuario interno
    Nombre                 NVARCHAR(200) NOT NULL,
    Email                  VARCHAR(200)  NOT NULL UNIQUE,
    PasswordHash           NVARCHAR(500) NOT NULL,
    Rol                    VARCHAR(20)   NOT NULL DEFAULT 'Cliente',    -- Admin, Operador, Cliente
    Activo                 BIT           NOT NULL DEFAULT 1,
    CreadoEn               DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
    -- Columnas de flujo de autenticación
    MustChangePassword     BIT           NOT NULL DEFAULT 0,
    EmailVerificado        BIT           NOT NULL DEFAULT 0,
    UltimoLogin            DATETIME2     NULL,
    LoginIntentosFallidos  INT           NOT NULL DEFAULT 0,
    BloqueadoHasta         DATETIME2     NULL
);
GO

-- ------------------------------------------------------------
-- SecuenciasSri
-- Control de secuenciales autorizados por el SRI por cliente/tipo
-- ------------------------------------------------------------
CREATE TABLE SecuenciasSri (
    Id                  INT          IDENTITY(1,1) PRIMARY KEY,
    ClienteId           INT          NOT NULL REFERENCES Clientes(Id),
    TipoDocumentoId     INT          NOT NULL REFERENCES TiposDocumento(Id),
    Establecimiento     VARCHAR(3)   NOT NULL,   -- ej: 001
    PuntoEmision        VARCHAR(3)   NOT NULL,   -- ej: 001
    SecuenciaActual     INT          NOT NULL DEFAULT 0,
    SecuenciaMax        INT          NOT NULL DEFAULT 999999999,
    NumAutorizacion     VARCHAR(50)  NOT NULL,
    FechaVencimiento    DATE         NOT NULL,
    Activo              BIT          NOT NULL DEFAULT 1,
    CONSTRAINT UQ_Secuencia UNIQUE (ClienteId, TipoDocumentoId, Establecimiento, PuntoEmision)
);
GO

-- ------------------------------------------------------------
-- Cotizaciones
-- Cotizaciones previas a la generación de un pedido
-- ------------------------------------------------------------
CREATE TABLE Cotizaciones (
    Id              INT             IDENTITY(1,1) PRIMARY KEY,
    ClienteId       INT             NOT NULL REFERENCES Clientes(Id),
    TipoDocumentoId INT             NOT NULL REFERENCES TiposDocumento(Id),
    Cantidad        INT             NOT NULL,
    PrecioUnitario  DECIMAL(10,4)   NOT NULL,
    Total           DECIMAL(12,2)   NOT NULL,
    Estado          VARCHAR(20)     NOT NULL DEFAULT 'Pendiente', -- Pendiente, Aprobada, Rechazada
    Observaciones   NVARCHAR(500)   NULL,
    CreadaEn        DATETIME2       NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ------------------------------------------------------------
-- Pedidos
-- Pedidos de impresión solicitados por los clientes
-- ------------------------------------------------------------
CREATE TABLE Pedidos (
    Id              INT           IDENTITY(1,1) PRIMARY KEY,
    ClienteId       INT           NOT NULL REFERENCES Clientes(Id),
    TipoDocumentoId INT           NOT NULL REFERENCES TiposDocumento(Id),
    SecuenciaSriId  INT           NOT NULL REFERENCES SecuenciasSri(Id),
    CotizacionId    INT           NULL     REFERENCES Cotizaciones(Id),
    NumPedido       VARCHAR(20)   NOT NULL UNIQUE,   -- generado internamente
    Cantidad        INT           NOT NULL,
    Estado          VARCHAR(20)   NOT NULL DEFAULT 'Recibido',
    -- Estados: Recibido, EnProceso, Impreso, Listo, Entregado, Anulado
    PrecioUnitario  DECIMAL(10,4) NOT NULL,
    Total           DECIMAL(12,2) NOT NULL,
    Observaciones   NVARCHAR(500) NULL,
    FechaPedido     DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
    FechaEntrega    DATETIME2     NULL,
    CreadoPorId     INT           NULL REFERENCES Usuarios(Id)
);
GO

-- ------------------------------------------------------------
-- DocumentosGenerados
-- Documentos electrónicos generados por cada pedido (PDF/XML SRI)
-- ------------------------------------------------------------
CREATE TABLE DocumentosGenerados (
    Id             INT           IDENTITY(1,1) PRIMARY KEY,
    PedidoId       INT           NOT NULL REFERENCES Pedidos(Id),
    NumSecuencial  VARCHAR(9)    NOT NULL,   -- ej: 000000001
    NumCompleto    VARCHAR(17)   NOT NULL,   -- ej: 001-001-000000001
    RutaPdf        NVARCHAR(500) NULL,
    XmlSri         NVARCHAR(MAX) NULL,       -- XML firmado para comprobantes electrónicos
    EnviadoSri     BIT           NOT NULL DEFAULT 0,
    EstadoSri      VARCHAR(20)   NULL,       -- Autorizado, Rechazado, Pendiente
    ClaveAcceso    VARCHAR(49)   NULL,       -- clave de acceso SRI (49 dígitos)
    GeneradoEn     DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ------------------------------------------------------------
-- LogsSri
-- Bitácora de comunicación con los servicios del SRI
-- ------------------------------------------------------------
CREATE TABLE LogsSri (
    Id          INT            IDENTITY(1,1) PRIMARY KEY,
    ClienteId   INT            NULL REFERENCES Clientes(Id),
    PedidoId    INT            NULL REFERENCES Pedidos(Id),
    Tipo        VARCHAR(30)    NOT NULL,   -- ConsultaRuc, AutorizacionDoc, Error
    Request     NVARCHAR(MAX)  NULL,
    Response    NVARCHAR(MAX)  NULL,
    Exitoso     BIT            NOT NULL DEFAULT 0,
    CreadoEn    DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ------------------------------------------------------------
-- OtpVerificacion
-- Códigos OTP de un solo uso para verificación (login, email, etc.)
-- ------------------------------------------------------------
CREATE TABLE OtpVerificacion (
    Id          INT           IDENTITY(1,1) PRIMARY KEY,
    Correo      VARCHAR(200)  NOT NULL,
    OtpHash     NVARCHAR(500) NOT NULL,       -- guardamos hash, nunca el OTP plano
    Intentos    INT           NOT NULL DEFAULT 0,
    Usado       BIT           NOT NULL DEFAULT 0,
    ExpiraEn    DATETIME2     NOT NULL,
    CreadoEn    DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ------------------------------------------------------------
-- RefreshTokens
-- Tokens de refresco para la sesión JWT de los usuarios
-- ------------------------------------------------------------
CREATE TABLE RefreshTokens (
    Id          INT           IDENTITY(1,1) PRIMARY KEY,
    UsuarioId   INT           NOT NULL REFERENCES Usuarios(Id),
    Token       NVARCHAR(200) NOT NULL UNIQUE,
    ExpiraEn    DATETIME2     NOT NULL,
    Revocado    BIT           NOT NULL DEFAULT 0,
    CreadoEn    DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);
GO
