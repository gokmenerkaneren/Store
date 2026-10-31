ALTER TABLE "UserAddresses" ADD COLUMN IF NOT EXISTS "IsDefault" boolean NOT NULL DEFAULT false;

-- Mevcut aktif kayıtlar içinde ilk adresi varsayılan yapalım
WITH first_addr AS (
    SELECT DISTINCT ON ("CustomerId") "Id"
    FROM "UserAddresses"
    WHERE "IsDeleted" = false
    ORDER BY "CustomerId", "InsertDate" ASC
)
UPDATE "UserAddresses"
SET "IsDefault" = true
WHERE "Id" IN (SELECT "Id" FROM first_addr);

SELECT "Id", "Title", "CustomerId", "IsDefault", "IsDeleted" FROM "UserAddresses";

