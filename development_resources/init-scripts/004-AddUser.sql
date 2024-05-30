-- SQL script to insert a new admin user in the "UserAccounts" schema
INSERT INTO "UserAccounts"."Users" ("UserId", "Username", "FirstName", "Surname", "Email", "PasswordHash", "Role", "Status", "RowVersion")
VALUES (
    gen_random_uuid(), -- Generates a random UUID for the user ID
    'admin',          -- Username
    'Admin',          -- First Name
    'User',           -- Surname
    'admin@example.com', -- Email
    '$2a$11$OFEJgTJ44AWUJAuYG24dge2NNi1tS8ZZwnMhFTUp9Yx1cKF2K5ehe', -- Password hash for 'Project1'
    2,                -- Role: Admin
    0,                -- Status: Active
    gen_random_bytes(8) -- Initializes RowVersion with random bytes
);

-- Variable to store the new user ID
DO $$
DECLARE
    admin_user_id uuid;
BEGIN
    SELECT "UserId" INTO admin_user_id FROM "UserAccounts"."Users" WHERE "Username" = 'admin';

    -- Insert address for the new admin user
    INSERT INTO "UserAccounts"."Addresses" ("AddressId", "UserId", "AddressLine", "City", "County", "PostCode", "Country", "IsShippingAddress", "IsBillingAddress")
    VALUES (
        gen_random_uuid(), -- Generates a random UUID for the address ID
        admin_user_id,     -- User ID of the admin user
        '123 Admin St',    -- Address Line
        'Admin City',      -- City
        'Admin County',    -- County
        'ADM1N',           -- PostCode
        'Admin Country',   -- Country
        TRUE,              -- IsShippingAddress
        TRUE               -- IsBillingAddress
    );

    -- Insert security settings for the new admin user
    INSERT INTO "UserAccounts"."SecuritySettings" ("UserId", "TwoFactorEnabled", "SecurityQuestion", "SecurityAnswerHash")
    VALUES (
        admin_user_id,                         -- User ID of the admin user
        TRUE,                                  -- TwoFactorEnabled
        'What is your favorite color?',        -- Security Question
        '$2a$12$eB7Q1VrY8lz8nMv3fV1tOOp9Rc9UkeOr5D1xNNuAkE3KxKP7eQ.5G' -- Security answer hash for 'Blue'
    );

    -- Insert user preferences for the new admin user
    INSERT INTO "UserAccounts"."UserPreferences" ("UserId", "ReceiveNewsletter", "PreferredPaymentMethod")
    VALUES (
        admin_user_id,                         -- User ID of the admin user
        TRUE,                                  -- ReceiveNewsletter
        'Credit Card'                          -- PreferredPaymentMethod
    );

    -- Insert account activity for the new admin user
    INSERT INTO "UserAccounts"."AccountActivities" ("ActivityId", "UserId", "Timestamp", "ActivityType", "Description")
    VALUES (
        gen_random_uuid(),                     -- Generates a random UUID for the activity ID
        admin_user_id,                         -- User ID of the admin user
        CURRENT_TIMESTAMP,                     -- Timestamp
        'Account Created',                     -- ActivityType
        'Admin account created successfully'   -- Description
    );
END $$;
