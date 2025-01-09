RAISERROR('Starting SQL script execution for issue 1', 10, 1) WITH NOWAIT  

    INSERT INTO AspNetRoles VALUES(NEWID(), 'SysAdmin', 'SYSADMIN', NEWID());
    INSERT INTO AspNetRoles VALUES(NEWID(), 'Admin', 'ADMIN', NEWID());
    INSERT INTO AspNetRoles VALUES(NEWID(), 'User', 'USER', NEWID());

RAISERROR('Completed SQL script execution for issue 1', 10, 1) WITH NOWAIT