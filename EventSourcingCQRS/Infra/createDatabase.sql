CREATE TABLE "Events" (
	"EventId"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	"AggregateName"	TEXT NOT NULL,
	"AggregateId"	TEXT NOT NULL,
	"EventType"	TEXT NOT NULL,
	"Data"	TEXT NOT NULL,
	"Metadata"	TEXT NOT NULL,
	CONSTRAINT "U_Events" UNIQUE (AggregateName, AggregateId, EventId)
);

CREATE TABLE "Category" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Category" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NULL
);

INSERT INTO "Category"("Name") VALUES ("Sports");

CREATE TABLE "Question" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Question" PRIMARY KEY AUTOINCREMENT,
    "Text" TEXT NULL,
    "Answer" TEXT NULL,
    "CategoryId" INTEGER NOT NULL,
    CONSTRAINT "FK_Question_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE
);

INSERT INTO "Question"("Text", "Answer", "CategoryId")
    WITH RECURSIVE cte(i) AS (
        SELECT 1
        UNION ALL
        SELECT i + 1 FROM cte LIMIT 100)
    SELECT "Sport " + i, "sport " + i, 1
	FROM cte
