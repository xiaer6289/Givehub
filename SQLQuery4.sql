INSERT INTO Donees (Name, Date, Description, Category, Address, Image, Requirements, AdminId)
VALUES (
    'Temporary Donee',                 -- Name
    GETDATE(),                         -- Date (current timestamp)
    'This is a temporary donee',       -- Description (optional)
    'Nursing Home',                     -- Category (optional)
    '123 Temporary Street',             -- Address (required)
    NULL,                               -- Image (optional)
    NULL,                               -- Requirements (optional)
    1                                   -- AdminId (must exist in Admins table)
);
