USE [master]
GO

/****** Object:  Database [EducomOpdrachtAPI]    Script Date: 21-6-2021 12:47:05 ******/
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [EducomOpdrachtAPI].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [EducomOpdrachtAPI] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET ARITHABORT OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET AUTO_CLOSE ON 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET  ENABLE_BROKER 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET READ_COMMITTED_SNAPSHOT ON 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET  MULTI_USER 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [EducomOpdrachtAPI] SET DB_CHAINING OFF 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [EducomOpdrachtAPI] SET QUERY_STORE = OFF
GO

USE [EducomOpdrachtAPI]
GO

ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO

ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO

ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO

ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO

ALTER DATABASE [EducomOpdrachtAPI] SET  READ_WRITE 
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Weerstations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StationId] [bigint] NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[Region] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[Temperature] [int] NOT NULL,
	[Humidity] [int] NOT NULL,
	[AirPressure] [int] NOT NULL,
 CONSTRAINT [PK_Weerstations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[Weerberichten](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[MaxTemperature] [int] NOT NULL,
	[MinTemperature] [int] NOT NULL,
	[RainChance] [int] NOT NULL,
	[SunChance] [int] NOT NULL,
 CONSTRAINT [PK_Weerberichten] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC master..sp_addsrvrolemember @loginame = N'NT AUTHORITY\SYSTEM', @rolename = N'sysadmin'
GO