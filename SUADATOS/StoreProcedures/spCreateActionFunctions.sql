-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Jesus,Armando>
-- Create date: <Martes 14 de Mayo del 2015>
-- Description:	<SP para crear las funciones de acción que se usaran en el sistema por modulo>
CREATE PROCEDURE spCreateActionFunctions 
	@usuarioId   INT
AS
BEGIN
	DECLARE @moduloId int
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Buscamos el modulo de seguridad para insertar las funciones correspondientes
	SET @moduloId = (SELECT id FROM Modulos WHERE descripcionCorta = 'IMSS')

	--Patrones
	INSERT INTO Funcions
	( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Patrones', 'Consultar detalle', 'Index', 'Patrones', 'A', @moduloId, GETDATE(), @usuarioId, 'A')

	INSERT INTO Funcions
	( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Patrones', 'Descargar excel', 'Index', 'Patrones', 'A', @moduloId, GETDATE(), @usuarioId, 'A')


    --Asegurados
	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Asegurados', 'Buscar', 'Index', 'Asegurados', 'A', @moduloId, GETDATE(), @usuarioId, 'A')

    INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Asegurados', 'Ver columna creacion', 'Index', 'Asegurados', 'A', @moduloId, GETDATE(), @usuarioId, 'A')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Asegurados', 'Ver columna modificacion', 'Index', 'Asegurados', 'A', @moduloId, GETDATE(), @usuarioId, 'A')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Asegurados', 'Subir acuse', 'Index', 'Asegurados', 'A', @moduloId, GETDATE(), @usuarioId, 'A')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Asegurados', 'Eliminar acuses', 'Index', 'Asegurados', 'A', @moduloId, GETDATE(), @usuarioId, 'A')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Asegurados', 'Eliminar movimiento', 'Index', 'Asegurados', 'A', @moduloId, GETDATE(), @usuarioId, 'A')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Asegurados', 'Descargar excel', 'Index', 'Asegurados', 'A', @moduloId, GETDATE(), @usuarioId, 'A')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Asegurados', 'Consultar detalle', 'Index', 'Asegurados', 'A', @moduloId, GETDATE(), @usuarioId, 'A')


    --Acreditados
	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Acreditados', 'Buscar', 'Index', 'Acreditados', 'A', @moduloId, GETDATE(), @usuarioId, 'A')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Acreditados', 'Subir acuse', 'Index', 'Acreditados', 'A', @moduloId, GETDATE(), @usuarioId, 'A')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Acreditados', 'Eliminar acuses', 'Index', 'Acreditados', 'A', @moduloId, GETDATE(), @usuarioId, 'A')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Acreditados', 'Eliminar movimiento', 'Index', 'Acreditados', 'A', @moduloId, GETDATE(), @usuarioId, 'A')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Acreditados', 'Descargar excel', 'Index', 'Acreditados', 'A', @moduloId, GETDATE(), @usuarioId, 'A')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Acreditados', 'Consultar detalle', 'Index', 'Acreditados', 'A', @moduloId, GETDATE(), @usuarioId, 'A')
	
END
GO



