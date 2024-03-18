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

    ('P_3_J50ICJLJBX', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'CS1898 Foyer Fabric S8Q Chair', 'Warranty is provided for one year for technical issues arising during production or installation. After the warranty period expires, if you have any requests or inquiries, please feel free to contact us.', 19900000, 1),
    ('P_3_KTCHM3ZQD4', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Rumba Fabric VACT 10784 Sofa', 'Warranty is provided for one year for technical issues arising during production or installation. After the warranty period expires, if you have any requests or inquiries, please feel free to contact us. We will not provide warranty in cases where the product is not used according to the specifications outlined in the warranty booklet (provided to you upon purchase), resulting in scratches, dents, dirt, or color fading.', 19900000, 1),
    ('P_3_VJWSNKMXA3', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Breeze Dining Table with Bronze Glass Top/GM2', 'Warranty is provided for three years for technical issues arising during production or installation. After the warranty period expires, if you have any requests or inquiries, please feel free to contact us. We will not provide warranty in cases where the product is deformed due to abnormal external conditions (excessive humidity, excessive dryness, mold, or due to the impact of electrical and water appliances, chemicals, or solvents that customers use inappropriately).)', 207900000, 1),
    ('P_3_RPCZYF1PBM', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Leman 1.8m Bed with VACT7459 Fabric', 'Warranty is provided for two years for technical issues arising during production or installation. After the warranty period expires, if you have any requests or inquiries, please feel free to contact us. We will not provide warranty in cases where the product is deformed due to abnormal external conditions (excessive humidity, excessive dryness, mold, or due to the impact of electrical and water appliances, chemicals, or solvents that customers use inappropriately).', 33650000, 1),
    ('P_3_TNS1Q3MJJL', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Luxury Golden Black 1.8m Mattress', 'The Luxury Golden Black mattress features millions of gel beads infused into the mattress, designed to reduce heat and keep you cool throughout the night. These beads also enhance the density of the foam, making it more durable. Additional benefits include quicker recovery from fatigue and improved energy upon waking up. Colmol mattresses are imported from Portugal - a renowned mattress brand since 1972. The SSI coil system stands for SUSPENSION independent coil pocket system. Colmol can compress any type of mattress - including pocket coil mattresses - to optimize shipping costs. Utilizing the finest organic materials, all controlled and verified, with all processes checked within the quality control system.', 51840000, 1),
    ('P_3_VWH1TRXJ0C', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Aila Turquoise Pitcher', 'Handblown from turquoise glass and embellished with arc-shaped decorations, Vase Aila is a perfect fit for a coffee table, console or empty shelf in your home. With its interesting silhouette and vibrant colour, this artistic vase will instantly elevate the space.', 12420000, 1),
	('P_1_NMLUD089KW', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Jadora Swivel Armchair in Red Orange with Pattern, Includes Matching Pillow', 'The armchair features a soft and incredibly comfortable design, especially with its smooth 360-degree swivel capability, providing wonderful moments of relaxation.', 15900000, 1),
	('P_1_TOD7EC1N7L', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Hopper 38929P Work Desk', 'The Hopper 38929P Work Desk is an ideal choice for modern workspaces. With its simple yet contemporary design, this desk offers both convenience and aesthetic appeal to your working environment. The spacious desktop provides ample space for comfortable work while also accommodating necessary items. Constructed with sturdy and durable materials, the Hopper 38929P Work Desk can withstand daily use while maintaining its beauty and longevity. With its sleek design and elegant color, this desk will be the perfect focal point for your workspace.', 20000000, 1),
	('P_1_HQ9H3GDMNV', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Armchair Hung King', 'A masterpiece of comfort and style. With its luxurious design and exquisite craftsmanship, this armchair is sure to elevate any living space. Sink into the plush cushions and experience unparalleled relaxation as you unwind after a long day. The Hung King Armchair is not only a statement piece but also a symbol of refinement and sophistication. Whether used as a cozy reading nook or a stylish accent in your living room, this armchair is bound to impress guests and become your favorite spot to lounge. Treat yourself to the ultimate in comfort and elegance with the Armchair Hung King.', 13000000, 1),
	('P_1_7YX3FKBZJS', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Tita Gold Side Table', 'The Tita Gold Side Table exudes elegance and sophistication. Crafted with meticulous attention to detail, this side table features a stunning gold finish that adds a touch of luxury to any space. Its sleek design and compact size make it perfect for use as a bedside table, accent piece in the living room, or even as a stylish addition to a cozy reading nook. The sturdy construction ensures durability, while the sleek surface provides ample space for a lamp, books, or other essentials. Elevate your home décor with the timeless beauty of the Tita Gold Side Table.', 16000000, 1),
	('P_1_90HR2W5L2E', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Leman 1.8m Bed with VACT10370 Fabric', 'The Leman 1.8m Bed upholstered in VACT10370 fabric offers both style and comfort for your bedroom. With its modern yet timeless design, this bed is sure to elevate the aesthetic appeal of any bedroom decor. The VACT10370 fabric adds a touch of sophistication with its texture and color, creating a cozy atmosphere. The sturdy frame ensures durability and stability, providing you with a reliable and comfortable sleeping experience. Whether youre lounging or catching up on sleep, the Leman Bed offers the perfect blend of functionality and elegance for your bedroom sanctuary.', 33650000, 1),
	('P_1_Z7ZZRDZCMF', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Iris 1.6m Stone Drawer Bed', 'The Iris 1.6m Stone Drawer Bed combines practicality with modern style. Crafted with functionality in mind, this bed features convenient drawers for extra storage, perfect for keeping your bedroom organized and clutter-free. The stone color adds a touch of sophistication to any bedroom decor, while the sleek design enhances the overall aesthetic appeal. Made from high-quality materials, this bed offers durability and comfort for a restful nights sleep. Upgrade your bedroom with the Iris Drawer Bed and enjoy both style and functionality in one elegant piece of furniture.', 14630000, 1),
	('P_1_5K6THLYZBQ', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Kilian 1.8m Brown Leather Bed L01', 'The Kilian 1.8m Brown Leather Bed L01 exudes timeless elegance and luxurious comfort. Crafted with premium brown leather upholstery, this bed adds a touch of sophistication to any bedroom decor. The sleek and minimalist design enhances the aesthetic appeal while providing a cozy retreat for relaxation. The sturdy frame ensures durability and stability, while the plush headboard offers additional comfort and support. Whether youre lounging or sleeping, the Kilian Bed offers the perfect blend of style and comfort for your bedroom sanctuary.', 125400000, 1),
	('P_1_5Z3AK4SWPL', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Wynn 1.8m Mushroom Leather Bed', 'The Wynn 1.8m Mushroom Leather Bed epitomizes luxury and comfort. Upholstered in exquisite mushroom-colored leather, this bed adds a touch of sophistication to any bedroom. Its sleek design and premium materials create a cozy and inviting atmosphere, perfect for relaxation and restful sleep. The sturdy construction ensures durability and stability, while the padded headboard provides extra comfort and support. Transform your bedroom into a stylish sanctuary with the Wynn Leather Bed.', 117720000, 1),
	('P_1_G36RN99W36', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Victoria 1.8m Wooden Bed', 'The Victoria 1.8m Wooden Bed combines timeless elegance with sturdy craftsmanship. Crafted from high-quality wood, this bed features a classic design that adds a touch of sophistication to any bedroom. The rich wood finish enhances the natural beauty of the grain, creating a warm and inviting atmosphere. With its sturdy construction and durable materials, the Victoria Bed offers both style and reliability for a restful nights sleep. Transform your bedroom into a cozy retreat with the classic charm of the Victoria Wooden Bed', 33900000, 1),
	('P_1_DJZTS25DXC', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Bui 1.6m Wooden Bed', 'The Bui 1.6m Wooden Bed exudes rustic charm and timeless appeal. Crafted from high-quality wood, this bed features a simple yet elegant design that adds warmth to any bedroom decor. The natural wood finish highlights the beauty of the grain, creating a cozy and inviting atmosphere. With its sturdy construction and durable materials, the Bui Bed offers both style and reliability for a restful nights sleep. Transform your bedroom into a rustic retreat with the understated elegance of the Bui Wooden Bed.', 16300000, 1),
	('P_1_LPTK4FOW38', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Pio 1.8m Fabric Upholstered Bed in VACT6090/VACT3399', 'The Pio 1.8m Fabric Upholstered Bed in VACT6090/VACT3399 offers both style and comfort for your bedroom. Upholstered in high-quality fabric with a captivating pattern, this bed adds a touch of elegance to any space. The plush headboard provides a cozy spot for relaxation, while the sturdy frame ensures durability and stability. With its modern design and premium upholstery, the Pio Bed creates a luxurious and inviting atmosphere in your bedroom. Upgrade your sleeping experience with the comfort and sophistication of the Pio Fabric Upholstered Bed.', 40000000, 1),
	('P_1_CQ9WWQ2GPY', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Wardrobe 02', 'The Wardrobe 02 is a stylish and functional storage solution for any bedroom. With its sleek design and ample storage space, this wardrobe offers both practicality and elegance. The spacious interior provides plenty of room for hanging clothes, while the shelves and drawers allow for organized storage of accessories and other items. Crafted from high-quality materials, the Wardrobe 02 is built to last and will complement any bedroom decor seamlessly. Add convenience and sophistication to your bedroom with the Wardrobe 02.', 273930000, 1),
	('P_1_H9SUS2VB30', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Acrylic Wardrobe', 'The Acrylic Wardrobe is a modern and versatile storage solution for your bedroom. Crafted with sleek acrylic panels, this wardrobe adds a touch of contemporary elegance to any space. Its transparent design allows you to easily view and access your clothes and accessories, while the sturdy construction ensures durability. With ample storage space including hanging rods and shelves, the Acrylic Wardrobe provides functionality without compromising on style. Elevate your bedroom decor with the chic and practical Acrylic Wardrobe.', 32310000, 1),
	('P_1_IRJPCD2ZWW', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Maxine Wardrobe', 'The Maxine Wardrobe is a blend of style and functionality, perfect for organizing your bedroom. With its sleek design and ample storage space, this wardrobe offers both practicality and elegance. Featuring multiple compartments, including hanging rods, shelves, and drawers, it provides versatile storage options for your clothes, shoes, and accessories. Crafted from high-quality materials, the Maxine Wardrobe is built to last and will complement any bedroom decor seamlessly. Keep your bedroom organized and clutter-free with the Maxine Wardrobe.', 42420000, 1),
	('P_1_FDGF1E9RWU', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Opal Right Corner Sofa - Gray Leather', 'The Opal Right Corner Sofa in Gray Leather offers the perfect blend of style and comfort for your living space. With its sleek design and luxurious gray leather upholstery, this sofa adds a touch of sophistication to any room. The right corner configuration allows for versatile placement options, while the plush cushions provide exceptional comfort for lounging or entertaining guests. Crafted with high-quality materials and attention to detail, the Opal Sofa is built to last and will enhance the aesthetic appeal of your home. Elevate your living room with the modern elegance of the Opal Right Corner Sofa.', 101750000, 1),
	('P_1_V4YZXOUUFQ', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Limited Dura Left Corner Sofa with Brown Leather Pillow', 'The Limited Dura Left Corner Sofa with Brown Leather Pillow combines sophistication with comfort, making it the perfect centerpiece for any living space. Upholstered in high-quality fabric, this sofa exudes elegance and style, while the brown leather pillow adds a touch of luxury and contrast. The left corner configuration offers versatility in seating arrangements, making it perfect for lounging or entertaining guests. With its sturdy construction and plush cushions, the Limited Dura Sofa provides both style and comfort. Elevate your home with this exquisite sofa, which effortlessly combines modern design with timeless sophistication.', 161140000, 1),
	('P_1_21S6DGSFI7', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Bali 520 Right Corner Combo Leather Sofa', 'The Bali 520 Right Corner Combo Leather Sofa is a luxurious and stylish addition to any living space. Upholstered in high-quality leather, this sofa exudes sophistication and elegance. The right corner configuration offers versatile seating options, making it ideal for relaxing or entertaining guests. The plush cushions provide exceptional comfort, while the sturdy construction ensures durability for years to come. Elevate your home decor with the timeless appeal and modern design of the Bali 520 Right Corner Combo Leather Sofa.', 96900000, 1),
	('P_1_V6AH9DYU1F', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Metro Next Modern Left Corner Sofa in Gray Leather L16', 'The Metro Next Modern Left Corner Sofa in Gray Leather L16 is the epitome of contemporary elegance. Upholstered in high-quality gray leather, this sofa exudes sophistication and style. Its left corner configuration offers flexible seating arrangements, perfect for lounging or entertaining guests. The sleek design and clean lines add a modern touch to any living space, while the plush cushions ensure maximum comfort. Crafted with durability in mind, this sofa is built to last. Elevate your home decor with the Metro Next Modern Left Corner Sofa for a chic and inviting atmosphere.', 200580000, 1),
	('P_1_BXS9V63UAS', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Pio Left Corner Sofa in VACT6090/VACT3399 Fabric', 'The Pio Left Corner Sofa in VACT6090/VACT3399 Fabric offers both style and comfort for your living space. Upholstered in high-quality fabric with a captivating pattern, this sofa brings a touch of elegance to any room. The left corner configuration allows for versatile placement options, while the plush cushions provide exceptional comfort for lounging or entertaining guests. Crafted with attention to detail and durability in mind, the Pio Sofa is designed to enhance the aesthetic appeal of your home while providing a cozy seating experience. Elevate your living room with the chic and inviting design of the Pio Left Corner Sofa.', 36920000, 1),
	('P_1_9NZEE29NEK', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Modern Left Corner Milia Sofa', 'The Modern Left Corner Milia Sofa is a stylish and contemporary addition to any living space. With its sleek design and clean lines, this sofa exudes modern sophistication. The left corner configuration offers versatile seating options, perfect for lounging or entertaining guests. Upholstered in high-quality fabric, the Milia Sofa provides both comfort and durability. Its minimalist design and neutral color make it easy to complement any decor style. Elevate your home with the chic and inviting presence of the Modern Left Corner Milia Sofa.', 91490000, 1),
	('P_1_A26P4GG71A', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Swivel Jadora Armchair in Brown Green Pattern, Includes Matching Pillow', 'The armchair features a soft and incredibly comfortable design, especially with its smooth 360-degree swivel capability, providing wonderful moments of relaxation.', 16000000, 1),
	('P_1_II6XK8FC9Q', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Rudo TV Cabinet', 'The Rudo TV Cabinet combines functionality with modern design, making it the perfect addition to your living room or entertainment area. With its sleek and minimalist appearance, this TV cabinet seamlessly blends into any decor style. It features ample storage space for media equipment, DVDs, and other items, keeping your entertainment area organized and clutter-free. The sturdy construction ensures durability, while the clean lines and neutral color enhance the overall aesthetic appeal. Elevate your home entertainment experience with the practicality and elegance of the Rudo TV Cabinet.', 18900000, 1),
	('P_1_HVDQVTXV6F', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Elegance Black TV Cabinet', 'The Elegance Black TV Cabinet is a stylish and functional addition to any living space. With its sleek design and modern appearance, this TV cabinet effortlessly complements any decor style. The black color adds a touch of sophistication to your entertainment area, while the ample storage space provides convenience for organizing media equipment, DVDs, and other items. Crafted with high-quality materials, this TV cabinet is built to last and withstand daily use. Elevate your home entertainment setup with the sleek and practical design of the Elegance Black TV Cabinet.', 52040000, 1),
	('P_1_516BSHRVH9', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'Wooden Bridge TV Cabinet 1.8m - Natural Wood Color', 'The graceful curves and exquisite craftsmanship of the Bridge TV Cabinet create a perfect addition to any living room space. The Bridge TV Cabinet features drawers that slide open and closed seamlessly, providing convenient storage solutions. Crafted from solid oak wood, the product comes in two finishes, bright and subdued, reflecting the natural allure of wood. The Bridge collection embodies the essence of Scandinavian design, a seamless fusion of the renowned Danish designer Hans Sandgren Jakobsen and the meticulous craftsmanship of Japan. With its timeless design, the use of natural oak wood and leather, the Bridge collection exudes sophistication, warmth, and comfort for homeowners. The highlight of the Bridge lies in its meticulous finishing touches, from every detail to the curved lines, the oak wood surface is carefully crafted and selected to create a perfect Bridge, touching the soul with emotional resonance and cherishing the enduring values of Vietnamese homeowners.', 59000000, 1)

GO
--Products Image
INSERT INTO ProductImages (ProductId,ImageUrl)
VALUES 
	((SELECT Id FROM Products WHERE Products.Name = 'CS1898 Foyer Fabric S8Q Chair'),'ProductImage_1_xUNCQD00YhkmS7O.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Rumba Fabric VACT 10784 Sofa'),'ProductImage_2_5h17YkHQvrV3IR7.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Rumba Fabric VACT 10784 Sofa'),'ProductImage_2_KMkSNtFWtsJ6t3J.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Breeze Dining Table with Bronze Glass Top/GM2'),'ProductImage_3_hOisad1UFSzuyYm.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Breeze Dining Table with Bronze Glass Top/GM2'),'ProductImage_3_dGlAHRMKKoQdDhe.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Breeze Dining Table with Bronze Glass Top/GM2'),'ProductImage_3_aGfZjmDotmj7z3l.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Leman 1.8m Bed with VACT7459 Fabric'),'ProductImage_4_Sa2FaPG7BjeErPg.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Luxury Golden Black 1.8m Mattress'),'ProductImage_5_oEUf6yXvhVfdLfy.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Luxury Golden Black 1.8m Mattress'),'ProductImage_5_h8eD6Z9OHhpwCMa.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Luxury Golden Black 1.8m Mattress'),'ProductImage_5_TZIGhTtjcThYhcf.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Aila Turquoise Pitcher'),'ProductImage_6_0oWrkwUHRWxY2Xl.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Aila Turquoise Pitcher'),'ProductImage_6_uLlOYZS3P8SyKzh.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Jadora Swivel Armchair in Red Orange with Pattern, Includes Matching Pillow'),'ProductImage_7_XwjrbH8v8PnNHKv.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Hopper 38929P Work Desk'),'ProductImage_8_SE2VP8cdCR8Zgcm.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Armchair Hung King'),'ProductImage_9_MgaRPNvm1bFl26A.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Tita Gold Side Table'),'ProductImage_10_Huv0LwNTymF00m6.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Leman 1.8m Bed with VACT10370 Fabric'),'ProductImage_11_at6fVYO0yKoe3IN.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Iris 1.6m Stone Drawer Bed'),'ProductImage_12_OdebsSXSOW2cM5U.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Kilian 1.8m Brown Leather Bed L01'),'ProductImage_13_FNI1Cbr5LcxPEKO.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Wynn 1.8m Mushroom Leather Bed'),'ProductImage_14_2kqvH1r5nNCRSH9.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Victoria 1.8m Wooden Bed'),'ProductImage_15_wq427cXduq08hIl.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Bui 1.6m Wooden Bed'),'ProductImage_16_a6ktXC5eHFyOtGQ.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Pio 1.8m Fabric Upholstered Bed in VACT6090/VACT3399'),'ProductImage_17_5vYJr2FP7v3ajuq.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Wardrobe 02'),'ProductImage_18_KUyqnzT6tPXcRD6.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Acrylic Wardrobe'),'ProductImage_19_919QmHKt88GQkrT.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Maxine Wardrobe'),'ProductImage_20_6M4KxiARr3F6f9l.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Opal Right Corner Sofa - Gray Leather'),'ProductImage_21_yswJRlRRaD7Vh9b.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Limited Dura Left Corner Sofa with Brown Leather Pillow'),'ProductImage_23_IXD6N1eWYKJiaTq.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Bali 520 Right Corner Combo Leather Sofa'),'ProductImage_24_E0cAjH9bXkdJnye.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Metro Next Modern Left Corner Sofa in Gray Leather L16'),'ProductImage_25_hhl88Aj8SwMEPKX.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Pio Left Corner Sofa in VACT6090/VACT3399 Fabric'),'ProductImage_26_DQNNZpR2xJ30A8T.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Modern Left Corner Milia Sofa'),'ProductImage_27_lk5697ST8ZnIKF8.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Swivel Jadora Armchair in Brown Green Pattern, Includes Matching Pillow'),'ProductImage_28_xkaGENbo04rxk1R.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Rudo TV Cabinet'),'ProductImage_29_9MWlgopJc0bUGV3.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Elegance Black TV Cabinet'),'ProductImage_30_3irKp8n7SNOaEjd.png'),
	((SELECT Id FROM Products WHERE Products.Name = 'Wooden Bridge TV Cabinet 1.8m - Natural Wood Color'),'ProductImage_31_ax19OnPlpZK6st6.png')
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
    ('C_1_2435476451', (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), (SELECT Id FROM Categories WHERE Name = 'Mid-century modern'), 'Construct Name 2', 'Construct Description 2', 500.00, 1)
    
GO

-- ConstructProducts
INSERT INTO ConstructProducts (ConstructId, ProductId, Quantity)
--SELECT c.Id, p.Id
--FROM Constructs c
--CROSS JOIN (SELECT TOP 3 Id FROM Products WHERE ContractorId = (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com') ORDER BY NEWID()) p;
VALUES
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'CS1898 Foyer Fabric S8Q Chair'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Rumba Fabric VACT 10784 Sofa'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Breeze Dining Table with Bronze Glass Top/GM2'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Leman 1.8m Bed with VACT7459 Fabric'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Luxury Golden Black 1.8m Mattress'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Aila Turquoise Pitcher'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Jadora Swivel Armchair in Red Orange with Pattern, Includes Matching Pillow'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Hopper 38929P Work Desk'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Armchair Hung King'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Tita Gold Side Table'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Leman 1.8m Bed with VACT10370 Fabric'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Iris 1.6m Stone Drawer Bed'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Kilian 1.8m Brown Leather Bed L01'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Wynn 1.8m Mushroom Leather Bed'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Victoria 1.8m Wooden Bed'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Bui 1.6m Wooden Bed'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Pio 1.8m Fabric Upholstered Bed in VACT6090/VACT3399'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Wardrobe 02'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Acrylic Wardrobe'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Maxine Wardrobe'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Opal Right Corner Sofa - Gray Leather'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Limited Dura Left Corner Sofa with Brown Leather Pillow'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Bali 520 Right Corner Combo Leather Sofa'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Metro Next Modern Left Corner Sofa in Gray Leather L16'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Pio Left Corner Sofa in VACT6090/VACT3399 Fabric'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Modern Left Corner Milia Sofa'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Swivel Jadora Armchair in Brown Green Pattern, Includes Matching Pillow'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Rudo TV Cabinet'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Elegance Black TV Cabinet'), 1),
	((SELECT Id FROM Constructs Where Code = 'C_1_2435476451'), (SELECT Id FROM Products WHERE Name = 'Wooden Bridge TV Cabinet 1.8m - Natural Wood Color'), 1)

GO

-- Requests
INSERT INTO Requests (CustomerId, ContractorId, Code, Note, TotalPrice, TimeIn, TimeOut, Status)
VALUES
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), 'R_1_1_2435476451', 'Request 1 note', 100.00, GETDATE(), DATEADD(day, 7, GETDATE()), 1)
    
GO

-- RequestDetails
INSERT INTO RequestDetails (RequestId, ProductId, Quantity)
VALUES
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'CS1898 Foyer Fabric S8Q Chair'), 1),
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Rumba Fabric VACT 10784 Sofa'), 1),
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Breeze Dining Table with Bronze Glass Top/GM2'), 1),
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Leman 1.8m Bed with VACT7459 Fabric'), 1),
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Luxury Golden Black 1.8m Mattress'), 1),
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Aila Turquoise Pitcher'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Jadora Swivel Armchair in Red Orange with Pattern, Includes Matching Pillow'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Hopper 38929P Work Desk'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Armchair Hung King'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Tita Gold Side Table'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Leman 1.8m Bed with VACT10370 Fabric'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Iris 1.6m Stone Drawer Bed'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Kilian 1.8m Brown Leather Bed L01'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Wynn 1.8m Mushroom Leather Bed'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Victoria 1.8m Wooden Bed'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Bui 1.6m Wooden Bed'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Pio 1.8m Fabric Upholstered Bed in VACT6090/VACT3399'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Wardrobe 02'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Acrylic Wardrobe'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Maxine Wardrobe'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Opal Right Corner Sofa - Gray Leather'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Limited Dura Left Corner Sofa with Brown Leather Pillow'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Bali 520 Right Corner Combo Leather Sofa'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Metro Next Modern Left Corner Sofa in Gray Leather L16'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Pio Left Corner Sofa in VACT6090/VACT3399 Fabric'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Modern Left Corner Milia Sofa'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Swivel Jadora Armchair in Brown Green Pattern, Includes Matching Pillow'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Rudo TV Cabinet'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Elegance Black TV Cabinet'), 1),
	((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), (SELECT Id FROM Products WHERE Name = 'Wooden Bridge TV Cabinet 1.8m - Natural Wood Color'), 1)
    
GO

-- Appointments
INSERT INTO Appointments (CustomerId, ContractorId, RequestId, MeetingDate, Status)
VALUES
    ((SELECT Id FROM Customers WHERE Email = 'customer@example.com'), (SELECT Id FROM Contractors WHERE Email = 'contractor@example.com'), (SELECT Id FROM Requests WHERE Note = 'Request 1 note'), '2024-03-01 10:00:00', 1)

GO

-- Contracts
INSERT INTO Contracts (RequestId, ContractUrl, UploadDate, Status)
VALUES
    ((SELECT Id FROM Requests WHERE Note = 'Request 1 note'), 'http://contract-url-1.com', '2024-03-01', 1)
    
GO