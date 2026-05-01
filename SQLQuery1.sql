CREATE TABLE Client (
    codClient INT PRIMARY KEY IDENTITY,
    nume NVARCHAR(100) NOT NULL,
    adresa NVARCHAR(200),
    telefon NVARCHAR(20),
    email NVARCHAR(100),
    soldClient DECIMAL(12,2) DEFAULT 0
);

CREATE TABLE Furnizori (
    codFurnizor INT PRIMARY KEY IDENTITY,
    numeFurnizor NVARCHAR(100) NOT NULL,
    adresa NVARCHAR(200),
    telefon NVARCHAR(20),
    email NVARCHAR(100),
    soldFurnizor DECIMAL(12,2) DEFAULT 0,
    IBAN NVARCHAR(34)
);

CREATE TABLE FacturaClient (
    serie NVARCHAR(10) NOT NULL,
    numar NVARCHAR(10) NOT NULL,
    dataDocument DATE NOT NULL,
    dataOperare DATE NOT NULL,
    valoareTotala DECIMAL(12,2) NOT NULL,
    TVA DECIMAL(12,2) NOT NULL,
    scadenta DATE,
    stareIncasare NVARCHAR(20) DEFAULT 'Neincasat',
    codClient INT FOREIGN KEY REFERENCES Client(codClient),
    PRIMARY KEY (serie, numar)
);

CREATE TABLE FacturaFurnizor (
    serie NVARCHAR(10) NOT NULL,
    numar NVARCHAR(10) NOT NULL,
    dataDocument DATE NOT NULL,
    dataOperare DATE NOT NULL,
    valoareTotala DECIMAL(12,2) NOT NULL,
    TVA DECIMAL(12,2) NOT NULL,
    scadenta DATE,
    stare NVARCHAR(20) DEFAULT 'Neplatit',
    codFurnizor INT FOREIGN KEY REFERENCES Furnizori(codFurnizor),
    PRIMARY KEY (serie, numar)
);

CREATE TABLE FacturaRetur (
    serie NVARCHAR(10) NOT NULL,
    numar NVARCHAR(10) NOT NULL,
    dataDocument DATE NOT NULL,
    dataOperare DATE NOT NULL,
    valoareRetur DECIMAL(12,2) NOT NULL,
    motivRetur NVARCHAR(200),
    stareRetur NVARCHAR(20) DEFAULT 'Emis',
    tipRetur NVARCHAR(10) NOT NULL CHECK (tipRetur IN ('Client','Furnizor')),
    codClient INT NULL FOREIGN KEY REFERENCES Client(codClient),
    codFurnizor INT NULL FOREIGN KEY REFERENCES Furnizori(codFurnizor),
    serieFactInit NVARCHAR(10),
    numarFactInit NVARCHAR(10),
    PRIMARY KEY (serie, numar)
);

-- Date demo
INSERT INTO Client VALUES ('Alpha SRL','Str. Unirii 1','0721000001','alpha@srl.ro', 5000);
INSERT INTO Client VALUES ('Beta SA','Str. Mihai 5','0721000002','beta@sa.ro', 3200);
INSERT INTO Furnizori VALUES ('Dist Nord SRL','Str. Nordului 10','0731000001','dist@nord.ro', 8000, 'RO49AAAA1B31007593840000');
INSERT INTO Furnizori VALUES ('Prod Sud SA','Str. Sudului 3','0731000002','prod@sud.ro', 5500, 'RO49BBBB1B31007593840001');
INSERT INTO FacturaClient VALUES ('FC','001','2026-03-01','2026-03-01',5000,950,'2026-04-01','Neincasat',1);
INSERT INTO FacturaClient VALUES ('FC','002','2026-03-05','2026-03-05',3200,608,'2026-04-05','Neincasat',2);
INSERT INTO FacturaFurnizor VALUES ('FF','101','2026-03-01','2026-03-01',8000,1520,'2026-03-31','Neplatit',1);
INSERT INTO FacturaFurnizor VALUES ('FF','102','2026-03-05','2026-03-05',5500,1045,'2026-04-05','Neplatit',2);