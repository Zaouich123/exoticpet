-- Create animals table
CREATE TABLE IF NOT EXISTS "Animals" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Species" VARCHAR(100) NOT NULL,
    "Age" INTEGER NOT NULL
);

-- Insert sample data
INSERT INTO "Animals" ("Name", "Species", "Age") 
VALUES 
    ('Leo', 'Lion', 5),
    ('Mamba', 'Snake', 3),
    ('Tiki', 'Parrot', 2)
ON CONFLICT DO NOTHING;
