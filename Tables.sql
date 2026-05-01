INSERT INTO Client (nume, adresa, telefon, email, soldClient) VALUES
('Alpha SRL',    'Str. Unirii 1',     '0721000001', 'alpha@srl.ro',   5000),
('Beta SA',      'Str. Mihai 5',      '0721000002', 'beta@sa.ro',     3200),
('Gamma SRL',    'Str. Victoriei 12', '0722000003', 'gamma@srl.ro',   2800),
('Delta SA',     'Str. Libertatii 5', '0722000004', 'delta@sa.ro',    6500);

INSERT INTO Furnizori (numeFurnizor, adresa, telefon, email, soldFurnizor, IBAN) VALUES
('Dist Nord SRL',   'Str. Nordului 10',  '0731000001', 'dist@nord.ro',  8000, 'RO49AAAA1B31007593840000'),
('Prod Sud SA',     'Str. Sudului 3',    '0731000002', 'prod@sud.ro',   5500, 'RO49BBBB1B31007593840001'),
('Trans Est SRL',   'Str. Estului 7',    '0732000003', 'trans@est.ro',  4500, 'RO49CCCC1B31007593840002');

INSERT INTO ContBancar (IBAN, sold, banca, titular) VALUES
('RO49AAAA1B31007593840000', 50000, 'Banca Transilvania', 'SC Transilvania General Import-Export SRL'),
('RO49BBBB1B31007593840001', 25000, 'BRD',                'SC Transilvania General Import-Export SRL');

INSERT INTO Caserie (soldNumerar, responsabil, locatie) VALUES
(12500, 'Popescu Ion', 'Sediu central');

INSERT INTO Angajat (functie, nume, prenume, CNP, dataNastere, dataAngajare, nrPermis, categorie) VALUES
('Sofer', 'Ionescu', 'Marius', '1850312123456', '1985-03-12', '2010-06-01', 'B123456', 'B'),
('Sofer', 'Popa',    'Vasile', '1900415234567', '1990-04-15', '2015-09-01', 'C789012', 'C');

INSERT INTO Angajat (functie, nume, prenume, CNP, dataNastere, dataAngajare, functieRH) VALUES
('AngajatRH', 'Muresan', 'Elena', '2880520345678', '1988-05-20', '2012-03-01', 'Specialist RH');

INSERT INTO Angajat (functie, nume, prenume, CNP, dataNastere, dataAngajare, nivel) VALUES
('DirectorFinanciar', 'Stanciu', 'Andrei', '1820710456789', '1982-07-10', '2008-01-15', 'Senior');

INSERT INTO FacturaClient (serie, numar, dataDocument, dataOperare, valoareTotala, TVA, scadenta, stareIncasare, codClient) VALUES
('FC', '001', '2026-03-01', '2026-03-01', 5000, 950,  '2026-04-01', 'Neincasat', 1),
('FC', '002', '2026-03-05', '2026-03-05', 3200, 608,  '2026-04-05', 'Neincasat', 2),
('FC', '003', '2026-01-15', '2026-01-15', 2380, 380,  '2026-02-15', 'Neincasat', 1),
('FC', '004', '2026-02-01', '2026-02-01', 5950, 950,  '2026-03-01', 'Neincasat', 1),
('FC', '005', '2026-03-10', '2026-03-10', 1190, 190,  '2026-04-10', 'Neincasat', 1),
('FC', '006', '2026-01-10', '2026-01-10', 4760, 760,  '2026-02-10', 'Neincasat', 2),
('FC', '007', '2026-02-15', '2026-02-15', 7140, 1140, '2026-03-15', 'Neincasat', 2),
('FC', '008', '2026-03-01', '2026-03-01', 2800, 448,  '2026-04-01', 'Neincasat', 3);

INSERT INTO FacturaFurnizor (serie, numar, dataDocument, dataOperare, valoareTotala, TVA, scadenta, stare, codFurnizor) VALUES
('FF', '101', '2026-03-01', '2026-03-01', 8000, 1520, '2026-03-31', 'Neplatit', 1),
('FF', '102', '2026-01-20', '2026-01-20', 4760, 760,  '2026-02-20', 'Neplatit', 1),
('FF', '103', '2026-02-10', '2026-02-10', 2380, 380,  '2026-03-10', 'Neplatit', 1),
('FF', '104', '2026-03-01', '2026-03-01', 6545, 1045, '2026-04-01', 'Neplatit', 1),
('FF', '201', '2026-01-15', '2026-01-15', 3570, 570,  '2026-02-15', 'Neplatit', 2),
('FF', '202', '2026-02-20', '2026-02-20', 8330, 1330, '2026-03-20', 'Neplatit', 2),
('FF', '203', '2026-03-05', '2026-03-05', 1190, 190,  '2026-04-05', 'Neplatit', 2),
('FF', '301', '2026-02-01', '2026-02-01', 4165, 665,  '2026-03-01', 'Neplatit', 3),
('FF', '302', '2026-03-10', '2026-03-10', 2975, 475,  '2026-04-10', 'Neplatit', 3);