INSERT INTO "gat-dev".public."Cfcs"("Id", "Name", "Cnpj", "Address", "Email", "CreationDate", "LastUpdateDate", "IsDeleted") VALUES
('e73c9f43-6c5d-4b52-8594-2d2ff2abf1b7', 'Autoescola Alfa', '12345678000199', 'Rua das Palmeiras, 123, São Paulo/SP', 'contato@autoalfa.com.br', '05-27-2025 12:00:00', '05-27-2025 12:00:00', false);

INSERT INTO "gat-dev".public."Users"("Id", "Name", "Cpf", "BirthDate", "Email", "RegistrationId", "Role", "CfcId", "CreationDate", "LastUpdateDate", "IsDeleted", "Password" ) VALUES
('8b7b8d7c-4f77-4c01-9c7e-6d2b70b66a70', 'João Silva', '12345678900', '1995-08-15', 'joao.silva@email.com', 'REG1234567', 1, 'e73c9f43-6c5d-4b52-8594-2d2ff2abf1b7', '05-27-2025 12:00:00', '05-27-2025 12:00:00', false, '123mudar');

INSERT INTO "gat-dev".public."Users"("Id", "Name", "Cpf", "BirthDate", "Email", "RegistrationId", "Role", "CfcId", "CreationDate", "LastUpdateDate", "IsDeleted", "Password" ) VALUES
('8b7b8d7c-4f77-4c01-9c7e-6d2b70b66a71', 'Nelso Jhonson', '12345678910', '1995-08-15', 'nelso.jhonson@email.com', 'REG1234567', 1, 'e73c9f43-6c5d-4b52-8594-2d2ff2abf1b7', '05-27-2025 12:00:00', '05-27-2025 12:00:00', false, '123mudar');

INSERT INTO "gat-dev".public."Schedules"("Id", "ScheduleDate", "UserId", "Done", "CreationDate", "LastUpdateDate", "IsDeleted") VALUES
('b88e9f5a-67a4-47c8-8d6e-3f73f29b6935', '2025-05-10 10:00:00', '8b7b8d7c-4f77-4c01-9c7e-6d2b70b66a70', false, '2025-04-26 12:00:00', '05-27-2025 12:00:00', false);

INSERT INTO "gat-dev".public."UsersProgress"("Id", "AulasTotais", "AulasMinimas", "UserId", "CreationDate", "LastUpdateDate", "IsDeleted") VALUES
('f3de7f2c-c5cb-4e30-b8dc-ff98c44674ab', 15, 10, '8b7b8d7c-4f77-4c01-9c7e-6d2b70b66a70', '05-27-2025 12:00:00', '05-27-2025 12:00:00', false);

