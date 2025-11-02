IF OBJECT_ID('dbo.Salaries', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Salaries;
END

IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Users;
END

CREATE TABLE Users(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    NationalNumber INT NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE Salaries(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Year INT NOT NULL,
    Month NVARCHAR(20) NOT NULL,
    Salary DECIMAL(10,2) NOT NULL,
    UserID INT NOT NULL
);

INSERT INTO Users (Username, NationalNumber, Email, Phone, IsActive) VALUES
('john.doe', 123456789, 'john.doe@company.com', '+962-79-1234567', 1),
('jane.smith', 987654321, 'jane.smith@company.com', '+962-79-2345678', 1),
('ahmed.hassan', 555666777, 'ahmed.hassan@company.com', '+962-79-3456789', 1),
('sara.ali', 111222333, 'sara.ali@company.com', '+962-79-4567890', 1),
('mike.johnson', 444555666, 'mike.johnson@company.com', '+962-79-5678901', 0),
('fatima.omar', 777888999, 'fatima.omar@company.com', '+962-79-6789012', 1),
('david.brown', 222333444, 'david.brown@company.com', '+962-79-7890123', 1),
('layla.mahmoud', 888999000, 'layla.mahmoud@company.com', '+962-79-8901234', 0),
('chris.wilson', '333444555', 'chris.wilson@company.com', '+962-79-9012345', 1),
('nour.salem', 666777888, 'nour.salem@company.com', '+962-79-0123456', 1);
INSERT INTO Salaries (Year, Month, Salary, UserID) VALUES
(2024, 'January', 450.00, 1),
(2024, 'February', 475.00, 1),
(2024, 'March', 500.00, 1),
(2024, 'April', 525.00, 1),
(2024, 'May', 550.00, 1),
(2024, 'June', 575.00, 1);

INSERT INTO Salaries (Year, Month, Salary, UserID) VALUES
(2024, 'January', 200.00, 2),
(2024, 'February', 220.00, 2),
(2024, 'March', 250.00, 2),
(2024, 'April', 280.00, 2),
(2024, 'May', 300.00, 2);

INSERT INTO Salaries (Year, Month, Salary, UserID) VALUES
(2024, 'January', 400.00, 3),
(2024, 'February', 400.00, 3),
(2024, 'March', 400.00, 3),
(2024, 'April', 400.00, 3),
(2024, 'May', 400.00, 3);

INSERT INTO Salaries (Year, Month, Salary, UserID) VALUES
(2024, 'January', 300.00, 4),
(2024, 'February', 350.00, 4),
(2024, 'March', 600.00, 4),
(2024, 'April', 450.00, 4),
(2024, 'May', 500.00, 4),
(2024, 'June', 400.00, 4),
(2024, 'July', 350.00, 4);

INSERT INTO Salaries (Year, Month, Salary, UserID) VALUES
(2024, 'January', 150.00, 5),
(2024, 'February', 175.00, 5),
(2024, 'March', 200.00, 5),
(2024, 'April', 180.00, 5),
(2024, 'May', 160.00, 5);

INSERT INTO Salaries (Year, Month, Salary, UserID) VALUES
(2024, 'January', 600.00, 6),
(2024, 'February', 650.00, 6),
(2024, 'March', 700.00, 6),
(2024, 'April', 750.00, 6),
(2024, 'May', 800.00, 6);

INSERT INTO Salaries (Year, Month, Salary, UserID) VALUES
(2024, 'January', 320.00, 7),
(2024, 'February', 340.00, 7),
(2024, 'March', 360.00, 7),
(2024, 'April', 380.00, 7),
(2024, 'May', 400.00, 7),
(2024, 'June', 420.00, 7);

INSERT INTO Salaries (Year, Month, Salary, UserID) VALUES
(2024, 'January', 250.00, 8),
(2024, 'February', 275.00, 8),
(2024, 'March', 300.00, 8),
(2024, 'April', 325.00, 8),
(2024, 'May', 350.00, 8);

INSERT INTO Salaries (Year, Month, Salary, UserID) VALUES
(2024, 'January', 280.00, 9),
(2024, 'February', 320.00, 9),
(2024, 'March', 380.00, 9),
(2024, 'April', 420.00, 9),
(2024, 'May', 480.00, 9),
(2024, 'June', 520.00, 9);

INSERT INTO Salaries (Year, Month, Salary, UserID) VALUES
(2024, 'January', 380.00, 10),
(2024, 'February', 390.00, 10),
(2024, 'March', 395.00, 10),
(2024, 'April', 405.00, 10),
(2024, 'May', 410.00, 10),
(2024, 'June', 420.00, 10);

CREATE NONCLUSTERED INDEX IX_Users_NationalNumber ON Users(NationalNumber);
CREATE NONCLUSTERED INDEX IX_Salaries_UserID ON Salaries(UserID);
CREATE NONCLUSTERED INDEX IX_Salaries_Year_Month ON Salaries(Year, Month);

GO
CREATE PROCEDURE sp_GetEmployeeByNationalNumber
    @NationalNumber INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ID,
        Username,
        NationalNumber,
        Email,
        Phone,
        IsActive
    FROM Users
    WHERE NationalNumber = @NationalNumber;
END;
GO
