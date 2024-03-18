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

-- Contractors
INSERT INTO Contractors (AccountId, Name, Email, PhoneNumber, Address)
VALUES 
	((SELECT Id FROM Accounts WHERE Username = 'contractor'), 'Contractor', 'contractor@example.com', '123456789', 'Contractor Address'),
    ((SELECT Id FROM Accounts WHERE Username = 'contractor2'), 'Contractor 2', 'contractor2@example.com', '123456789', 'Contractor Address 2'),
    ((SELECT Id FROM Accounts WHERE Username = 'contractor3'), 'Contractor 3', 'contractor3@example.com', '123456789', 'Contractor Address 3'),
    ((SELECT Id FROM Accounts WHERE Username = 'contractor4'), 'Contractor 4', 'contractor4@example.com', '123456789', 'Contractor Address 4'),
	((SELECT Id FROM Accounts WHERE Username = 'contractor5'), 'Contractor 5', 'contractor5@example.com', '123456789', 'Contractor Address 5');
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
    ('B_1_2435476451', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 1', 'Content of Blog 1', GETDATE(), GETDATE(), 1),
    ('B_1_1243524611', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 2', 'Content of Blog 2', GETDATE(), GETDATE(), 1),
    ('B_1_5314132121', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 3', 'Content of Blog 3', GETDATE(), GETDATE(), 1),
    ('B_1_7645374356', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 4', 'Content of Blog 4', GETDATE(), GETDATE(), 1),
    ('B_1_6523454352', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 5', 'Content of Blog 5', GETDATE(), GETDATE(), 1),
    ('B_1_8465725435', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 6', 'Content of Blog 6', GETDATE(), GETDATE(), 1),
    ('B_1_2453253456', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 7', 'Content of Blog 7', GETDATE(), GETDATE(), 1),
    ('B_1_9867567456', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 8', 'Content of Blog 8', GETDATE(), GETDATE(), 1),
    ('B_1_9867567456', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 9', 'Content of Blog 9', GETDATE(), GETDATE(), 1),
    ('B_1_4573645235', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Title of Blog 10', 'Content of Blog 10', GETDATE(), GETDATE(), 1);
GO

-- Products
INSERT INTO Products (Code, ContractorId, Name, Description, Price, Status)
VALUES
--ContractorId 3
    ('P_3_J50ICJLJBX', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Ghế CS1898 Foyer vải S8Q', 'Bảo hành một năm cho các trường hợp có lỗi về kỹ thuật trong quá trình sản xuất hay lắp đặt. Sau thời gian hết hạn bảo hành, nếu quý khách có bất kỳ yêu cầu hay thắc mắc thì vui lòng liên hệ với chúng tôi.', 19900000, 1),
    ('P_3_KTCHM3ZQD4', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Sofa Rumba vải VACT 10784', 'Bảo hành một năm cho các trường hợp có lỗi về kỹ thuật trong quá trình sản xuất hay lắp đặt. Sau thời gian hết hạn bảo hành, nếu quý khách có bất kỳ yêu cầu hay thắc mắc thì vui lòng liên hệ với chúng tôi. Chúng tôi sẽ không bảo hành trong trường hợp sản phẩm được sử dụng không đúng quy cách của sổ bảo hành (được trao gửi khi quý khách mua sản phẩm) gây nên trầy xước, móp, dơ bẩn hay mất màu', 19900000, 1),
    ('P_3_VJWSNKMXA3', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Bàn ăn Breeze mặt kính bronze/GM2', 'Bảo hành ba năm cho các trường hợp có lỗi về kỹ thuật trong quá trình sản xuất hay lắp đặt. Sau thời gian hết hạn bảo hành, nếu quý khách có bất kỳ yêu cầu hay thắc mắc thì vui lòng liên hệ với chúng tôi. Chúng tôi sẽ không bảo hành trong trường hợp sản phẩm bị biến dạng do môi trường bên ngoài bất bình thường (quá ẩm, quá khô, mối hay do tác động từ các thiết bị điện nước, các hóa chất hay dung môi khách hàng sử dụng không phù hợp)', 207900000, 1),
    ('P_3_RPCZYF1PBM', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Giường Leman 1m8 vải VACT7459', 'Bảo hành hai năm cho các trường hợp có lỗi về kỹ thuật trong quá trình sản xuất hay lắp đặt. Sau thời gian hết hạn bảo hành, nếu quý khách có bất kỳ yêu cầu hay thắc mắc thì vui lòng liên hệ với chúng tôi. Chúng tôi sẽ không bảo hành trong trường hợp sản phẩm bị biến dạng do môi trường bên ngoài bất bình thường (quá ẩm, quá khô, mối hay do tác động từ các thiết bị điện nước, các hóa chất hay dung môi khách hàng sử dụng không phù hợp', 33650000, 1),
    ('P_3_TNS1Q3MJJL', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Nệm Luxury Golden Black 1m8', 'Nệm Luxury Golden Black với cấu tạo hàng triệu hạt gel được thấm vào nệm, được thiết kế để làm giảm nhiệt và giúp bạn mát mẻ suốt đêm. Các hạt cũng làm tăng mật độ của bọt làm cho nó thêm bền. Các lợi ích bổ sung bao gồm phục hồi nhanh hơn sau mệt mỏi cũng như cải thiện năng lượng khi thức dậy. Nệm Colmol được nhập khẩu từ Bồ Đào Nha- là thương hiệu nệm nổi tiếng từ năm 1972. Hệ thống lò xo SSI là viết tắt của hệ thống SUSPENSION độc lập với cuộn dây bỏ túi. Colmol có thể cuộn bất kỳ loại nệm nào - bao gồm lò xo túi - để tối ưu hóa chi phí vận chuyển giao hàng. Sử dụng các vật liệu hữu cơ tốt nhất, được kiểm soát, xác minh, và tất cả các quá trình đều được kiểm tra trong hệ thống kiểm soát chất lượng.', 51840000, 1),
    ('P_3_VWH1TRXJ0C', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Bình Aila Turquoise', 'Handblown from turquoise glass and embellished with arc-shaped decorations, Vase Aila is a perfect fit for a coffee table, console or empty shelf in your home. With its interesting silhouette and vibrant colour, this artistic vase will instantly elevate the space.', 12420000, 1)
	---------------------------------
GO
--Products Image
INSERT INTO ProductImages (ProductId,ImageUrl)
VALUES 
	((SELECT Id FROM Products WHERE Name = 'Ghế CS1898 Foyer vải S8Q'),'ProductImage_1_xUNCQD00YhkmS7O.png'),
	((SELECT Id FROM Products WHERE Name = 'Sofa Rumba vải VACT 10784'),'ProductImage_2_5h17YkHQvrV3IR7.png'),
	((SELECT Id FROM Products WHERE Name = 'Sofa Rumba vải VACT 10784'),'ProductImage_2_KMkSNtFWtsJ6t3J.png'),
	((SELECT Id FROM Products WHERE Name = 'Bàn ăn Breeze mặt kính bronze/GM2'),'ProductImage_3_hOisad1UFSzuyYm.png'),
	((SELECT Id FROM Products WHERE Name = 'Bàn ăn Breeze mặt kính bronze/GM2'),'ProductImage_3_dGlAHRMKKoQdDhe.png'),
	((SELECT Id FROM Products WHERE Name = 'Bàn ăn Breeze mặt kính bronze/GM2'),'ProductImage_3_aGfZjmDotmj7z3l.png'),
	((SELECT Id FROM Products WHERE Name = 'Giường Leman 1m8 vải VACT7459'),'ProductImage_4_Sa2FaPG7BjeErPg.png'),
	((SELECT Id FROM Products WHERE Name = 'Nệm Luxury Golden Black 1m8'),'ProductImage_5_oEUf6yXvhVfdLfy.png'),
	((SELECT Id FROM Products WHERE Name = 'Nệm Luxury Golden Black 1m8'),'ProductImage_5_h8eD6Z9OHhpwCMa.png'),
	((SELECT Id FROM Products WHERE Name = 'Nệm Luxury Golden Black 1m8'),'ProductImage_5_TZIGhTtjcThYhcf.png'),
	((SELECT Id FROM Products WHERE Name = 'Bình Aila Turquoise'),'ProductImage_6_0oWrkwUHRWxY2Xl.png'),
	((SELECT Id FROM Products WHERE Name = 'Bình Aila Turquoise'),'ProductImage_6_uLlOYZS3P8SyKzh.png')

GO
-- Categories
INSERT INTO Categories (Name)
VALUES ('Mid-century modern'),
       ('Industrial'),
	   ('Traditional'),
       ('Modern');
GO

-- Constructs
INSERT INTO Constructs (Code, ContractorId, CategoryId, Name, Description, EstimatedPrice, Status)
VALUES
    ('C_1_2435476451', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), (SELECT Id FROM Categories WHERE Name = 'Mid-century modern'), 'Construct Name 2', 'Construct Description 2', 500.00, 1),
    ('C_1_6875463452', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), (SELECT Id FROM Categories WHERE Name = 'Industrial'), 'Traditional', 'Construct Description 2', 750.00, 1),
    ('C_1_6523454276', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), (SELECT Id FROM Categories WHERE Name = 'Modern'), 'Industrial', 'Construct Description 2', 1000.00, 1);
GO

-- ConstructProducts
INSERT INTO ConstructProducts (ConstructId, ProductId, Quantity)
--SELECT c.Id, p.Id
--FROM Constructs c
--CROSS JOIN (SELECT TOP 3 Id FROM Products WHERE ContractorId = (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com') ORDER BY NEWID()) p;
VALUES
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Ghế CS1898 Foyer vải S8Q'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Sofa Rumba vải VACT 10784'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Bàn ăn Breeze mặt kính bronze/GM2'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Giường Leman 1m8 vải VACT7459'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Nệm Luxury Golden Black 1m8'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Bình Aila Turquoise'), 1)

GO

-- Requests
INSERT INTO Requests (CustomerId, ContractorId, Code, Note, TotalPrice, TimeIn, TimeOut, Status)
VALUES
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'R_1_1_2435476451', 'Request 1 note', 100.00, GETDATE(), DATEADD(day, 7, GETDATE()), 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'R_1_1_2435476451', 'Request 2 note', 150.00, GETDATE(), DATEADD(day, 7, GETDATE()), 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'R_1_1_2435476451', 'Request 3 note', 200.00, GETDATE(), DATEADD(day, 7, GETDATE()), 1);
GO

-- RequestDetails
INSERT INTO RequestDetails (RequestId, ProductId, Quantity)
VALUES
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Ghế CS1898 Foyer vải S8Q'), 1),
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Sofa Rumba vải VACT 10784'), 1),
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Bàn ăn Breeze mặt kính bronze/GM2'), 1),
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Giường Leman 1m8 vải VACT7459'), 1),
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Nệm Luxury Golden Black 1m8'), 1),
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Bình Aila Turquoise'), 1)
    
GO

-- Appointments
INSERT INTO Appointments (CustomerId, ContractorId, RequestId, MeetingDate, Status)
VALUES
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), (SELECT Id FROM Requests WHERE Note = 'Request 1 note'), '2024-03-01 10:00:00', 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), (SELECT Id FROM Requests WHERE Note = 'Request 2 note'), '2024-03-02 11:00:00', 1),
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), (SELECT Id FROM Requests WHERE Note = 'Request 3 note'), '2024-03-03 12:00:00', 1);
GO

-- Contracts
INSERT INTO Contracts (RequestId, ContractUrl, UploadDate, Status)
VALUES
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), 'http://contract-url-1.com', '2024-03-01', 1),
    ((SELECT Id FROM Requests WHERE Note = 'Request 2 note'), 'http://contract-url-2.com', '2024-03-02', 1),
    ((SELECT Id FROM Requests WHERE Note = 'Request 3 note'), 'http://contract-url-3.com', '2024-03-03', 1);
GO