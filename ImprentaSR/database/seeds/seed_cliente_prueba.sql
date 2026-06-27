-- Seed: Cliente de prueba y su usuario
-- Ejecutar solo si no existe un cliente con este RUC

IF NOT EXISTS (SELECT 1 FROM Clientes WHERE NumeroCedulaRuc = '0999999999')
BEGIN
    INSERT INTO Clientes (NumeroCedulaRuc, RazonSocial, Direccion, Email, Telefono, TipoContribuyente, Estado)
    VALUES ('0999999999', 'Cliente de Prueba', 'Av. Principal 123', 'cliente@test.com', '0999999999', 'PERSONA_NATURAL', 1);
END;

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'cliente@test.com')
BEGIN
    DECLARE @ClienteId INT = (SELECT Id FROM Clientes WHERE NumeroCedulaRuc = '0999999999');
    INSERT INTO Usuarios (ClienteId, Nombre, Email, PasswordHash, Rol, EmailVerificado)
    VALUES (@ClienteId, 'Cliente Prueba', 'cliente@test.com', '$2a$11$K4YfGqJ1eQY6fL8x9WzN0O3pR5sT7vB9dF1hJ3kM5nP7rS9tV2wX', 'Cliente', 1);
END;
