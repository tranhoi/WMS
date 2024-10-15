 SELECT * FROM wms.AspNetRoles
 
 SELECT * FROM wms.[AspNetUsers]
  
  --SELECT * FROM authp.Tenants
  
    SELECT * FROM wms.[Permissions]

  SELECT * FROM wms.AspNetUserRoles
  
    SELECT * FROM wms.[RoleToPermission]

  SELECT * FROM wms.usertotenant

  --truncate table wms.[AspNetUsers]

  SELECT  *  FROM wms.refreshtokens  order by ExpirationTime desc


              SELECT *
  FROM wms.Locations
	  SELECT *
  FROM wms.Bins

          SELECT *
  FROM wms.Units

            SELECT *
  FROM dbo.Suppliers

  select* from WMS.Logtime order by createddate desc
  --truncate table Wms.[Logtime]

  SELECT  creation_time 
       ,last_execution_time
       ,total_physical_reads
       ,total_logical_reads 
       ,total_logical_writes
       , execution_count
       , total_worker_time
       , total_elapsed_time
       , total_elapsed_time / execution_count avg_elapsed_time
       ,SUBSTRING(st.text, (qs.statement_start_offset/2) + 1,
        ((CASE statement_end_offset
         WHEN -1 THEN DATALENGTH(st.text)
         ELSE qs.statement_end_offset END
           - qs.statement_start_offset)/2) + 1) AS statement_text
FROM sys.dm_exec_query_stats AS qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) st
ORDER BY total_elapsed_time / execution_count DESC;
  
 -- select _per.*
	--	,_roleToPer.RoleId
	--	,_roleToPer.RoleName
 -- from wms.[Permissions] _per
	--left join wms.RoleToPermission _roleToPer on _roleToPer.PermissionId = _per.Id

  --insert into wms.[Permissions] 
  --(Id,RoleId,RoleName,PermissionId,PermisionName,PermisionDescription)
  --values
  --( NEWID(),'4b128b49-96a9-4c52-a261-c812bbb43f33','Admin', 'FB3C3FD2-CA28-4F9F-AD5B-5BE51826BFA4','Insert','Allow add new data')
  
  --update wms.RoleToPermission set RoleName= 'Admin' where RoleId='7E9F080F-3CF3-4277-96AE-B476465FC257'
  --truncate table wms.roletopermission
  --SELECT *
  --FROM RoleToPermissions
  -- SELECT *
  --FROM RoleToPermissionsTenant

  --select * from Unit
  --select * from Category
  --select * from product

  --truncate table Wms.[Logtime]
    --truncate table Wms.[refreshtokens]
  --truncate table Category
  --truncate table product
  --Xb0zPmUsezyXb26jvz0YQETK87EB5LDFdpYPWI7FQeCtGxnQyqmXr3A7lyh24FXOnLZqDANDUcvhgWmwCGQKjg==