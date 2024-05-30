-- Start the transaction
BEGIN;

-- Array of usernames, first names, and surnames for the new users
DO $$
DECLARE
    user_data TEXT[][] := ARRAY[
        ARRAY['user1', 'John', 'Doe'],
        ARRAY['user2', 'Jane', 'Smith'],
        ARRAY['user3', 'Bob', 'Brown'],
        ARRAY['user4', 'Alice', 'Johnson'],
        ARRAY['user5', 'Charlie', 'White'],
        ARRAY['user6', 'Emily', 'Davis'],
        ARRAY['user7', 'David', 'Wilson'],
        ARRAY['chromiumUser', 'chromium', 'User'],
        ARRAY['firefoxUser', 'firefox', 'User'],
        ARRAY['webkitUser', 'webkit', 'User']
    ];

    -- Base64 encoded image (generic placeholder image)
    base64_image TEXT := 'iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAABTElEQVQ4T6XTSUtDURQG4P+rcZEFiRBFD6VJLPALBFkMQUoFxcpwUQpUrFI7FFoI7fEDsFgJSJbgQNFaKTAbEKxZUatnX34zk5Fq6V+5+zH3zfm+fcmfgP0axF7FDckD3jpO23SaT2IjEAvQ3eVUM6w4BPIfsMcmA3iH+MgEjhO+GQGmYXBeqkg2wu8ZwDD5R/htcL5OkCPpxwmOglxD7cX+OmVcb8pWbVFbFEIa4CdiDsG/5NlfZr7a+vFXOGHeArwPDUcYXbMEBZ8TS8B/iLwmOVe3/OfV24AT+Fc/cMsEY9rDhRcAFjMGNL2ULjHqN8wXExZC/pGt5IlmGVdbnUe8gE0Q/qraP8DhJQxHD+Ar1pCPtql4VgAAAABJRU5ErkJggg==';

    -- Variables for random attributes
    user_id UUID;
    address_id UUID;
    password_hash TEXT := '$2a$11$OFEJgTJ44AWUJAuYG24dge2NNi1tS8ZZwnMhFTUp9Yx1cKF2K5ehe'; -- Password hash for 'Project1'
    security_answer_hash TEXT := '$2a$12$eB7Q1VrY8lz8nMv3fV1tOOp9Rc9UkeOr5D1xNNuAkE3KxKP7eQ.5G'; -- Security answer hash for 'Blue'
    price NUMERIC;
    category INT;
    stock_level INT;
    name TEXT;
    brand TEXT;
    description TEXT;
    condition INT;
    image BYTEA := DECODE(base64_image, 'base64');

BEGIN
    FOR i IN 1..10 LOOP
        -- Generate a new user ID
        user_id := gen_random_uuid();

        -- Insert new user into Users table
        INSERT INTO "UserAccounts"."Users" ("UserId", "Username", "FirstName", "Surname", "Email", "PasswordHash", "Role", "Status", "RowVersion")
        VALUES (
            user_id,
            user_data[i][1],
            user_data[i][2],
            user_data[i][3],
            user_data[i][1] || '@example.com',
            password_hash,
            0, -- Role: Customer
            0, -- Status: Active
            gen_random_bytes(8)
        );

        -- Insert address for the new user
        INSERT INTO "UserAccounts"."Addresses" ("AddressId", "UserId", "AddressLine", "City", "County", "PostCode", "Country", "IsShippingAddress", "IsBillingAddress")
        VALUES (
            gen_random_uuid(),
            user_id,
            '123 Main St',
            'City' || i,
            'County' || i,
            'PC' || i,
            'Country' || i,
            TRUE,
            TRUE
        );

        -- Insert security settings for the new user
        INSERT INTO "UserAccounts"."SecuritySettings" ("UserId", "TwoFactorEnabled", "SecurityQuestion", "SecurityAnswerHash")
        VALUES (
            user_id,
            TRUE,
            'What is your favorite color?',
            security_answer_hash
        );

        -- Insert user preferences for the new user
        INSERT INTO "UserAccounts"."UserPreferences" ("UserId", "ReceiveNewsletter", "PreferredPaymentMethod")
        VALUES (
            user_id,
            TRUE,
            'Credit Card'
        );

        -- Insert account activity for the new user
        INSERT INTO "UserAccounts"."AccountActivities" ("ActivityId", "UserId", "Timestamp", "ActivityType", "Description")
        VALUES (
            gen_random_uuid(),
            user_id,
            CURRENT_TIMESTAMP,
            'Account Created',
            'User account created successfully'
        );
    END LOOP;
END $$;

-- Commit the transaction
COMMIT;
