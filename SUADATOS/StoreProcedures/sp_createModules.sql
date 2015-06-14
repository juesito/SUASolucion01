USE [sua]
GO

/****** Object:  StoredProcedure [dbo].[sp_createModules]    Script Date: 13/05/2015 01:22:56 p.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Jesus,Armando>
-- Create date: <Martes 13 de Mayo del 2015>
-- Description:	<SP para crear los modulos que se usaran en el sistema>
CREATE PROCEDURE [dbo].[sp_createModules]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   INSERT INTO Modulos
	( descripcionCorta, descripcionLarga, estatus, fechaCreacion)
	VALUES 
		('Seguridad', 'Administración del Sistema SIAP', 'A', GETDATE())

	INSERT INTO Modulos
	( descripcionCorta, descripcionLarga, estatus, fechaCreacion)
	VALUES 
		('Catalogos', 'Catalogos del Sistema SIAP', 'A', GETDATE())

	INSERT INTO Modulos
	( descripcionCorta, descripcionLarga, estatus, fechaCreacion)
	VALUES 
		('IMSS', 'Manejo del IMSS en SIAP', 'A', GETDATE())

	INSERT INTO Modulos
	( descripcionCorta, descripcionLarga, estatus, fechaCreacion)
	VALUES 
		('Carga', 'Carga de Archivos SUA en SIAP', 'A', GETDATE())
END

GO


