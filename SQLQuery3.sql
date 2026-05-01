-- Facturi pentru Alpha SRL (codClient=1) — are deja FC-001, FC-002
INSERT INTO FacturaClient (serie, numar, dataDocument, dataOperare, valoareTotala, TVA, scadenta, stareIncasare, codClient) VALUES
('FC', '003', '2026-01-15', '2026-01-15', 2380, 380,  '2026-02-15', 'Neincasat', 1),
('FC', '004', '2026-02-01', '2026-02-01', 5950, 950,  '2026-03-01', 'Neincasat', 1),
('FC', '005', '2026-02-20', '2026-02-20', 1190, 190,  '2026-03-20', 'Neincasat', 1);

-- Facturi pentru Beta SA (codClient=2) — are deja FC-002... NU, FC-002 e Alpha
-- Beta SA nu are nicio factura inca
INSERT INTO FacturaClient (serie, numar, dataDocument, dataOperare, valoareTotala, TVA, scadenta, stareIncasare, codClient) VALUES
('FC', '006', '2026-01-10', '2026-01-10', 4760, 760,  '2026-02-10', 'Neincasat', 2),
('FC', '007', '2026-02-15', '2026-02-15', 7140, 1140, '2026-03-15', 'Neincasat', 2),
('FC', '008', '2026-03-01', '2026-03-01', 2800, 448,  '2026-04-01', 'Neincasat', 2);

-- Facturi pentru Dist Nord SRL (codFurnizor=1) — are deja FF-101
INSERT INTO FacturaFurnizor (serie, numar, dataDocument, dataOperare, valoareTotala, TVA, scadenta, stare, codFurnizor) VALUES
('FF', '102', '2026-01-20', '2026-01-20', 4760, 760,  '2026-02-20', 'Neplatit', 1),
('FF', '103', '2026-02-10', '2026-02-10', 2380, 380,  '2026-03-10', 'Neplatit', 1),
('FF', '104', '2026-03-01', '2026-03-01', 6545, 1045, '2026-04-01', 'Neplatit', 1);

-- Facturi pentru Prod Sud SA (codFurnizor=2) — are deja FF-102... NU, FF-102 e Dist Nord
-- Prod Sud nu are decat FF-102 din scriptul initial
INSERT INTO FacturaFurnizor (serie, numar, dataDocument, dataOperare, valoareTotala, TVA, scadenta, stare, codFurnizor) VALUES
('FF', '201', '2026-01-15', '2026-01-15', 3570, 570,  '2026-02-15', 'Neplatit', 2),
('FF', '202', '2026-02-20', '2026-02-20', 8330, 1330, '2026-03-20', 'Neplatit', 2),
('FF', '203', '2026-03-05', '2026-03-05', 1190, 190,  '2026-04-05', 'Neplatit', 2);