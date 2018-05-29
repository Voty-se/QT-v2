


USE [QTransportDB];
GO


IF OBJECT_ID(N'[dbo].[FK_BokingProduct]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProductSet] DROP CONSTRAINT [FK_BokingProduct];
GO

  IF OBJECT_ID(N'[dbo].[ProductSet]', 'U') IS NOT NULL
      DROP TABLE [dbo].[ProductSet];
  GO

CREATE TABLE [dbo].[ProductSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Price] nvarchar(max)  NOT NULL,
    [Quantity] nvarchar(max)  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [BokingProduct_Product_Id] int  NOT NULL
);
GO

ALTER TABLE [dbo].[ProductSet]
ADD CONSTRAINT [PK_ProductSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO


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
