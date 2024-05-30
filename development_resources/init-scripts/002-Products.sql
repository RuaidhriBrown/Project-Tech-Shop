-- Start the transaction
BEGIN;

-- Create the schema if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_namespace WHERE nspname = 'Products') THEN
        CREATE SCHEMA "Products";
    END IF;
END $$;

-- Create the migration history table if it doesn't exist
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

-- Create extensions if they do not exist
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Create the Products table if it does not exist
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_class WHERE relname = 'Products' AND relnamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'Products')) THEN
        CREATE TABLE "Products"."Products" (
            "ProductId" SERIAL PRIMARY KEY,
            "Name" VARCHAR(256) NOT NULL,
            "Description" VARCHAR(256) NOT NULL,
            "Price" NUMERIC(18, 2) NOT NULL,
            "Category" INTEGER NOT NULL,
            "StockLevel" INTEGER NOT NULL,
            "Brand" VARCHAR(256) NOT NULL DEFAULT '',
            "Condition" INTEGER NOT NULL DEFAULT 0,
            "GraphicsCard" TEXT,
            "ProcessorType" TEXT,
            "RAM" INTEGER,
            "ScreenSize" DOUBLE PRECISION,
            "Series" TEXT,
            "Storage" INTEGER,
            "StorageType" TEXT,
            "TouchScreen" BOOLEAN,
            "Image" TEXT NOT NULL DEFAULT ''
        );

        CREATE INDEX "IX_Products_Category" ON "Products"."Products" ("Category");
    END IF;
END $$;

-- Create the Baskets table if it does not exist
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_class WHERE relname = 'Baskets' AND relnamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'Products')) THEN
        CREATE TABLE "Products"."Baskets" (
            "BasketId" uuid NOT NULL DEFAULT uuid_generate_v4(),
            "CustomerId" uuid NOT NULL,
            "Status" integer NOT NULL,
            CONSTRAINT "PK_Baskets" PRIMARY KEY ("BasketId")
        );
    END IF;
END $$;

-- Create the BasketItems table if it does not exist
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_class WHERE relname = 'BasketItems' AND relnamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'Products')) THEN
        CREATE TABLE "Products"."BasketItems" (
            "BasketItemId" SERIAL PRIMARY KEY,
            "BasketId" uuid NOT NULL,
            "ProductId" integer NOT NULL,
            "Quantity" integer NOT NULL,
            CONSTRAINT "FK_BasketItems_Baskets_BasketId" FOREIGN KEY ("BasketId") REFERENCES "Products"."Baskets" ("BasketId") ON DELETE CASCADE,
            CONSTRAINT "FK_BasketItems_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products"."Products" ("ProductId") ON DELETE CASCADE
        );
    END IF;
END $$;

-- Create the Sales table if it does not exist
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_class WHERE relname = 'Sales' AND relnamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'Products')) THEN
        CREATE TABLE "Products"."Sales" (
            "SaleId" SERIAL PRIMARY KEY,
            "BasketId" uuid NOT NULL,
            "QuantitySold" integer NOT NULL,
            "SaleDate" timestamp with time zone NOT NULL,
            "TotalSaleAmount" numeric(18,2) NOT NULL,
            "CustomerId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000',
            "Status" character varying(256) NOT NULL DEFAULT '',
            CONSTRAINT "FK_Sales_Baskets_BasketId" FOREIGN KEY ("BasketId") REFERENCES "Products"."Baskets" ("BasketId") ON DELETE CASCADE
        );

        CREATE INDEX "IX_Sales_BasketId" ON "Products"."Sales" ("BasketId");
    END IF;
END $$;

-- Insert migration history once all operations are done
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES 
    ('20240427193511_CreatedProductsSchema', '7.0.18'),
    ('20240428124655_AddedLaptopDetailsToProduct', '7.0.18'),
    ('20240429191732_RefinedBasketsandAndsaleTables', '7.0.18'),
    ('20240507130419_missingstuff', '7.0.18'),
    ('20240507234308_fixProductCategories2', '7.0.18'),
    ('20240508003443_fixProductCategories3', '7.0.18')
ON CONFLICT ("MigrationId") DO NOTHING;

-- Commit all changes
COMMIT;
