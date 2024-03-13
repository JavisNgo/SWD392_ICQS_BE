USE ICQS_Db
GO

-- Accounts
INSERT INTO Accounts (Username, Password, Role, Status)
VALUES
	('admin', '4dff4ea340f0a823f15d3f4f01ab62eae0e5da579ccb851f8db9dfe84c58b2b37b89903a740e1ee172da793a6e79d560e5f7f9bd058a12a280433ed6fa46510a', 0, 1),
	('contractor', '4dff4ea340f0a823f15d3f4f01ab62eae0e5da579ccb851f8db9dfe84c58b2b37b89903a740e1ee172da793a6e79d560e5f7f9bd058a12a280433ed6fa46510a', 1, 1),
	('customer', '4dff4ea340f0a823f15d3f4f01ab62eae0e5da579ccb851f8db9dfe84c58b2b37b89903a740e1ee172da793a6e79d560e5f7f9bd058a12a280433ed6fa46510a', 2, 1),
    ('contractor2', '4dff4ea340f0a823f15d3f4f01ab62eae0e5da579ccb851f8db9dfe84c58b2b37b89903a740e1ee172da793a6e79d560e5f7f9bd058a12a280433ed6fa46510a', 1, 1),
    ('contractor3', '4dff4ea340f0a823f15d3f4f01ab62eae0e5da579ccb851f8db9dfe84c58b2b37b89903a740e1ee172da793a6e79d560e5f7f9bd058a12a280433ed6fa46510a', 1, 1),
    ('contractor4', '4dff4ea340f0a823f15d3f4f01ab62eae0e5da579ccb851f8db9dfe84c58b2b37b89903a740e1ee172da793a6e79d560e5f7f9bd058a12a280433ed6fa46510a', 1, 1),
    ('contractor5', '4dff4ea340f0a823f15d3f4f01ab62eae0e5da579ccb851f8db9dfe84c58b2b37b89903a740e1ee172da793a6e79d560e5f7f9bd058a12a280433ed6fa46510a', 1, 0),
    ('customer2', '4dff4ea340f0a823f15d3f4f01ab62eae0e5da579ccb851f8db9dfe84c58b2b37b89903a740e1ee172da793a6e79d560e5f7f9bd058a12a280433ed6fa46510a', 2, 1),
    ('customer3', '4dff4ea340f0a823f15d3f4f01ab62eae0e5da579ccb851f8db9dfe84c58b2b37b89903a740e1ee172da793a6e79d560e5f7f9bd058a12a280433ed6fa46510a', 2, 1),
    ('customer4', '4dff4ea340f0a823f15d3f4f01ab62eae0e5da579ccb851f8db9dfe84c58b2b37b89903a740e1ee172da793a6e79d560e5f7f9bd058a12a280433ed6fa46510a', 2, 1),
    ('customer5', '4dff4ea340f0a823f15d3f4f01ab62eae0e5da579ccb851f8db9dfe84c58b2b37b89903a740e1ee172da793a6e79d560e5f7f9bd058a12a280433ed6fa46510a', 2, 0);
GO

-- Subscriptions
INSERT INTO Subscriptions (Name, Description, Price, Duration, Status)
VALUES
    ('Subscription 1', 'Description for Subscription 1', 9.99, 30, 1),
    ('Subscription 2', 'Description for Subscription 2', 19.99, 30, 1),
    ('Subscription 3', 'Description for Subscription 3', 29.99, 30, 1);
GO

-- Contractors
INSERT INTO Contractors (AccountId, Name, Email, PhoneNumber, Address, SubscriptionId, ExpiredDate)
VALUES 
	((SELECT Id FROM Accounts WHERE Username = 'contractor'), 'Contractor', 'contractor@example.com', '123456789', 'Contractor Address', 1, DATEADD(day, 30, GETDATE())),
    ((SELECT Id FROM Accounts WHERE Username = 'contractor2'), 'Contractor 2', 'contractor2@example.com', '123456789', 'Contractor Address 2', 1, DATEADD(day, 30, GETDATE())),
    ((SELECT Id FROM Accounts WHERE Username = 'contractor3'), 'Contractor 3', 'contractor3@example.com', '123456789', 'Contractor Address 3', 1, DATEADD(day, 30, GETDATE())),
    ((SELECT Id FROM Accounts WHERE Username = 'contractor4'), 'Contractor 4', 'contractor4@example.com', '123456789', 'Contractor Address 4', 1, DATEADD(day, 30, GETDATE())),
	((SELECT Id FROM Accounts WHERE Username = 'contractor5'), 'Contractor 5', 'contractor5@example.com', '123456789', 'Contractor Address 5', 1, DATEADD(day, 30, GETDATE()));
GO

-- Customers
INSERT INTO Customers (AccountId, Name, Email, Address, PhoneNumber)
VALUES 
	((SELECT Id FROM Accounts WHERE Username = 'customer'), 'Customer', 'customer@example.com', 'Customer Address', '123456789'),
    ((SELECT Id FROM Accounts WHERE Username = 'customer2'), 'Customer 2', 'customer2@example.com', 'Customer Address 2', '123456789'),
    ((SELECT Id FROM Accounts WHERE Username = 'customer3'), 'Customer 3', 'customer3@example.com', 'Customer Address 3', '123456789'),
    ((SELECT Id FROM Accounts WHERE Username = 'customer4'), 'Customer 4', 'customer4@example.com', 'Customer Address 4', '123456789'),
    ((SELECT Id FROM Accounts WHERE Username = 'customer5'), 'Customer 5', 'customer5@example.com', 'Customer Address 5', '123456789');
GO

-- Messages
INSERT INTO Messages (CustomerId, ContractorId, Content, SendAt, Status)
VALUES 
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Message content 1', GETDATE(), 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Message content 2', GETDATE(), 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Message content 3', GETDATE(), 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Message content 4', GETDATE(), 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Message content 5', GETDATE(), 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Message content 6', GETDATE(), 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Message content 7', GETDATE(), 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Message content 8', GETDATE(), 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Message content 9', GETDATE(), 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Message content 10', GETDATE(), 1);
GO

-- Blogs
INSERT INTO Blogs (Code, ContractorId, Title, Content, PostTime, EditTime, Status)
VALUES
    ('B-1-2435476451', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 1', 'Content of Blog 1', GETDATE(), GETDATE(), 1),
    ('B-1-1243524611', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 2', 'Content of Blog 2', GETDATE(), GETDATE(), 1),
    ('B-1-5314132121', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 3', 'Content of Blog 3', GETDATE(), GETDATE(), 1),
    ('B-1-7645374356', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 4', 'Content of Blog 4', GETDATE(), GETDATE(), 1),
    ('B-1-6523454352', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 5', 'Content of Blog 5', GETDATE(), GETDATE(), 1),
    ('B-1-8465725435', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 6', 'Content of Blog 6', GETDATE(), GETDATE(), 1),
    ('B-1-2453253456', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 7', 'Content of Blog 7', GETDATE(), GETDATE(), 1),
    ('B-1-9867567456', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 8', 'Content of Blog 8', GETDATE(), GETDATE(), 1),
    ('B-1-9867567456', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 9', 'Content of Blog 9', GETDATE(), GETDATE(), 1),
    ('B-1-4573645235', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 10', 'Content of Blog 10', GETDATE(), GETDATE(), 1);
GO

-- Orders
INSERT INTO Orders (SubscriptionId, ContractorId, OrderPrice, OrderDate, Status, TransactionCode)
VALUES
    ((SELECT Id FROM Subscriptions WHERE Name = 'Subscription 1'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 9.99, GETDATE(), 1, 'TransactionCode1'),
    ((SELECT Id FROM Subscriptions WHERE Name = 'Subscription 1'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 9.99, GETDATE(), 1, 'TransactionCode2'),
    ((SELECT Id FROM Subscriptions WHERE Name = 'Subscription 1'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 9.99, GETDATE(), 1, 'TransactionCode3');
GO

-- Products
INSERT INTO Products (Code, ContractorId, Name, Description, Price, Status)
VALUES
    ('P-1-2435476451', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Product 1', 'Description of Product 1', 10.99, 1),
    ('P-1-6435543132', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Product 2', 'Description of Product 2', 20.99, 1),
    ('P-1-7354253654', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Product 3', 'Description of Product 3', 30.99, 1),
    ('P-1-1545246355', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Product 4', 'Description of Product 4', 40.99, 1),
    ('P-1-7542645441', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Product 5', 'Description of Product 5', 50.99, 1),
    ('P-1-7532542435', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Product 6', 'Description of Product 6', 60.99, 1),
    ('P-1-3754863221', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Product 7', 'Description of Product 7', 70.99, 1),
    ('P-1-7633432662', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Product 8', 'Description of Product 8', 80.99, 1),
    ('P-1-1752473554', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Product 9', 'Description of Product 9', 90.99, 1),
    ('P-1-9746654622', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Product 10', 'Description of Product 10', 100.99, 1);
GO

-- Categories
INSERT INTO Categories (Name)
VALUES ('Category 1'),
       ('Category 2'),
       ('Category 3');
GO

-- Constructs
INSERT INTO Constructs (Code, ContractorId, CategoryId, Name, EstimatedPrice, Status)
VALUES
    ('C-1-2435476451', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), (SELECT Id FROM Categories WHERE Name = 'Category 1'), 'Construct Name 2', 500.00, 1),
    ('C-1-6875463452', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), (SELECT Id FROM Categories WHERE Name = 'Category 2'), 'Construct Name 2', 750.00, 1),
    ('C-1-6523454276', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), (SELECT Id FROM Categories WHERE Name = 'Category 3'), 'Construct Name 2', 1000.00, 1);
GO

-- ConstructProducts
INSERT INTO ConstructProducts (ConstructId, ProductId)
SELECT c.Id, p.Id
FROM Constructs c
CROSS JOIN (SELECT TOP 3 Id FROM Products WHERE ContractorId = (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com') ORDER BY NEWID()) p;
GO

-- Requests
INSERT INTO Requests (CustomerId, ContractorId, Note, TotalPrice, TimeIn, TimeOut, Status)
VALUES
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Request 1 note', 100.00, GETDATE(), DATEADD(day, 7, GETDATE()), 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Request 2 note', 150.00, GETDATE(), DATEADD(day, 7, GETDATE()), 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Request 3 note', 200.00, GETDATE(), DATEADD(day, 7, GETDATE()), 1);
GO

-- RequestDetails
INSERT INTO RequestDetails (RequestId, ProductId)
VALUES
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Product 1')),
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Product 2')),
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Product 3')),
    ((SELECT Id FROM Requests WHERE Note = 'Request 2 note'), (SELECT Id FROM Products WHERE Name = 'Product 4')),
    ((SELECT Id FROM Requests WHERE Note = 'Request 2 note'), (SELECT Id FROM Products WHERE Name = 'Product 5')),
    ((SELECT Id FROM Requests WHERE Note = 'Request 2 note'), (SELECT Id FROM Products WHERE Name = 'Product 6')),
    ((SELECT Id FROM Requests WHERE Note = 'Request 3 note'), (SELECT Id FROM Products WHERE Name = 'Product 7')),
    ((SELECT Id FROM Requests WHERE Note = 'Request 3 note'), (SELECT Id FROM Products WHERE Name = 'Product 8')),
    ((SELECT Id FROM Requests WHERE Note = 'Request 3 note'), (SELECT Id FROM Products WHERE Name = 'Product 9'));
GO

-- Appointments
INSERT INTO Appointments (CustomerId, ContractorId, RequestId, MeetingDate, Status)
VALUES
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), (SELECT Id FROM Requests WHERE Note = 'Request 1 note'), '2024-03-01 10:00:00', 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), (SELECT Id FROM Requests WHERE Note = 'Request 2 note'), '2024-03-02 11:00:00', 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), (SELECT Id FROM Requests WHERE Note = 'Request 3 note'), '2024-03-03 12:00:00', 1);
GO

-- Contracts
INSERT INTO Contracts (AppointmentId, ContractUrl, UploadDate, Status)
VALUES
    ((SELECT Id FROM Appointments WHERE RequestId = (SELECT Id FROM Requests WHERE Note = 'Request 1 note')), 'http://contract-url-1.com', '2024-03-01', 1),
    ((SELECT Id FROM Appointments WHERE RequestId = (SELECT Id FROM Requests WHERE Note = 'Request 2 note')), 'http://contract-url-2.com', '2024-03-02', 1),
    ((SELECT Id FROM Appointments WHERE RequestId = (SELECT Id FROM Requests WHERE Note = 'Request 3 note')), 'http://contract-url-3.com', '2024-03-03', 1);
GO