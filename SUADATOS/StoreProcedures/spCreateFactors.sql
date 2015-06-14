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
CREATE PROCEDURE spCreateFactors
	@usuarioId   INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(1       , 6 , 0.25, 1.5   , 365    , 
		0.0041   , 15, 365 , 0.0411, 1.0452 ,
		GETDATE(), @usuarioId)

	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(2       , 8 , 0.25, 2.0   , 365    , 
		0.0055   , 15, 365 , 0.0411, 1.0466 ,
		GETDATE(), @usuarioId)

	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(3       , 10 , 0.25, 2.5   , 365    , 
		0.0068   , 15, 365 , 0.0411, 1.0479 ,
		GETDATE(), @usuarioId)

	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(4       , 12 , 0.25, 3.0   , 365    , 
		0.0082   , 15, 365 , 0.0411, 1.0493 ,
		GETDATE(), @usuarioId)

	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(5       , 14 , 0.25, 3.5   , 365    , 
		0.0096   , 15, 365 , 0.0411, 1.0507 ,
		GETDATE(), @usuarioId)

	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(6       , 14 , 0.25, 3.5   , 365    , 
		0.0096   , 15, 365 , 0.0411, 1.0507 ,
		GETDATE(), @usuarioId)

	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(7       , 14 , 0.25, 3.5   , 365    , 
		0.0096   , 15, 365 , 0.0411, 1.0507 ,
		GETDATE(), @usuarioId)

	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(8       , 14 , 0.25, 3.5   , 365    , 
		0.0096   , 15, 365 , 0.0411, 1.0507 ,
		GETDATE(), @usuarioId)

	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(9       , 14 , 0.25, 3.5   , 365    , 
		0.0096   , 15, 365 , 0.0411, 1.0507 ,
		GETDATE(), @usuarioId)
	
	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(10       , 16 , 0.25, 4.0   , 365    , 
		0.0110   , 15, 365 , 0.0411, 1.0521 ,
		GETDATE(), @usuarioId)
	
	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(11       , 16 , 0.25, 4.0   , 365    , 
		0.0110   , 15, 365 , 0.0411, 1.0521 ,
		GETDATE(), @usuarioId)

	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(12       , 16 , 0.25, 4.0   , 365    , 
		0.0110   , 15, 365 , 0.0411, 1.0521 ,
		GETDATE(), @usuarioId)

	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(13       , 16 , 0.25, 4.0   , 365    , 
		0.0110   , 15, 365 , 0.0411, 1.0521 ,
		GETDATE(), @usuarioId)

	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(14       , 16 , 0.25, 4.0   , 365    , 
		0.0110   , 15, 365 , 0.0411, 1.0521 ,
		GETDATE(), @usuarioId)

	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(15       , 18 , 0.25, 4.0   , 365    , 
		0.0123   , 15, 365 , 0.0411, 1.0534 ,
		GETDATE(), @usuarioId)

	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(16       , 18 , 0.25, 4.0   , 365    , 
		0.0123   , 15, 365 , 0.0411, 1.0534 ,
		GETDATE(), @usuarioId)

	INSERT INTO Factores
	( anosTrabajados  , diasVacaciones, primaVacacional , porcentaje, diasAno, 
	  factorVacaciones, aguinaldo     , diasAnoAguinaldo, factor    , factorIntegracion, 
	  fechaRegistro   , usuarioId)
	VALUES 
		(17       , 18 , 0.25, 4.0   , 365    , 
		0.0123   , 15, 365 , 0.0411, 1.0534 ,
		GETDATE(), @usuarioId)

END
GO



