use [mainkatthe]
/****** Script for SelectTopNRows command from SSMS  ******/
SELECT *  FROM [dbo].[Tenants]
SELECT *   FROM [dbo].[AspNetRoles]
SELECT *  FROM [dbo].[AspNetUserRoles]
select * from [dbo].[AspNetUsers]

--update [dbo].[AspNetUsers]
--set TenantId='b7384612-5768-445a-bf62-aeddd039cf9a' where id='b76c06e9-a767-4824-835f-8b4e86b52c90'

--select * FROM [dbo].[AspNetRoleClaims]
select * from [AspNetUserClaims]