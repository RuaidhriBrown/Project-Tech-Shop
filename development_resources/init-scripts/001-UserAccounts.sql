-- Start the transaction
BEGIN;

-- Create the schema if it doesn't exist
CREATE SCHEMA IF NOT EXISTS "UserAccounts";

-- Ensure the migration history table exists
CREATE TABLE IF NOT EXISTS "UserAccounts"."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

-- Create extensions if they do not exist
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- User Roles table
CREATE TABLE IF NOT EXISTS "UserAccounts"."Roles" (
    "RoleId" uuid NOT NULL DEFAULT uuid_generate_v4(),
    "Name" character varying(256) NOT NULL,
    CONSTRAINT "PK_Roles" PRIMARY KEY ("RoleId")
);

-- Users table
CREATE TABLE IF NOT EXISTS "UserAccounts"."Users" (
    "UserId" uuid NOT NULL DEFAULT uuid_generate_v4(),
    "Username" character varying(256) NOT NULL,
    "FirstName" character varying(256) NOT NULL,
    "Surname" character varying(256) NOT NULL,
    "Email" character varying(256) NOT NULL,
    "PasswordHash" text NOT NULL,
    "Role" integer NOT NULL,
    "Status" integer NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("UserId"),
    CONSTRAINT "UQ_Users_Username" UNIQUE ("Username"),
    CONSTRAINT "UQ_Users_Email" UNIQUE ("Email")
);

-- Addresses table
CREATE TABLE IF NOT EXISTS "UserAccounts"."Addresses" (
    "AddressId" uuid NOT NULL DEFAULT uuid_generate_v4(),
    "UserId" uuid NOT NULL,
    "AddressLine" character varying(1024) NOT NULL,
    "City" character varying(256) NOT NULL,
    "County" character varying(256) NOT NULL,
    "PostCode" character varying(10) NOT NULL,
    "Country" character varying(256) NOT NULL,
    "IsShippingAddress" boolean NOT NULL,
    "IsBillingAddress" boolean NOT NULL,
    CONSTRAINT "PK_Addresses" PRIMARY KEY ("AddressId"),
    CONSTRAINT "FK_Addresses_Users" FOREIGN KEY ("UserId") REFERENCES "UserAccounts"."Users" ("UserId") ON DELETE CASCADE
);

-- Security settings
CREATE TABLE IF NOT EXISTS "UserAccounts"."SecuritySettings" (
    "UserId" uuid NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "SecurityQuestion" text NOT NULL,
    "SecurityAnswerHash" text NOT NULL,
    CONSTRAINT "PK_SecuritySettings" PRIMARY KEY ("UserId"),
    CONSTRAINT "FK_SecuritySettings_Users" FOREIGN KEY ("UserId") REFERENCES "UserAccounts"."Users" ("UserId") ON DELETE CASCADE
);

-- User preferences
CREATE TABLE IF NOT EXISTS "UserAccounts"."UserPreferences" (
    "UserId" uuid NOT NULL,
    "ReceiveNewsletter" boolean NOT NULL,
    "PreferredPaymentMethod" text NOT NULL,
    CONSTRAINT "PK_UserPreferences" PRIMARY KEY ("UserId"),
    CONSTRAINT "FK_UserPreferences_Users" FOREIGN KEY ("UserId") REFERENCES "UserAccounts"."Users" ("UserId") ON DELETE CASCADE
);

-- Account activities
CREATE TABLE IF NOT EXISTS "UserAccounts"."AccountActivities" (
    "ActivityId" uuid NOT NULL DEFAULT uuid_generate_v4(),
    "UserId" uuid NOT NULL,
    "Timestamp" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ActivityType" text NOT NULL,
    "Description" text NOT NULL,
    CONSTRAINT "PK_AccountActivities" PRIMARY KEY ("ActivityId"),
    CONSTRAINT "FK_AccountActivities_Users" FOREIGN KEY ("UserId") REFERENCES "UserAccounts"."Users" ("UserId") ON DELETE CASCADE
);

-- Indexes for faster lookup
CREATE INDEX IF NOT EXISTS "IX_AccountActivities_UserId" ON "UserAccounts"."AccountActivities" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_Addresses_UserId" ON "UserAccounts"."Addresses" ("UserId");

-- Add migration history
INSERT INTO "UserAccounts"."__EFMigrationsHistory" ("MigrationId", "ProductVersion") VALUES
('20240418014319_InitialCreate', '7.0.18'),
('20240419124048_MakePasswordHashString', '7.0.18'),
('20240426152345_AddUsersFirstNameLastName', '7.0.18'),
('20240428115039_simplifiedUserRole', '7.0.18'),
('20240428120010_slightchanges', '7.0.18')
ON CONFLICT ("MigrationId") DO NOTHING;

-- Commit all changes
COMMIT;
