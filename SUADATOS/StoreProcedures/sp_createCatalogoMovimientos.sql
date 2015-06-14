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
-- Description:	<SP para crear el catalogo de movimientos que se usaran en el sistema>
CREATE PROCEDURE sp_createCatalogoMovimientos
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO catalogoMovimientos
	 ( tipo, descripcion, fechaCreacion)
	VALUES 
		('01', 'Alta', GETDATE())

	INSERT INTO catalogoMovimientos
	 ( tipo, descripcion, fechaCreacion)
	VALUES 
		('02', 'Baja', GETDATE())

	INSERT INTO catalogoMovimientos
	( tipo, descripcion, fechaCreacion)
	VALUES 
		('07', 'Modificaci�n al salario', GETDATE())

	INSERT INTO catalogoMovimientos
		( tipo, descripcion, fechaCreacion)
	VALUES 
		('08', 'Reingreso', GETDATE())

	INSERT INTO catalogoMovimientos
		( tipo, descripcion, fechaCreacion)
	VALUES 
		('11', 'Ausentismo', GETDATE())

	INSERT INTO catalogoMovimientos
		( tipo, descripcion, fechaCreacion)
	VALUES 
		('12', 'Incapacidad', GETDATE())

	INSERT INTO catalogoMovimientos
		( tipo, descripcion, fechaCreacion)
	VALUES 
		('13', 'Incremento al salrio minimo', GETDATE())

	INSERT INTO catalogoMovimientos
		( tipo, descripcion, fechaCreacion)
	VALUES 
		('15', 'Inicio de cr�dito vivienda', GETDATE())

	INSERT INTO catalogoMovimientos
		( tipo, descripcion, fechaCreacion)
	VALUES 
		('16', 'Suspensi�n de descuentos', GETDATE())

	INSERT INTO catalogoMovimientos
		( tipo, descripcion, fechaCreacion)
	VALUES 
		('17', 'Reinicio de descuento', GETDATE())

	INSERT INTO catalogoMovimientos
		( tipo, descripcion, fechaCreacion)
	VALUES 
		('18', 'Mod. Tipo descuento', GETDATE())

	INSERT INTO catalogoMovimientos
		( tipo, descripcion, fechaCreacion)
	VALUES 
		('19', 'Mod. Valor Descuento', GETDATE())

	INSERT INTO catalogoMovimientos
		( tipo, descripcion, fechaCreacion)
	VALUES 
		('20', 'Mod. Num. Cr�dito', GETDATE())
END
GO
