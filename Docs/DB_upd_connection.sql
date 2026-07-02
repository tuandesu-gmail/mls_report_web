SELECT TOP (1000) [ID]
      ,[UserId]
      ,[Name]
      ,[Value]
      ,[Type]
      ,[M_Server]
      ,[M_Database]
      ,[M_User]
      ,[M_Pass]
      ,[ValueBin]
  FROM [Meliasoft_Web].[dbo].[Sys_UserProperty]
  WHERE [UserId] IN (64, 66, 1432)

  SELECT CAST('Server=118.70.187.118,1909;Database=Ms2026_AI;User Id=Meliasoft_HDDT;Password=60145;TrustServerCertificate=True;' AS varbinary(MAX)) AS ConnectionString FROM Sys_UserProperty 
  WHERE [UserId] IN (64, 66, 1432)

  --UPDATE [Meliasoft_Web].[dbo].[Sys_UserProperty] SET [ValueBin] = CAST('Server=118.70.187.118,1909;Database=Ms2026_AI;User Id=Meliasoft_HDDT;Password=60145;TrustServerCertificate=True;' AS varbinary(MAX))
  --WHERE [UserId] IN (1432)
