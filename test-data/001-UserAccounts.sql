CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'UserAccounts') THEN
        CREATE SCHEMA "UserAccounts";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'UserAccounts') THEN
        CREATE SCHEMA "UserAccounts";
    END IF;
END $EF$;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE "UserAccounts"."Roles" (
    "RoleId" uuid NOT NULL,
    "Name" character varying(256) NOT NULL,
    CONSTRAINT "PK_Roles" PRIMARY KEY ("RoleId")
);

CREATE TABLE "UserAccounts"."Users" (
    "UserId" uuid NOT NULL DEFAULT (uuid_generate_v4()),
    "Username" character varying(256) NOT NULL,
    "Email" character varying(256) NOT NULL,
    "PasswordHash" bytea NOT NULL,
    "Status" integer NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("UserId")
);

CREATE TABLE "UserAccounts"."AccountActivities" (
    "ActivityId" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Timestamp" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "ActivityType" text NOT NULL,
    "Description" text NOT NULL,
    CONSTRAINT "PK_AccountActivities" PRIMARY KEY ("ActivityId"),
    CONSTRAINT "FK_AccountActivities_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "UserAccounts"."Users" ("UserId") ON DELETE CASCADE
);

CREATE TABLE "UserAccounts"."Addresses" (
    "AddressId" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "AddressLine" character varying(1024) NOT NULL,
    "City" character varying(256) NOT NULL,
    "State" character varying(256) NOT NULL,
    "ZipCode" character varying(10) NOT NULL,
    "Country" character varying(256) NOT NULL,
    "IsShippingAddress" boolean NOT NULL,
    "IsBillingAddress" boolean NOT NULL,
    CONSTRAINT "PK_Addresses" PRIMARY KEY ("AddressId"),
    CONSTRAINT "FK_Addresses_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "UserAccounts"."Users" ("UserId") ON DELETE CASCADE
);

CREATE TABLE "UserAccounts"."SecuritySettings" (
    "UserId" uuid NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "SecurityQuestion" text NOT NULL,
    "SecurityAnswerHash" text NOT NULL,
    CONSTRAINT "PK_SecuritySettings" PRIMARY KEY ("UserId"),
    CONSTRAINT "FK_SecuritySettings_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "UserAccounts"."Users" ("UserId") ON DELETE CASCADE
);

CREATE TABLE "UserAccounts"."UserPreferences" (
    "UserId" uuid NOT NULL,
    "ReceiveNewsletter" boolean NOT NULL,
    "PreferredPaymentMethod" text NOT NULL,
    CONSTRAINT "PK_UserPreferences" PRIMARY KEY ("UserId"),
    CONSTRAINT "FK_UserPreferences_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "UserAccounts"."Users" ("UserId") ON DELETE CASCADE
);

CREATE TABLE "UserAccounts"."UserRoles" (
    "RolesRoleId" uuid NOT NULL,
    "UsersUserId" uuid NOT NULL,
    CONSTRAINT "PK_UserRoles" PRIMARY KEY ("RolesRoleId", "UsersUserId"),
    CONSTRAINT "FK_UserRoles_Roles_RolesRoleId" FOREIGN KEY ("RolesRoleId") REFERENCES "UserAccounts"."Roles" ("RoleId") ON DELETE CASCADE,
    CONSTRAINT "FK_UserRoles_Users_UsersUserId" FOREIGN KEY ("UsersUserId") REFERENCES "UserAccounts"."Users" ("UserId") ON DELETE CASCADE
);

CREATE INDEX "IX_AccountActivities_UserId" ON "UserAccounts"."AccountActivities" ("UserId");

CREATE INDEX "IX_Addresses_UserId" ON "UserAccounts"."Addresses" ("UserId");

CREATE INDEX "IX_UserRoles_UsersUserId" ON "UserAccounts"."UserRoles" ("UsersUserId");

CREATE UNIQUE INDEX "IX_Users_Email" ON "UserAccounts"."Users" ("Email");

CREATE UNIQUE INDEX "IX_Users_Username" ON "UserAccounts"."Users" ("Username");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240418014319_InitialCreate', '7.0.18');

COMMIT;

START TRANSACTION;

ALTER TABLE "UserAccounts"."Users" ALTER COLUMN "PasswordHash" TYPE text;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240419124048_MakePasswordHashString', '7.0.18');

COMMIT;

START TRANSACTION;

ALTER TABLE "UserAccounts"."Users" ADD "FirstName" character varying(256) NOT NULL DEFAULT '';

ALTER TABLE "UserAccounts"."Users" ADD "Surname" character varying(256) NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240426152345_AddUsersFirstNameLastName', '7.0.18');

COMMIT;

START TRANSACTION;

DROP TABLE "UserAccounts"."UserRoles";

DROP TABLE "UserAccounts"."Roles";

ALTER TABLE "UserAccounts"."Users" ADD "Role" integer NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240428115039_simplifiedUserRole', '7.0.18');

COMMIT;

START TRANSACTION;

ALTER TABLE "UserAccounts"."Addresses" RENAME COLUMN "ZipCode" TO "PostCode";

ALTER TABLE "UserAccounts"."Addresses" RENAME COLUMN "State" TO "County";

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240428120010_slightchanges', '7.0.18');

COMMIT;

