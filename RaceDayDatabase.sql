
-- RaceDay Database PROG6212 Part 1


CREATE DATABASE RaceDayDB;
GO

USE RaceDayDB;
GO

--CREATING TABLES
-- ACCOUNT TABLE
CREATE TABLE Account
(
    AccountID INT IDENTITY(1,1) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(150) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    PhoneNumber VARCHAR(20),
    Role VARCHAR(20) NOT NULL,
    ProfileImageUrl VARCHAR(500),

    CONSTRAINT PK_Account
        PRIMARY KEY (AccountID),

    CONSTRAINT UQ_Account_Email
        UNIQUE (Email),

    CONSTRAINT CK_Account_Role
        CHECK (Role IN ('Organiser', 'Participant'))
);
GO



-- EVENT TABLE
CREATE TABLE Event
(
    EventID INT IDENTITY(1,1) NOT NULL,
    AccountID INT NOT NULL,
    EventName VARCHAR(150) NOT NULL,
    EventDescription VARCHAR(1000),
    EventDate DATE NOT NULL,
    Location VARCHAR(255) NOT NULL,
    EventType VARCHAR(10) NOT NULL,
    BannerImageUrl VARCHAR(500),

    CONSTRAINT PK_Event
        PRIMARY KEY (EventID),

    CONSTRAINT FK_Event_Account
        FOREIGN KEY (AccountID)
        REFERENCES Account(AccountID),

    CONSTRAINT CK_Event_EventType
        CHECK (EventType IN ('Run', 'Walk', 'Cycle'))
);
GO



-- CATEGORY TABLE
CREATE TABLE Category
(
    CategoryID INT IDENTITY(1,1) NOT NULL,
    EventID INT NOT NULL,
    CategoryName VARCHAR(100) NOT NULL,
    CategoryDescription VARCHAR(500),
    CategoryType VARCHAR(20) NOT NULL,
    MinAge INT,
    MaxAge INT,
    DistanceKm DECIMAL(6,2),

    CONSTRAINT PK_Category
        PRIMARY KEY (CategoryID),

    CONSTRAINT FK_Category_Event
        FOREIGN KEY (EventID)
        REFERENCES Event(EventID),

    CONSTRAINT CK_Category_Type
        CHECK (CategoryType IN ('Age', 'Distance')),

    CONSTRAINT CK_Category_Values
        CHECK
        (
            (
                CategoryType = 'Age'
                AND MinAge IS NOT NULL
                AND MaxAge IS NOT NULL
                AND MinAge >= 0
                AND MaxAge >= MinAge
                AND DistanceKm IS NULL
            )
            OR
            (
                CategoryType = 'Distance'
                AND DistanceKm IS NOT NULL
                AND DistanceKm > 0
                AND MinAge IS NULL
                AND MaxAge IS NULL
            )
        ),

    CONSTRAINT UQ_Category_Event_Name
        UNIQUE (EventID, CategoryName),

    CONSTRAINT UQ_Category_Event_CategoryID
        UNIQUE (EventID, CategoryID)
);
GO



-- ROUTE TABLE
CREATE TABLE Route
(
    RouteID INT IDENTITY(1,1) NOT NULL,
    EventID INT NOT NULL,
    RouteName VARCHAR(150) NOT NULL,
    RouteDescription VARCHAR(1000) NULL,
    RouteDistance DECIMAL(6,2) NOT NULL,
    MapUrl VARCHAR(500) NULL,

    CONSTRAINT PK_Route
        PRIMARY KEY (RouteID),

    CONSTRAINT FK_Route_Event
        FOREIGN KEY (EventID)
        REFERENCES Event(EventID),

    CONSTRAINT UQ_Route_Event
        UNIQUE (EventID),

    CONSTRAINT CK_Route_Distance
        CHECK (RouteDistance > 0)
);
GO


-- ENTRY TABLE
CREATE TABLE Entry
(
    EntryID INT IDENTITY(1,1) NOT NULL,
    AccountID INT NOT NULL,
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    EntryDate DATETIME2 NOT NULL
        CONSTRAINT DF_Entry_EntryDate DEFAULT SYSDATETIME(),
    EntryStatus VARCHAR(20) NOT NULL
        CONSTRAINT DF_Entry_EntryStatus DEFAULT 'Pending',

    CONSTRAINT PK_Entry
        PRIMARY KEY (EntryID),

    CONSTRAINT FK_Entry_Account
        FOREIGN KEY (AccountID)
        REFERENCES Account(AccountID),

    CONSTRAINT FK_Entry_Event
        FOREIGN KEY (EventID)
        REFERENCES Event(EventID),

    CONSTRAINT FK_Entry_Category
        FOREIGN KEY (EventID, CategoryID)
        REFERENCES Category(EventID, CategoryID),

    CONSTRAINT UQ_Entry_Account_Event
        UNIQUE (AccountID, EventID),

    CONSTRAINT CK_Entry_Status
        CHECK (EntryStatus IN ('Pending', 'Confirmed', 'Cancelled'))
);
GO


-- RESULT TABLE
CREATE TABLE Result
(
    ResultID INT IDENTITY(1,1) NOT NULL,
    EntryID INT NOT NULL,
    FinishTime TIME,
    FinishPosition INT,

    CONSTRAINT PK_Result
        PRIMARY KEY (ResultID),

    CONSTRAINT FK_Result_Entry
        FOREIGN KEY (EntryID)
        REFERENCES Entry(EntryID),

    CONSTRAINT UQ_Result_Entry
        UNIQUE (EntryID),

    CONSTRAINT CK_Result_Position
        CHECK (FinishPosition IS NULL OR FinishPosition > 0)
);
GO



-- INSERTING SAMPLE DATA

-- Accounts (3 Organisers and 3 Participants)
-- PasswordHash values are sample placeholders. Real passwords will be securely hashed in the application.

INSERT INTO Account
    (FirstName, LastName, Email, PasswordHash, PhoneNumber, Role, ProfileImageUrl)
VALUES
    ('Thabo', 'Mokoena', 'thabo@raceday.co.za',
     'SampleHash_Thabo', '0825551001', 'Organiser', NULL),

    ('Naledi', 'Dlamini', 'naledi@raceday.co.za',
     'SampleHash_Naledi', '0825551002', 'Organiser', NULL),

    ('Michael', 'Jacobs', 'michael@raceday.co.za',
     'SampleHash_Michael', '0825551003', 'Organiser', NULL),

    ('Lerato', 'Khumalo', 'lerato@gmail.com',
     'SampleHash_Lerato', '0715552001', 'Participant', NULL),

    ('Sipho', 'Ndlovu', 'sipho@email.com',
     'SampleHash_Sipho', '0715552002', 'Participant', NULL),

    ('Ayesha', 'Petersen', 'ayesha@icloud.com',
     'SampleHash_Ayesha', '0715552003', 'Participant', NULL);
GO


-- Events -4 sample events

INSERT INTO Event
    (AccountID, EventName, EventDescription,
     EventDate, Location, EventType, BannerImageUrl)
VALUES
    (1,
     'Cape Town Sunrise Run',
     'A scenic morning road-running event through Cape Town.',
     '2026-11-15',
     'Cape Town',
     'Run',
     NULL),

    (1,
     'Durban Beach Walk',
     'A community walking event along the Durban beachfront.',
     '2026-12-06',
     'Durban',
     'Walk',
     NULL),

    (2,
     'Johannesburg City Run',
     'An urban road-running event through Johannesburg.',
     '2027-01-17',
     'Johannesburg',
     'Run',
     NULL),

    (3,
     'Stellenbosch Cycle Challenge',
     'A cycling challenge through Stellenbosch and surrounding areas.',
     '2027-02-21',
     'Stellenbosch',
     'Cycle',
     NULL);
GO


-- Categories (Every event has categories)

INSERT INTO Category
    (EventID, CategoryName, CategoryDescription,
     CategoryType, MinAge, MaxAge, DistanceKm)
VALUES
    (1,
     '5 km Open',
     'Open five kilometre running category.',
     'Distance',
     NULL,
     NULL,
     5.00),

    (1,
     'Under 18',
     'Running category for participants aged 13 to 17.',
     'Age',
     13,
     17,
     NULL),

    (2,
     '10 km Open',
     'Open ten kilometre walking category.',
     'Distance',
     NULL,
     NULL,
     10.00),

    (2,
     'Senior Walkers',
     'Walking category for participants aged 60 and above.',
     'Age',
     60,
     100,
     NULL),

    (3,
     '10 km Open',
     'Open ten kilometre running category.',
     'Distance',
     NULL,
     NULL,
     10.00),

    (3,
     'Under 18',
     'Running category for participants aged 13 to 17.',
     'Age',
     13,
     17,
     NULL),

    (4,
     '21 km Cycle',
     'Twenty-one kilometre cycling category.',
     'Distance',
     NULL,
     NULL,
     21.00),

    (4,
     '50 km Cycle',
     'Fifty kilometre cycling category.',
     'Distance',
     NULL,
     NULL,
     50.00);
GO



-- Routes (One route per event)

INSERT INTO Route
    (EventID, RouteName, RouteDescription, RouteDistance, MapUrl)
VALUES
    (1,
     'Cape Town Sunrise Route',
     'Road-running route through central Cape Town.',
     10.00,
     'https://maps.example.com/cape-town-sunrise'),

    (2,
     'Durban Beachfront Route',
     'Walking route along the Durban beachfront.',
     10.00,
     'https://maps.example.com/durban-beach-walk'),

    (3,
     'Johannesburg City Route',
     'Urban running route through Johannesburg.',
     10.00,
     'https://maps.example.com/johannesburg-city-run'),

    (4,
     'Stellenbosch Cycle Route',
     'Cycling route through Stellenbosch.',
     50.00,
     'https://maps.example.com/stellenbosch-cycle');
GO


-- ---------------------------------------------------------
-- Entries
-- Participants select categories when entering events
-- ---------------------------------------------------------

INSERT INTO Entry
    (AccountID, EventID, CategoryID, EntryStatus)
VALUES
    (4, 1, 1, 'Confirmed'),
    (5, 1, 2, 'Pending'),
    (6, 2, 3, 'Confirmed'),
    (4, 3, 5, 'Confirmed'),
    (5, 4, 7, 'Confirmed'),
    (6, 4, 8, 'Pending');
GO



-- Results

INSERT INTO Result
    (EntryID, FinishTime, FinishPosition)
VALUES
    (1, '00:27:35', 3),
    (3, '01:24:10', 8),
    (4, '00:52:44', 5),
    (5, '01:08:20', 2);
GO


-- TEST QUERIES
-- Used to confirm that the tables and relationships work


-- Display all accounts
SELECT *
FROM Account;
GO


-- Display all events with their organisers
SELECT
    E.EventID,
    E.EventName,
    E.EventDate,
    E.Location,
    E.EventType,
    A.FirstName + ' ' + A.LastName AS Organiser
FROM Event E
INNER JOIN Account A
    ON E.AccountID = A.AccountID;
GO


-- Display categories for each event
SELECT
    E.EventName,
    C.CategoryName,
    C.CategoryType,
    C.MinAge,
    C.MaxAge,
    C.DistanceKm
FROM Category C
INNER JOIN Event E
    ON C.EventID = E.EventID
ORDER BY E.EventID, C.CategoryID;
GO


-- Display participant entries
SELECT
    EN.EntryID,
    A.FirstName + ' ' + A.LastName AS Participant,
    E.EventName,
    C.CategoryName,
    EN.EntryDate,
    EN.EntryStatus
FROM Entry EN
INNER JOIN Account A
    ON EN.AccountID = A.AccountID
INNER JOIN Event E
    ON EN.EventID = E.EventID
INNER JOIN Category C
    ON EN.CategoryID = C.CategoryID
ORDER BY EN.EntryID;
GO


-- Display event routes
SELECT
    E.EventName,
    R.RouteName,
    R.RouteDistance,
    R.MapUrl
FROM Route R
INNER JOIN Event E
    ON R.EventID = E.EventID;
GO


-- Display participant results
SELECT
    A.FirstName + ' ' + A.LastName AS Participant,
    E.EventName,
    C.CategoryName,
    R.FinishTime,
    R.FinishPosition
FROM Result R
INNER JOIN Entry EN
    ON R.EntryID = EN.EntryID
INNER JOIN Account A
    ON EN.AccountID = A.AccountID
INNER JOIN Event E
    ON EN.EventID = E.EventID
INNER JOIN Category C
    ON EN.CategoryID = C.CategoryID
ORDER BY E.EventName, R.FinishPosition;
GO


SELECT * FROM Account;
SELECT * FROM Event;
SELECT * FROM Category;
SELECT * FROM Route;
SELECT * FROM Entry;
SELECT * FROM Result;
GO