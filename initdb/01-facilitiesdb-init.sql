﻿CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240411193104_InitialCreate') THEN
    CREATE TABLE factories (
        id integer GENERATED BY DEFAULT AS IDENTITY,
        name text NOT NULL,
        description text,
        CONSTRAINT "PK_factories" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240411193104_InitialCreate') THEN
    CREATE TABLE units (
        id integer GENERATED BY DEFAULT AS IDENTITY,
        name text NOT NULL,
        description text,
        factoryid integer NOT NULL,
        CONSTRAINT "PK_units" PRIMARY KEY (id),
        CONSTRAINT "FK_units_factories_factoryid" FOREIGN KEY (factoryid) REFERENCES factories (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240411193104_InitialCreate') THEN
    CREATE TABLE tanks (
        id integer GENERATED BY DEFAULT AS IDENTITY,
        name text NOT NULL,
        description text,
        unitid integer NOT NULL,
        volume integer NOT NULL,
        maxvolume integer NOT NULL,
        CONSTRAINT "PK_tanks" PRIMARY KEY (id),
        CONSTRAINT "FK_tanks_units_unitid" FOREIGN KEY (unitid) REFERENCES units (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240411193104_InitialCreate') THEN
    INSERT INTO factories (id, description, name)
    VALUES (1, 'Первый нефтеперерабатывающий завод', 'НПЗ№1');
    INSERT INTO factories (id, description, name)
    VALUES (2, 'Второй нефтеперерабатывающий завод', 'НПЗ№2');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240411193104_InitialCreate') THEN
    INSERT INTO units (id, description, factoryid, name)
    VALUES (1, 'Газофракционирующая установка', 1, 'ГФУ-2');
    INSERT INTO units (id, description, factoryid, name)
    VALUES (2, 'Атмосферно-вакуумная трубчатка', 1, 'АВТ-6');
    INSERT INTO units (id, description, factoryid, name)
    VALUES (3, 'Атмосферно - вакуумная трубчатка', 2, 'АВТ-10');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240411193104_InitialCreate') THEN
    INSERT INTO tanks (id, description, maxvolume, name, unitid, volume)
    VALUES (1, 'Надземный-вертикальный', 2000, 'Резервуар 1', 1, 1500);
    INSERT INTO tanks (id, description, maxvolume, name, unitid, volume)
    VALUES (2, 'Надземный-горизонтальный', 3000, 'Резервуар 2', 1, 2500);
    INSERT INTO tanks (id, description, maxvolume, name, unitid, volume)
    VALUES (3, 'Надземный-горизонтальный', 3000, 'Резервуар 3', 2, 3000);
    INSERT INTO tanks (id, description, maxvolume, name, unitid, volume)
    VALUES (4, 'Надземный-вертикальный', 3000, 'Резервуар 4', 2, 3000);
    INSERT INTO tanks (id, description, maxvolume, name, unitid, volume)
    VALUES (5, 'Подземный-двустенный', 5000, 'Резервуар 5', 2, 4000);
    INSERT INTO tanks (id, description, maxvolume, name, unitid, volume)
    VALUES (6, 'Подводный', 500, 'Резервуар 6', 2, 500);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240411193104_InitialCreate') THEN
    CREATE INDEX "IX_tanks_unitid" ON tanks (unitid);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240411193104_InitialCreate') THEN
    CREATE INDEX "IX_units_factoryid" ON units (factoryid);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240411193104_InitialCreate') THEN
    PERFORM setval(
        pg_get_serial_sequence('factories', 'id'),
        GREATEST(
            (SELECT MAX(id) FROM factories) + 1,
            nextval(pg_get_serial_sequence('factories', 'id'))),
        false);
    PERFORM setval(
        pg_get_serial_sequence('units', 'id'),
        GREATEST(
            (SELECT MAX(id) FROM units) + 1,
            nextval(pg_get_serial_sequence('units', 'id'))),
        false);
    PERFORM setval(
        pg_get_serial_sequence('tanks', 'id'),
        GREATEST(
            (SELECT MAX(id) FROM tanks) + 1,
            nextval(pg_get_serial_sequence('tanks', 'id'))),
        false);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240411193104_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20240411193104_InitialCreate', '8.0.3');
    END IF;
END $EF$;
COMMIT;
