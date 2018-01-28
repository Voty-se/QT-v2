
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/23/2018 21:54:34
-- Generated from EDMX file: C:\Users\Ben\Documents\Voty AB\Projects\AB\Customers Apps\QT v2\QT\QTransportModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [QTransportDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_BokingCustomer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BokingSet] DROP CONSTRAINT [FK_BokingCustomer];
GO
IF OBJECT_ID(N'[dbo].[FK_BokingUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BokingSet] DROP CONSTRAINT [FK_BokingUser];
GO
IF OBJECT_ID(N'[dbo].[FK_BokingUser1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BokingSet] DROP CONSTRAINT [FK_BokingUser1];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[UserSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserSet];
GO
IF OBJECT_ID(N'[dbo].[CarSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CarSet];
GO
IF OBJECT_ID(N'[dbo].[CustomerSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CustomerSet];
GO
IF OBJECT_ID(N'[dbo].[BokingSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BokingSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'UserSet'
CREATE TABLE [dbo].[UserSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Lastname] nvarchar(max)  NULL,
    [Username] nvarchar(max)  NOT NULL,
    [Email] nvarchar(max)  NULL,
    [Password] nvarchar(max)  NOT NULL,
    [Role] nvarchar(max)  NOT NULL,
    [Order] smallint  NOT NULL,
    [Active] bit  NOT NULL,
    [Token] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'CarSet'
CREATE TABLE [dbo].[CarSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RegNbr] nvarchar(max)  NOT NULL,
    [Model] nvarchar(max)  NULL,
    [Capacity] nvarchar(max)  NULL,
    [Range] nvarchar(max)  NULL
);
GO

-- Creating table 'CustomerSet'
CREATE TABLE [dbo].[CustomerSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL,
    [Lastname] nvarchar(max)  NULL,
    [PersonalNbr] nvarchar(max)  NULL,
    [Phone1] nvarchar(max)  NULL,
    [Phone2] nvarchar(max)  NULL,
    [Email] nvarchar(max)  NULL,
    [PortCode] nvarchar(max)  NULL,
    [Floor] nvarchar(max)  NULL,
    [Address1] nvarchar(max)  NOT NULL,
    [Address2] nvarchar(max)  NULL,
    [ZipCode] nvarchar(max)  NULL,
    [City] nvarchar(max)  NULL,
    [Country] nvarchar(max)  NULL
);
GO

-- Creating table 'BokingSet'
CREATE TABLE [dbo].[BokingSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [OrderNbr] nvarchar(max)  NOT NULL,
    [OrderAmount] decimal(18,0)  NOT NULL,
    [BookingDay] datetime  NOT NULL,
    [PartOfTheDay] smallint  NOT NULL,
    [DeliveryCost] nvarchar(max)  NOT NULL,
    [WayOfDelivery] nvarchar(max)  NOT NULL,
    [NbrItems] int  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [Distance] decimal(18,0)  NOT NULL,
    [Zone] nvarchar(max)  NOT NULL,
    [Pickup] bit  NOT NULL,
    [NbrItemsPickup] int  NOT NULL,
    [PayBySupplier] bit  NOT NULL,
    [Remarks] nvarchar(max)  NOT NULL,
    [Sms] bit  NOT NULL,
    [Payed] bit  NOT NULL,
    [Email] bit  NOT NULL,
    [Done] bit  NOT NULL,
    [Status] nvarchar(max)  NOT NULL,
    [Canceled] bit  NOT NULL,
    [Customer_Id] int  NOT NULL,
    [CreatedBy_Id] int  NOT NULL,
    [Car_Id] int  NOT NULL
);
GO

-- Creating table 'ProductSet'
CREATE TABLE [dbo].[ProductSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Price] nvarchar(max)  NOT NULL,
    [Quantity] nvarchar(max)  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [BokingProduct_Product_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'UserSet'
ALTER TABLE [dbo].[UserSet]
ADD CONSTRAINT [PK_UserSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CarSet'
ALTER TABLE [dbo].[CarSet]
ADD CONSTRAINT [PK_CarSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CustomerSet'
ALTER TABLE [dbo].[CustomerSet]
ADD CONSTRAINT [PK_CustomerSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BokingSet'
ALTER TABLE [dbo].[BokingSet]
ADD CONSTRAINT [PK_BokingSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProductSet'
ALTER TABLE [dbo].[ProductSet]
ADD CONSTRAINT [PK_ProductSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Customer_Id] in table 'BokingSet'
ALTER TABLE [dbo].[BokingSet]
ADD CONSTRAINT [FK_BokingCustomer]
    FOREIGN KEY ([Customer_Id])
    REFERENCES [dbo].[CustomerSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_BokingCustomer'
CREATE INDEX [IX_FK_BokingCustomer]
ON [dbo].[BokingSet]
    ([Customer_Id]);
GO

-- Creating foreign key on [CreatedBy_Id] in table 'BokingSet'
ALTER TABLE [dbo].[BokingSet]
ADD CONSTRAINT [FK_BokingUser]
    FOREIGN KEY ([CreatedBy_Id])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_BokingUser'
CREATE INDEX [IX_FK_BokingUser]
ON [dbo].[BokingSet]
    ([CreatedBy_Id]);
GO

-- Creating foreign key on [Car_Id] in table 'BokingSet'
ALTER TABLE [dbo].[BokingSet]
ADD CONSTRAINT [FK_BokingUser1]
    FOREIGN KEY ([Car_Id])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_BokingUser1'
CREATE INDEX [IX_FK_BokingUser1]
ON [dbo].[BokingSet]
    ([Car_Id]);
GO

-- Creating foreign key on [BokingProduct_Product_Id] in table 'ProductSet'
ALTER TABLE [dbo].[ProductSet]
ADD CONSTRAINT [FK_BokingProduct]
    FOREIGN KEY ([BokingProduct_Product_Id])
    REFERENCES [dbo].[BokingSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_BokingProduct'
CREATE INDEX [IX_FK_BokingProduct]
ON [dbo].[ProductSet]
    ([BokingProduct_Product_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------