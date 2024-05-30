-- Start the transaction
BEGIN;

-- Base64 encoded image (generic placeholder image)
DO $$
DECLARE
    base64_image TEXT := 'iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAABTElEQVQ4T6XTSUtDURQG4P+rcZEFiRBFD6VJLPALBFkMQUoFxcpwUQpUrFI7FFoI7fEDsFgJSJbgQNFaKTAbEKxZUatnX34zk5Fq6V+5+zH3zfm+fcmfgP0axF7FDckD3jpO23SaT2IjEAvQ3eVUM6w4BPIfsMcmA3iH+MgEjhO+GQGmYXBeqkg2wu8ZwDD5R/htcL5OkCPpxwmOglxD7cX+OmVcb8pWbVFbFEIa4CdiDsG/5NlfZr7a+vFXOGHeArwPDUcYXbMEBZ8TS8B/iLwmOVe3/OfV24AT+Fc/cMsEY9rDhRcAFjMGNL2ULjHqN8wXExZC/pGt5IlmGVdbnUe8gE0Q/qraP8DhJQxHD+Ar1pCPtql4VgAAAABJRU5ErkJggg==';

    -- Array of possible product names
    product_names TEXT[] := ARRAY[
        'Laptop 15"', 'Gaming Desktop', 'Smartphone Pro', 'Wireless Mouse', 
        'Mechanical Keyboard', '27" Monitor', 'External SSD 1TB', 
        'Gaming Headset', 'Bluetooth Speaker', 'Smartwatch', 'Tablet 10"',
        'VR Headset', 'Portable Charger', 'Router', 'Webcam'
    ];

    -- Array of possible brands
    brands TEXT[] := ARRAY[
        'BrandA', 'BrandB', 'BrandC', 'BrandD', 'BrandE'
    ];

    -- Array of categories (assumed to be in sync with your C# enum)
    categories INT[] := ARRAY[0, 1, 2, 3, 4]; -- Other, Laptop, Desktop, Mobile, Accessories

    -- Random product attributes
    price NUMERIC;
    category INT;
    stock_level INT;
    name TEXT;
    brand TEXT;
    description TEXT;
    condition INT;
    image BYTEA;
BEGIN
    FOR i IN 1..100 LOOP -- Insert 100 random products
        price := (RANDOM() * 1000 + 50)::NUMERIC(18, 2); -- Random price between 50 and 1050
        category := categories[1 + TRUNC(RANDOM() * ARRAY_LENGTH(categories, 1))];
        stock_level := TRUNC(RANDOM() * 100 + 1); -- Random stock level between 1 and 100
        name := product_names[1 + TRUNC(RANDOM() * ARRAY_LENGTH(product_names, 1))];
        brand := brands[1 + TRUNC(RANDOM() * ARRAY_LENGTH(brands, 1))];
        description := 'A high-quality ' || name || ' from ' || brand;
        condition := TRUNC(RANDOM() * 4); -- Random condition between 0 (New) and 3 (Broken)
        image := DECODE(base64_image, 'base64');

        INSERT INTO "Products"."Products" ("Name", "Description", "Price", "Category", "StockLevel", "Brand", "Condition", "GraphicsCard", "ProcessorType", "RAM", "ScreenSize", "Series", "Storage", "StorageType", "TouchScreen", "Image")
        VALUES (name, description, price, category, stock_level, brand, condition, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, image);
    END LOOP;
END $$;

-- Commit the transaction
COMMIT;
