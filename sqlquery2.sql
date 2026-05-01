-- Mai multi clienti
INSERT INTO Client (nume, adresa, telefon, email, soldClient) VALUES
('Gamma SRL',    'Str. Victoriei 12', '0722000003', 'gamma@srl.ro',   2800),
('Delta SA',     'Str. Libertatii 5', '0722000004', 'delta@sa.ro',    6500),
('Epsilon SRL',  'Str. Florilor 8',   '0722000005', 'epsilon@srl.ro', 1200),
('Zeta Trading', 'Bd. Unirii 22',     '0722000006', 'zeta@trade.ro',  9000);

-- Mai multi furnizori
INSERT INTO Furnizori (numeFurnizor, adresa, telefon, email, soldFurnizor, IBAN) VALUES
('Trans Est SRL',   'Str. Estului 7',   '0732000003', 'trans@est.ro',   4500, 'RO49CCCC1B31007593840002'),
('Alpha Import SA', 'Str. Nordului 15', '0732000004', 'alpha@imp.ro',   7200, 'RO49DDDD1B31007593840003'),
('Beta Logistic',   'Bd. Sudului 3',    '0732000005', 'beta@log.ro',    3100, 'RO49EEEE1B31007593840004');

-- Facturi client — pentru Alpha SRL (codClient=1)
INSERT INTO FacturaClient (serie, numar, dataDocument, dataOperare, valoareTotala, TVA, scadenta, stareIncasare, codClient) VALUES
('FC', '003', '2026-01-15', '2026-01-15', 2380, 380,  '2026-02-15', 'Neincasat', 1),
('FC', '004', '2026-02-01', '2026-02-01', 5950, 950,  '2026-03-01', 'Neincasat', 1),
('FC', '005', '2026-02-20', '2026-02-20', 1190, 190,  '2026-03-20', 'Neincasat', 1),
('FC', '006', '2026-03-05', '2026-03-05', 3570, 570,  '2026-04-05', 'Neincasat', 1);

-- Facturi client — pentru Beta SA (codClient=2)
INSERT INTO FacturaClient (serie, numar, dataDocument, dataOperare, valoareTotala, TVA, scadenta, stareIncasare, codClient) VALUES
('FC', '007', '2026-01-10', '2026-01-10', 4760, 760,  '2026-02-10', 'Neincasat', 2),
('FC', '008', '2026-02-15', '2026-02-15', 7140, 1140, '2026-03-15', 'Neincasat', 2);

-- Facturi client — pentru Gamma SRL (codClient=3)
INSERT INTO FacturaClient (serie, numar, dataDocument, dataOperare, valoareTotala, TVA, scadenta, stareIncasare, codClient) VALUES
('FC', '009', '2026-02-01', '2026-02-01', 2800, 448,  '2026-03-01', 'Neincasat', 3),
('FC', '010', '2026-03-10', '2026-03-10', 1680, 268,  '2026-04-10', 'Neincasat', 3);

-- Facturi furnizor — pentru Dist Nord SRL (codFurnizor=1)
INSERT INTO FacturaFurnizor (serie, numar, dataDocument, dataOperare, valoareTotala, TVA, scadenta, stare, codFurnizor) VALUES
('FF', '102', '2026-01-20', '2026-01-20', 4760, 760,  '2026-02-20', 'Neplatit', 1),
('FF', '103', '2026-02-10', '2026-02-10', 2380, 380,  '2026-03-10', 'Neplatit', 1),
('FF', '104', '2026-03-01', '2026-03-01', 6545, 1045, '2026-04-01', 'Neplatit', 1);

-- Facturi furnizor — pentru Prod Sud SA (codFurnizor=2)
INSERT INTO FacturaFurnizor (serie, numar, dataDocument, dataOperare, valoareTotala, TVA, scadenta, stare, codFurnizor) VALUES
('FF', '201', '2026-01-15', '2026-01-15', 3570, 570,  '2026-02-15', 'Neplatit', 2),
('FF', '202', '2026-02-20', '2026-02-20', 8330, 1330, '2026-03-20', 'Neplatit', 2),
('FF', '203', '2026-03-05', '2026-03-05', 1190, 190,  '2026-04-05', 'Neplatit', 2);

-- Facturi furnizor — pentru Trans Est SRL (codFurnizor=3)
INSERT INTO FacturaFurnizor (serie, numar, dataDocument, dataOperare, valoareTotala, TVA, scadenta, stare, codFurnizor) VALUES
('FF', '301', '2026-02-01', '2026-02-01', 4165, 665,  '2026-03-01', 'Neplatit', 3),
('FF', '302', '2026-03-10', '2026-03-10', 2975, 475,  '2026-04-10', 'Neplatit', 3);