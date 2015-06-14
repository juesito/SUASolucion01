USE [sua]
GO

/****** Object:  StoredProcedure [dbo].[sp_createRoles]    Script Date: 13/05/2015 01:21:37 p.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Jesus,Armando>
-- Create date: <Martes 13 de Mayo del 2015>
-- Description:	<SP para crear los modulos que se usaran en el sistema>
CREATE PROCEDURE [dbo].[sp_createRoles]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO Roles
	( descripcion, estatus, fechaCreacion)
	VALUES 
		('Administrador', 'A', GETDATE())

    INSERT INTO Roles
	( descripcion, estatus, fechaCreacion)
	VALUES 
		('Ejecutivo', 'A', GETDATE())

	INSERT INTO Roles
	( descripcion, estatus, fechaCreacion)
	VALUES 
		('Cliente', 'A', GETDATE())
	
END

GO


