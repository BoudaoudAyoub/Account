-- Replace the placeholder "0" with the specific issue or script number 
-- to accurately track the SQL script being executed during deployment 
-- in the terminal logs.

RAISERROR('Starting SQL script execution for issue #0', 10, 1) WITH NOWAIT  

    -- INSERT YOUR SQL CODE HERE  

RAISERROR('Completed SQL script execution for issue #0', 10, 1) WITH NOWAIT