-- SQL script to insert a new admin user in the "UserAccounts" schema
INSERT INTO "UserAccounts"."Users" ("UserId", "Username", "FirstName", "Surname", "Email", "PasswordHash", "Role", "Status")
VALUES (
    gen_random_uuid(), -- Generates a random UUID for the user ID
    'admin',          -- Username
    'Admin',          -- First Name
    'User',           -- Surname
    'admin@example.com', -- Email
    '$2a$11$OFEJgTJ44AWUJAuYG24dge2NNi1tS8ZZwnMhFTUp9Yx1cKF2K5ehe', -- Password hash for 'Project1'
    2,                -- Role: Admin
    0                 -- Status: Active
);
