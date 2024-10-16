USE [master]
GO
/****** Object:  Database [FBT_DEV]    Script Date: 10/13/2024 10:43:32 AM ******/
CREATE DATABASE [FBT_DEV]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'FBT_DEV', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\FBT_DEV.mdf' , SIZE = 139264KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'FBT_DEV_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\FBT_DEV_log.ldf' , SIZE = 204800KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [FBT_DEV] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [FBT_DEV].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [FBT_DEV] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [FBT_DEV] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [FBT_DEV] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [FBT_DEV] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [FBT_DEV] SET ARITHABORT OFF 
GO
ALTER DATABASE [FBT_DEV] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [FBT_DEV] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [FBT_DEV] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [FBT_DEV] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [FBT_DEV] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [FBT_DEV] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [FBT_DEV] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [FBT_DEV] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [FBT_DEV] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [FBT_DEV] SET  ENABLE_BROKER 
GO
ALTER DATABASE [FBT_DEV] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [FBT_DEV] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [FBT_DEV] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [FBT_DEV] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [FBT_DEV] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [FBT_DEV] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [FBT_DEV] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [FBT_DEV] SET RECOVERY FULL 
GO
ALTER DATABASE [FBT_DEV] SET  MULTI_USER 
GO
ALTER DATABASE [FBT_DEV] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [FBT_DEV] SET DB_CHAINING OFF 
GO
ALTER DATABASE [FBT_DEV] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [FBT_DEV] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [FBT_DEV] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [FBT_DEV] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [FBT_DEV] SET QUERY_STORE = OFF
GO
USE [FBT_DEV]
GO
/****** Object:  Schema [authp]    Script Date: 10/13/2024 10:43:35 AM ******/
CREATE SCHEMA [authp]
GO
/****** Object:  Schema [wms]    Script Date: 10/13/2024 10:43:35 AM ******/
CREATE SCHEMA [wms]
GO
/****** Object:  Table [authp].[AuthUsers]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [authp].[AuthUsers](
	[UserId] [nvarchar](256) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[UserName] [nvarchar](128) NULL,
	[TenantId] [int] NULL,
	[ConcurrencyToken] [timestamp] NULL,
	[IsDisabled] [bit] NOT NULL,
 CONSTRAINT [PK_AuthUsers] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [authp].[RefreshTokens]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [authp].[RefreshTokens](
	[TokenValue] [varchar](50) NOT NULL,
	[UserId] [nvarchar](256) NOT NULL,
	[JwtId] [nvarchar](max) NULL,
	[IsInvalid] [bit] NOT NULL,
	[AddedDateUtc] [datetime2](7) NOT NULL,
	[ConcurrencyToken] [timestamp] NULL,
 CONSTRAINT [PK_RefreshTokens] PRIMARY KEY CLUSTERED 
(
	[TokenValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [authp].[RoleToPermissions]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [authp].[RoleToPermissions](
	[RoleName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[PackedPermissionsInRole] [nvarchar](max) NOT NULL,
	[ConcurrencyToken] [timestamp] NULL,
	[RoleType] [tinyint] NOT NULL,
 CONSTRAINT [PK_RoleToPermissions] PRIMARY KEY CLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [authp].[RoleToPermissionsTenant]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [authp].[RoleToPermissionsTenant](
	[TenantRolesRoleName] [nvarchar](100) NOT NULL,
	[TenantsTenantId] [int] NOT NULL,
	[ConcurrencyToken] [timestamp] NULL,
 CONSTRAINT [PK_RoleToPermissionsTenant] PRIMARY KEY CLUSTERED 
(
	[TenantRolesRoleName] ASC,
	[TenantsTenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [authp].[Tenants]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [authp].[Tenants](
	[TenantId] [int] IDENTITY(1,1) NOT NULL,
	[ParentDataKey] [varchar](250) NULL,
	[TenantFullName] [nvarchar](400) NOT NULL,
	[IsHierarchical] [bit] NOT NULL,
	[ParentTenantId] [int] NULL,
	[ConcurrencyToken] [timestamp] NULL,
	[DatabaseInfoName] [nvarchar](max) NULL,
	[HasOwnDb] [bit] NOT NULL,
 CONSTRAINT [PK_Tenants] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [authp].[UserToRoles]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [authp].[UserToRoles](
	[UserId] [nvarchar](256) NOT NULL,
	[RoleName] [nvarchar](100) NOT NULL,
	[ConcurrencyToken] [timestamp] NULL,
 CONSTRAINT [PK_UserToRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[__AppDbContextMigrationHistory]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__AppDbContextMigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___AppDbContextMigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[__AuthPermissionsMigrationHistory]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__AuthPermissionsMigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___AuthPermissionsMigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistoryWMS]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistoryWMS](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistoryWMS] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApiCodes]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApiCodes](
	[Code] [nvarchar](450) NOT NULL,
	[Message] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ApiCodes] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ArrivalInstructionDetails]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ArrivalInstructionDetails](
	[Id] [int] NOT NULL,
	[ScheduledArrivalNumber] [int] NULL,
	[ProductCode] [nvarchar](max) NULL,
	[ProductName] [nvarchar](max) NULL,
	[Quantity] [int] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[Status] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
 CONSTRAINT [PK_ArrivalInstructionDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ArrivalInstructions]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ArrivalInstructions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[ScheduledArrivalNumber] [int] NOT NULL,
	[ScheduledArrivalDate] [datetime2](7) NOT NULL,
	[ProductCode] [nvarchar](max) NULL,
	[ProductName] [nvarchar](max) NULL,
	[Quantity] [int] NOT NULL,
	[OrderNumber] [nvarchar](max) NULL,
	[OrderDate] [datetime2](7) NOT NULL,
	[SupplierId] [int] NOT NULL,
	[SupplierName] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ArrivalInstructions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BatchCalendars]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BatchCalendars](
	[Date] [varchar](10) NOT NULL,
	[ScheduleType] [varchar](1) NOT NULL,
 CONSTRAINT [PK_BatchCalendar] PRIMARY KEY CLUSTERED 
(
	[Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Batches]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Batches](
	[BatchId] [nvarchar](50) NOT NULL,
	[BatchName] [nvarchar](255) NOT NULL,
	[ExecFile] [nvarchar](255) NOT NULL,
	[StartupStatus] [nvarchar](50) NOT NULL,
	[Args] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](50) NOT NULL,
	[CreateAt] [datetime] NOT NULL,
	[UpdateOperatorId] [nvarchar](50) NULL,
	[UpdatedAt] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BatchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BatchSchedules]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BatchSchedules](
	[BatchScheduleId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [varchar](50) NULL,
	[BatchId] [varchar](50) NOT NULL,
	[StartTime] [time](7) NOT NULL,
	[EndTime] [time](7) NULL,
	[ScheduleDivision] [varchar](2) NOT NULL,
	[ScheduleTime] [time](7) NULL,
	[CreateOperatorId] [varchar](10) NULL,
	[CreateAt] [datetime] NOT NULL,
	[UpdateOperatorId] [varchar](10) NULL,
	[UpdatedAt] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[ScheduleType] [varchar](1) NULL,
	[DayOfWeek] [varchar](1) NULL,
 CONSTRAINT [PK_BacthSchedulers] PRIMARY KEY CLUSTERED 
(
	[BatchScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](max) NULL,
	[CompanyId] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChannelMasters]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChannelMasters](
	[ChannelMasterCode] [nvarchar](450) NOT NULL,
	[ChannelMasterName] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ChannelMasters] PRIMARY KEY CLUSTERED 
(
	[ChannelMasterCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Channels]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Channels](
	[ChannelCode] [nvarchar](450) NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[ChannelName] [nvarchar](max) NULL,
	[ChannelMasterCode] [nvarchar](450) NULL,
	[ChannelMasterName] [nvarchar](max) NULL,
	[PaymentMethod] [nvarchar](max) NULL,
	[BillCalcType] [nvarchar](max) NULL,
	[Currency] [nvarchar](max) NULL,
	[LanguageCode] [nvarchar](max) NULL,
	[IsActive] [bit] NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[DeliveryCompanyCode] [nvarchar](max) NULL,
 CONSTRAINT [PK_Channels] PRIMARY KEY CLUSTERED 
(
	[ChannelCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Companies]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Companies](
	[CompanyTenantId] [uniqueidentifier] NOT NULL,
	[FullName] [nvarchar](max) NULL,
	[ShortName] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[AuthPTenantId] [int] NOT NULL,
	[IsDeleted] [bit] NULL,
	[InventoryAlertEmail] [nvarchar](max) NULL,
 CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED 
(
	[CompanyTenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CountryMasters]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CountryMasters](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CountryIsoNumeric] [nvarchar](max) NULL,
	[CountryIso2] [nvarchar](max) NULL,
	[CountryIso3] [nvarchar](max) NULL,
	[CountryNameEn] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_CountryMasters] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Currencies]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Currencies](
	[CurrencyCode] [nvarchar](450) NOT NULL,
	[Country] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[IsDisplayCurrency] [bit] NOT NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Currencies] PRIMARY KEY CLUSTERED 
(
	[CurrencyCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CurrencyPairSettings]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CurrencyPairSettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CurrencyCodeFrom] [nvarchar](max) NULL,
	[CurrencyCodeTo] [nvarchar](max) NULL,
	[RateDecimalPlaces] [int] NOT NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_CurrencyPairSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExchangeRates]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExchangeRates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CurrencyCodeFrom] [nvarchar](max) NULL,
	[CurrencyCodeTo] [nvarchar](max) NULL,
	[Period] [nvarchar](max) NULL,
	[Rate] [float] NOT NULL,
	[AcquisitionDate] [datetime2](7) NOT NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ExchangeRates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HistoryOrderChanges]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HistoryOrderChanges](
	[HistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[OrderId] [varchar](50) NOT NULL,
	[HistoryNo] [int] NOT NULL,
	[ActivityLog] [nvarchar](2000) NULL,
	[OperatorId] [varchar](10) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_HistoryOrderChanges] PRIMARY KEY CLUSTERED 
(
	[HistoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LogTime]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogTime](
	[Id] [uniqueidentifier] NOT NULL,
	[LogName] [nvarchar](max) NULL,
	[EslapseTime] [int] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_LogTime] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDispatches]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDispatches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [varchar](10) NOT NULL,
	[DeliveryId] [varchar](50) NOT NULL,
	[MarketDeliveryNo] [varchar](50) NULL,
	[OrderId] [varchar](50) NULL,
	[TrackingNo] [varchar](50) NULL,
	[DeliveryCompanyCode] [varchar](50) NULL,
	[ShipmentDate] [datetime] NULL,
	[DispatchStatus] [varchar](2) NULL,
	[IsCutOff] [varchar](1) NULL,
	[CutoffDate] [datetime] NULL,
	[CutoffId] [varchar](200) NULL,
	[IsMarketShipped] [varchar](1) NULL,
	[CreateOperatorId] [varchar](10) NULL,
	[CreateAt] [datetime] NOT NULL,
	[UpdateOperatorId] [varchar](10) NULL,
	[UpdatedAt] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[DataKey] [varchar](10) NULL,
 CONSTRAINT [PK_OrderDispatches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDispatchProducts]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDispatchProducts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DispatchHeaderId] [int] NOT NULL,
	[CompanyId] [varchar](10) NOT NULL,
	[DeliveryId] [varchar](50) NOT NULL,
	[OrderId] [varchar](50) NULL,
	[LineNo] [int] NOT NULL,
	[ProductSku] [varchar](50) NOT NULL,
	[ItemName] [nvarchar](200) NULL,
	[ShippedQty] [int] NULL,
	[MarketShippedQty] [int] NULL,
	[Price] [decimal](12, 3) NULL,
	[DeclaredValue] [decimal](15, 3) NULL,
	[CreateOperatorId] [varchar](10) NULL,
	[CreateAt] [datetime] NOT NULL,
	[UpdateOperatorId] [varchar](10) NULL,
	[UpdatedAt] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[DataKey] [varchar](10) NULL,
 CONSTRAINT [PK_OrderDispatchProducts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItems]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderHeaderId] [int] NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[OrderId] [nvarchar](max) NULL,
	[LineNo] [int] NULL,
	[SalesProductCode] [nvarchar](max) NULL,
	[SalesProductName] [nvarchar](max) NULL,
	[Quantity] [int] NULL,
	[PurchaseUnitPrice] [float] NULL,
	[DeclaredValue] [float] NULL,
	[Currency] [nvarchar](max) NULL,
	[ReturnQuantity] [int] NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[ProductCode] [nvarchar](450) NULL,
	[ProductName] [nvarchar](450) NULL,
	[ParentItemCode] [nvarchar](450) NULL,
 CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderReturnItems]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderReturnItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderHeaderId] [int] NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[OrderId] [nvarchar](max) NULL,
	[SalesProductCode] [nvarchar](max) NULL,
	[SalesProductName] [nvarchar](max) NULL,
	[ReturnQuantity] [int] NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_OrderReturnItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[WarehouseCode] [nvarchar](max) NULL,
	[ChannelCode] [nvarchar](450) NULL,
	[OrderId] [nvarchar](max) NULL,
	[OrderStatus] [nvarchar](max) NULL,
	[OrderDate] [datetime2](7) NULL,
	[Total] [float] NULL,
	[SubTotal] [float] NULL,
	[HandlingCharge] [float] NULL,
	[Giftvoucher] [float] NULL,
	[Point] [float] NULL,
	[Shipping] [float] NULL,
	[CodCharge] [float] NULL,
	[DiscountAmount] [float] NULL,
	[OtherDiscount] [float] NULL,
	[TaxAmount] [float] NULL,
	[Currency] [nvarchar](max) NULL,
	[DeliveryCity] [nvarchar](max) NULL,
	[DeliveryState] [nvarchar](max) NULL,
	[DeliveryCountryCode] [nvarchar](max) NULL,
	[DeliveryCountryName] [nvarchar](max) NULL,
	[DeliveryMail] [nvarchar](max) NULL,
	[BillCity] [nvarchar](max) NULL,
	[BillState] [nvarchar](max) NULL,
	[BillCountry] [nvarchar](max) NULL,
	[BillMail] [nvarchar](max) NULL,
	[OrderDeliveryCompany] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[InternalRemarks] [nvarchar](max) NULL,
	[ReturnStatus] [nvarchar](max) NULL,
	[ShippedDate] [datetime2](7) NULL,
	[IsDeltaData] [bit] NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[OnHoldStatus] [int] NULL,
	[CustomerId] [nvarchar](20) NULL,
	[HoldJudgmentMemo] [nvarchar](max) NULL,
	[SubscriptionStatus] [int] NULL,
	[HadCheckAttachItem] [int] NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderStatuses]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderStatuses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StatusOrder] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [int] NOT NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_OrderStatuses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PackageAttachProducts]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PackageAttachProducts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PackageId] [int] NOT NULL,
	[DataKey] [varchar](250) NULL,
	[AttachProductCode] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Quantity] [int] NULL,
 CONSTRAINT [PK_PackageAttachProducts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Packages]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Packages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PackageName] [nvarchar](max) NULL,
	[OrderChannel] [nvarchar](max) NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NOT NULL,
	[ProductCode] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[CompanyId] [nvarchar](255) NULL,
 CONSTRAINT [PK_Packages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductBundles]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductBundles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ChildProductStatus] [int] NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[SaleProductBundleCode] [nvarchar](max) NULL,
	[ProductBundleCode] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[Quantity] [int] NOT NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[BundlePriceRatio] [int] NOT NULL,
	[EndTime] [datetime2](7) NULL,
	[StartTime] [datetime2](7) NULL,
 CONSTRAINT [PK_ProductBundles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductCategories]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
	[DataKey] [varchar](250) NULL,
 CONSTRAINT [PK_ProductCategories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductJanCodes]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductJanCodes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JanCode] [nvarchar](max) NULL,
	[JanDescription] [nvarchar](max) NULL,
	[ProductId] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_ProductJanCodes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[SupplierId] [int] NOT NULL,
	[SaleProductCode] [nvarchar](max) NULL,
	[Remark] [nvarchar](max) NULL,
	[ProductShortCode] [nvarchar](max) NULL,
	[SaleProductName] [nvarchar](max) NULL,
	[JanCode] [nvarchar](max) NULL,
	[HsCode] [nvarchar](max) NULL,
	[Weight] [float] NULL,
	[StockReceiptProcessInstruction] [nvarchar](max) NULL,
	[ProductType] [int] NOT NULL,
	[RegularPrice] [float] NULL,
	[Net] [float] NULL,
	[Currency] [nvarchar](max) NULL,
	[ProductModelNumber] [nvarchar](max) NULL,
	[ProductName] [nvarchar](max) NULL,
	[ProductStatus] [int] NOT NULL,
	[CountryOfOrigin] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[BaseCost] [float] NULL,
	[BaseCostOther] [float] NULL,
	[CurrencyCode] [nvarchar](max) NULL,
	[Depth] [float] NULL,
	[Description] [nvarchar](max) NULL,
	[Height] [float] NULL,
	[InventoryMethod] [nvarchar](max) NULL,
	[Length] [float] NULL,
	[CategoryId] [int] NOT NULL,
	[ProductCode] [nvarchar](max) NULL,
	[ProductEname] [nvarchar](max) NULL,
	[ProductImageName] [nvarchar](max) NULL,
	[ProductImageUrl] [nvarchar](max) NULL,
	[ProductIname] [nvarchar](max) NULL,
	[RegistrationDate] [datetime2](7) NULL,
	[ShippingLimitDays] [int] NOT NULL,
	[StockAvailableQuanitty] [int] NOT NULL,
	[UnitId] [int] NOT NULL,
	[VendorId] [int] NOT NULL,
	[StockThreshold] [int] NULL,
	[ToApplyPreBundles] [datetime2](7) NULL,
	[IndividuallyShippedItem] [bit] NULL,
	[VendorProductName] [nvarchar](max) NULL,
	[ProductShortName] [nvarchar](max) NULL,
	[MakerManagementCode] [nvarchar](max) NULL,
	[StandardPrice] [float] NULL,
	[ProductURL] [nvarchar](max) NULL,
	[WarehouseProcessingFlag] [bit] NULL,
	[TenantId] [int] NULL,
	[FromApplyPreBundles] [datetime2](7) NULL,
	[Width] [float] NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductStatuses]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductStatuses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StatusProduct] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ProductStatuses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductStocks]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductStocks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[WarehouseCode] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[StockQuantity] [int] NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[BinCode] [nvarchar](50) NULL,
	[Expried] [datetime2](7) NULL,
	[LOT] [nvarchar](50) NULL,
 CONSTRAINT [PK_ProductStocks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SalesData]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalesData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](max) NULL,
	[CompanyId] [nvarchar](max) NULL,
	[Channel] [nvarchar](max) NULL,
	[OrderId] [nvarchar](max) NULL,
	[OrderDate] [datetime2](7) NOT NULL,
	[OrderYear] [int] NOT NULL,
	[OrderQty] [int] NOT NULL,
	[CostOfSales] [float] NOT NULL,
	[Sales] [float] NOT NULL,
	[Profit] [float] NOT NULL,
	[Currency] [nvarchar](max) NULL,
	[ProductName] [nvarchar](max) NULL,
	[ProductCategory] [nvarchar](max) NULL,
	[State] [nvarchar](max) NULL,
	[City] [nvarchar](max) NULL,
	[Country] [nvarchar](max) NULL,
	[CustomerEmail] [nvarchar](max) NULL,
	[RepeatPurchase] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
 CONSTRAINT [PK_SalesData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ShippingCountries]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShippingCountries](
	[CountryCode] [nvarchar](450) NOT NULL,
	[CountryName] [nvarchar](max) NULL,
	[DisplayName] [nvarchar](max) NULL,
	[DisplayOrder] [int] NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ShippingCountries] PRIMARY KEY CLUSTERED 
(
	[CountryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockInOutItems]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockInOutItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[SeqNo] [int] NOT NULL,
	[ItemCD] [nvarchar](max) NULL,
	[Price] [float] NOT NULL,
	[Qty] [int] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_StockInOutItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockInOuts]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockInOuts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SeqNo] [int] NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[WarehouseCode] [nvarchar](max) NULL,
	[StockDate] [datetime2](7) NOT NULL,
	[LocationCD] [nvarchar](max) NULL,
	[InOutCD] [nvarchar](max) NULL,
	[StockCD] [nvarchar](max) NULL,
	[OperatorID] [nvarchar](max) NULL,
	[Comment] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_StockInOuts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockLocations]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockLocations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LocationCD] [nvarchar](max) NULL,
	[LocationName] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_StockLocations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockTypes]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StockCD] [nvarchar](max) NULL,
	[StockTypeName] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_StockTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Suppliers]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Suppliers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SupplierName] [nvarchar](max) NULL,
	[CompanyId] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[SupplierId] [nvarchar](max) NULL,
	[SupplierCode] [nvarchar](max) NULL,
	[HeadOfficeAddress] [nvarchar](max) NULL,
	[HeadOfficePhone] [nchar](10) NULL,
	[HeadOfficeFax] [nvarchar](max) NULL,
	[BillingAddress] [nvarchar](max) NULL,
	[BillingContactName] [nvarchar](max) NULL,
	[BillingPhone] [nvarchar](max) NULL,
	[BillingFax] [nvarchar](max) NULL,
	[BillingMail] [nvarchar](max) NULL,
	[BankName] [nvarchar](max) NULL,
	[BankBranch] [nvarchar](max) NULL,
	[BankAccountType] [nvarchar](max) NULL,
	[BankAccountNumber] [nvarchar](max) NULL,
	[BankAccountHolder] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[JamCharge] [nvarchar](max) NULL,
	[BillingDate] [date] NULL,
	[Status] [nvarchar](100) NULL,
	[TenantId] [int] NULL,
 CONSTRAINT [PK_Suppliers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemClassCompanies]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemClassCompanies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[LargeClassCode] [nvarchar](max) NULL,
	[SmallClassCode] [nvarchar](max) NULL,
	[LargeClassCodeName] [nvarchar](max) NULL,
	[SmallClassCodeName] [nvarchar](max) NULL,
	[Code1] [nvarchar](max) NULL,
	[Code2] [nvarchar](max) NULL,
	[Numeric1] [int] NULL,
	[Numeric2] [float] NULL,
	[Date1] [datetime2](7) NULL,
	[Date2] [datetime2](7) NULL,
	[Text1] [nvarchar](max) NULL,
	[Text2] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_SystemClassCompanies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemMaxPKNos]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemMaxPKNos](
	[ObjectCode] [varchar](15) NOT NULL,
	[MaxNo] [int] NOT NULL,
	[Digit] [int] NULL,
	[Remarks] [varchar](100) NULL,
	[CreateOperatorId] [varchar](10) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [varchar](10) NULL,
	[UpdatedAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_SystemMaxPKNos] PRIMARY KEY CLUSTERED 
(
	[ObjectCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskChatHistories]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskChatHistories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskId] [int] NOT NULL,
	[ChatContent] [nvarchar](max) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NOT NULL,
	[CreateAt] [datetime2](7) NOT NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[DataKey] [varchar](250) NULL,
	[IsModified] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tasks]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tasks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[Title] [nvarchar](max) NULL,
	[Status] [int] NULL,
	[Priority] [int] NULL,
	[Assignee] [nvarchar](max) NULL,
	[TaskType] [int] NULL,
	[TaskContent] [nvarchar](max) NULL,
	[FDALink] [nvarchar](max) NULL,
	[StartTime] [datetime2](7) NULL,
	[EndTime] [datetime2](7) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[DataKey] [varchar](250) NULL,
 CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskSchedules]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskSchedules](
	[ScheduleId] [bigint] IDENTITY(1,1) NOT NULL,
	[ScheduleDatetime] [datetime] NOT NULL,
	[CompanyId] [varchar](50) NULL,
	[MarketplaceCode] [varchar](5) NULL,
	[StartDatetime] [datetime] NULL,
	[FinishDatetime] [datetime] NULL,
	[BatchId] [varchar](500) NOT NULL,
	[Priority] [varchar](1) NULL,
	[IsFailed] [bit] NULL,
	[IsStopped] [bit] NULL,
	[RequestId] [varchar](100) NULL,
	[IsBatchInRequest] [bit] NULL,
	[CreateOperatorId] [varchar](10) NULL,
	[CreateAt] [datetime] NOT NULL,
	[UpdateOperatorId] [varchar](10) NULL,
	[UpdatedAt] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TempShopifyOrderProducts]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempShopifyOrderProducts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [varchar](100) NOT NULL,
	[OrderId] [varchar](100) NULL,
	[Name] [nvarchar](100) NULL,
	[Grams] [varchar](100) NULL,
	[Price] [varchar](100) NULL,
	[ProductId] [varchar](100) NULL,
	[Quantity] [varchar](100) NULL,
	[Sku] [nvarchar](100) NULL,
	[Title] [nvarchar](100) NULL,
	[TotalDiscount] [varchar](100) NULL,
	[VariantId] [varchar](100) NULL,
	[DataKey] [varchar](250) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TempShopifyOrders]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempShopifyOrders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [varchar](100) NOT NULL,
	[OrderId] [varchar](100) NULL,
	[OrderName] [nvarchar](100) NULL,
	[FinancialStatus] [varchar](100) NULL,
	[FulfillmentStatus] [varchar](100) NULL,
	[Gateway] [varchar](100) NULL,
	[LocationId] [varchar](100) NULL,
	[Currency] [varchar](100) NULL,
	[TotalPrice] [varchar](100) NULL,
	[SubtotalPrice] [varchar](100) NULL,
	[TotalDiscounts] [varchar](100) NULL,
	[TotalTax] [varchar](100) NULL,
	[Number] [varchar](100) NULL,
	[OrderNumber] [varchar](100) NULL,
	[PresentmentCurrency] [varchar](100) NULL,
	[TotalWeight] [varchar](100) NULL,
	[Created] [varchar](100) NULL,
	[Updated] [varchar](100) NULL,
	[UserId] [varchar](100) NULL,
	[Email] [varchar](100) NULL,
	[Phone] [varchar](100) NULL,
	[FirstName] [nvarchar](120) NULL,
	[LastName] [nvarchar](120) NULL,
	[Address1] [nvarchar](255) NULL,
	[Address2] [nvarchar](120) NULL,
	[Province] [nvarchar](120) NULL,
	[Country] [nvarchar](120) NULL,
	[City] [nvarchar](100) NULL,
	[Zip] [varchar](100) NULL,
	[CountryCode] [varchar](100) NULL,
	[ProvinceCode] [varchar](100) NULL,
	[Shipping] [varchar](100) NULL,
	[CustomerId] [varchar](100) NULL,
	[Note] [varchar](100) NULL,
	[DataKey] [varchar](250) NULL,
 CONSTRAINT [PK_TempShopifyOrders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ThirdPLWarehouses]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ThirdPLWarehouses](
	[WarehouseCode] [nvarchar](450) NOT NULL,
	[WarehouseName] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ThirdPLWarehouses] PRIMARY KEY CLUSTERED 
(
	[WarehouseCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserSettings]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSettings](
	[UserId] [nvarchar](450) NOT NULL,
	[CurrencyCode] [nvarchar](max) NULL,
	[PageLength] [int] NOT NULL,
	[DataKey] [varchar](250) NULL,
 CONSTRAINT [PK_UserSettings] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserTenants]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTenants](
	[id] [uniqueidentifier] NOT NULL,
	[UserId] [int] NULL,
	[Tenant] [int] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [nchar](10) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [nchar](10) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserVendors]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserVendors](
	[UserId] [nvarchar](450) NOT NULL,
	[VendorCode] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
 CONSTRAINT [PK_UserVendors] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VendorBilling]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VendorBilling](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[VendorCode] [nvarchar](max) NULL,
	[BillingPeriod] [nvarchar](max) NULL,
	[Total] [float] NULL,
	[DataKey] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [nvarchar](max) NULL,
 CONSTRAINT [PK_VendorBilling] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VendorBillingDetail]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VendorBillingDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VendorBillingHeaderId] [int] NOT NULL,
	[CompanyId] [nvarchar](max) NOT NULL,
	[VendorCode] [nvarchar](max) NULL,
	[BillingPeriod] [nvarchar](max) NULL,
	[ShippedDate] [datetime2](7) NULL,
	[OrderDate] [datetime2](7) NULL,
	[OrderId] [nvarchar](max) NULL,
	[SalesProductCode] [nvarchar](max) NULL,
	[CustomerProductCode] [nvarchar](max) NULL,
	[CHannelMasterCode] [nvarchar](max) NULL,
	[Currency] [nvarchar](max) NULL,
	[CurrencyRate] [float] NULL,
	[PurchaseUnitPrice] [float] NULL,
	[MallFee] [float] NULL,
	[DiscountAmount] [float] NULL,
	[JamFeeExcludingTax] [float] NULL,
	[JamFeeTaxAmount] [float] NULL,
	[MallShippingFee] [float] NULL,
	[ShippingFee] [float] NULL,
	[SubTotal] [float] NULL,
	[DataKey] [nvarchar](max) NULL,
	[SlsLgsShippingFee] [float] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [nvarchar](max) NULL,
 CONSTRAINT [PK_VendorBillingDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vendors]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vendors](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VendorCode] [nvarchar](max) NULL,
	[VendorName] [nvarchar](max) NULL,
	[VendorCompanyName] [nvarchar](max) NULL,
	[HeadOfficeAddress] [nvarchar](max) NULL,
	[HeadOfficePhone] [nvarchar](max) NULL,
	[HeadOfficeFax] [nvarchar](max) NULL,
	[BillingAddress] [nvarchar](max) NULL,
	[BillingContactName] [nvarchar](max) NULL,
	[BillingPhone] [nvarchar](max) NULL,
	[BillingFax] [nvarchar](max) NULL,
	[BillingMail] [nvarchar](max) NULL,
	[BankName] [nvarchar](max) NULL,
	[BankBranch] [nvarchar](max) NULL,
	[BankAccountType] [nvarchar](max) NULL,
	[BankAccountNumber] [nvarchar](max) NULL,
	[BankAccountHolder] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[JamCharge] [float] NULL,
	[BillingDate] [int] NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_Vendors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WarehouseUserSettings]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WarehouseUserSettings](
	[WarehouseUserId] [nvarchar](450) NOT NULL,
	[WarehouseCode] [nvarchar](max) NULL,
	[CompanyId] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_WarehouseUserSettings] PRIMARY KEY CLUSTERED 
(
	[WarehouseUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WkShopifyCategory]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WkShopifyCategory](
	[SeqNo] [int] IDENTITY(1,1) NOT NULL,
	[Category1] [nvarchar](max) NULL,
	[Category2] [nvarchar](max) NULL,
	[Category3] [nvarchar](max) NULL,
	[Category4] [nvarchar](max) NULL,
	[Category5] [nvarchar](max) NULL,
	[Category6] [nvarchar](max) NULL,
	[Category7] [nvarchar](max) NULL,
	[Category8] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[AspNetRoleClaims]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[AspNetRoleClaims](
	[Id] [int] NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[AspNetRoles]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[AspNetUserClaims]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[AspNetUserClaims](
	[Id] [int] NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[AspNetUserLogins]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[AspNetUserRoles]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [wms].[AspNetUsers]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[FullName] [nvarchar](max) NULL,
	[Localtion] [nvarchar](max) NULL,
	[Status] [int] NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[AspNetUserTokens]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[Batches]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[Batches](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductCode] [nvarchar](max) NOT NULL,
	[TenantId] [int] NOT NULL,
	[LotNo] [nvarchar](max) NOT NULL,
	[ManufacturingDate] [date] NULL,
	[ExpirationDate] [date] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_Batches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[Bins]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[Bins](
	[Id] [uniqueidentifier] NOT NULL,
	[LocationId] [uniqueidentifier] NOT NULL,
	[BinCode] [nvarchar](max) NOT NULL,
	[Remarks] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
	[LocationCD] [nvarchar](max) NULL,
	[LocationName] [nvarchar](500) NULL,
 CONSTRAINT [PK_Bins] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[Devices]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[Devices](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Type] [nvarchar](max) NULL,
	[Model] [nvarchar](max) NULL,
	[ActiveUser] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[OS] [nvarchar](max) NULL,
	[CPU] [nvarchar](max) NULL,
	[Memory] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_Devices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[Locations]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[Locations](
	[Id] [uniqueidentifier] NOT NULL,
	[LocationCD] [nvarchar](500) NULL,
	[LocationName] [nvarchar](max) NULL,
	[Abbreviation] [nvarchar](max) NULL,
	[Type] [nvarchar](100) NULL,
	[Email] [nvarchar](100) NULL,
	[Phone] [nvarchar](100) NULL,
	[Fax] [nvarchar](100) NULL,
	[Address] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[LogTime]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[LogTime](
	[Id] [uniqueidentifier] NOT NULL,
	[LogName] [nvarchar](max) NULL,
	[EslapseTime] [int] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_LogTime] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[MstUserSetting]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[MstUserSetting](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [nvarchar](max) NOT NULL,
	[Currency] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[NumberSequences]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[NumberSequences](
	[Id] [uniqueidentifier] NOT NULL,
	[JournalType] [nvarchar](50) NULL,
	[Prefix] [nvarchar](10) NULL,
	[SequenceLength] [int] NULL,
	[CurrentSequenceNo] [int] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_NumberSequences] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[Permissions]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[Permissions](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[PermissionsTenant]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[PermissionsTenant](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Desciption] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[RefreshTokens]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[RefreshTokens](
	[UserId] [nvarchar](500) NOT NULL,
	[Token] [nvarchar](max) NOT NULL,
	[RefreshToken] [nvarchar](450) NOT NULL,
	[ExpirationTime] [datetime] NOT NULL,
	[Activated] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[ReturnOrderLines]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[ReturnOrderLines](
	[Id] [uniqueidentifier] NOT NULL,
	[ReturnOrderNo] [nvarchar](max) NOT NULL,
	[Location] [nvarchar](max) NULL,
	[Qty] [float] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [nchar](10) NULL,
	[IsDeleted] [datetime] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_ReturnOrderLines] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[ReturnOrders]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[ReturnOrders](
	[Id] [uniqueidentifier] NOT NULL,
	[ReturnOrderNo] [nvarchar](max) NULL,
	[ShipmentNo] [nvarchar](max) NULL,
	[ReturnDate] [date] NULL,
	[Reason] [nvarchar](max) NULL,
	[PersonInCharge] [nvarchar](max) NULL,
	[ShipTo] [nvarchar](max) NULL,
	[ShipDate] [date] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_ReturnOrders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[RoleToPermission]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[RoleToPermission](
	[Id] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](max) NOT NULL,
	[PermissionId] [uniqueidentifier] NOT NULL,
	[PermisionName] [nvarchar](max) NULL,
	[PermisionDescription] [nvarchar](max) NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[RoleToPermissionTenant]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[RoleToPermissionTenant](
	[Id] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](max) NOT NULL,
	[PermissionTenantId] [uniqueidentifier] NOT NULL,
	[PermissionTenantName] [nvarchar](max) NULL,
	[PermissionTenantDesciption] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[ShippingBoxes]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[ShippingBoxes](
	[Id] [uniqueidentifier] NOT NULL,
	[BoxName] [nvarchar](max) NOT NULL,
	[BoxType] [nvarchar](max) NULL,
	[Length] [float] NULL,
	[Width] [float] NULL,
	[Height] [float] NULL,
	[MaxWeight] [float] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[Status] [int] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ShippingBoxes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[ShippingCarriers]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[ShippingCarriers](
	[Id] [uniqueidentifier] NOT NULL,
	[ShippingCarrierCode] [nvarchar](max) NOT NULL,
	[ShippingCarrierName] [nvarchar](max) NULL,
	[Status] [int] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ShippingCarrier] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[Units]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[Units](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UnitName] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_Units] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[UserToTenant]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[UserToTenant](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [nvarchar](max) NOT NULL,
	[TenantId] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehousePackingLines]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehousePackingLines](
	[Id] [uniqueidentifier] NOT NULL,
	[ShipmentNo] [nvarchar](max) NULL,
	[ProductNo] [nvarchar](max) NULL,
	[Unit] [nchar](10) NULL,
	[ShipmentOrderQty] [float] NULL,
	[PickedQty] [float] NULL,
	[PackedQty] [float] NULL,
	[Location] [nvarchar](max) NULL,
	[Bin] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_WarehousePackingLines] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehousePackingList]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehousePackingList](
	[Id] [uniqueidentifier] NOT NULL,
	[PackingNo] [nvarchar](max) NULL,
	[PackingDate] [date] NULL,
	[ShipmentNo] [nvarchar](max) NULL,
	[TenantId] [nchar](10) NULL,
	[Location] [nvarchar](max) NULL,
	[ShippingCarrier] [nvarchar](max) NULL,
	[ShippingAdress] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_WarehousePacks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehousePickingLines]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehousePickingLines](
	[Id] [uniqueidentifier] NOT NULL,
	[PickNo] [nvarchar](max) NOT NULL,
	[ProductCode] [nvarchar](max) NOT NULL,
	[Unit] [nchar](10) NULL,
	[Location] [nvarchar](max) NULL,
	[Bin] [nvarchar](max) NULL,
	[LotNo] [nvarchar](max) NULL,
	[PickQty] [float] NULL,
	[ActualQty] [float] NULL,
	[ShipmentNo] [nvarchar](max) NULL,
	[SalesNo] [nvarchar](max) NULL,
	[ShippingCarrierCode] [nvarchar](max) NULL,
	[TenantId] [int] NULL,
	[OrderDate] [date] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_WarehousePickLines] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehousePickingList]    Script Date: 10/13/2024 10:43:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehousePickingList](
	[Id] [uniqueidentifier] NOT NULL,
	[PickNo] [nvarchar](max) NOT NULL,
	[ShipmentNo] [nvarchar](max) NULL,
	[ShippingAgent] [nvarchar](max) NULL,
	[TenantId] [int] NULL,
	[SalesNo] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[PersonInCharge] [nvarchar](max) NULL,
	[PickedDate] [date] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [nchar](10) NULL,
	[IsDeleted] [datetime] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_WarehousePicks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehousePickingStaging]    Script Date: 10/13/2024 10:43:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehousePickingStaging](
	[Id] [uniqueidentifier] NOT NULL,
	[PickNo] [nvarchar](max) NOT NULL,
	[ProductCode] [nvarchar](max) NOT NULL,
	[Unit] [nchar](10) NULL,
	[Location] [nvarchar](max) NULL,
	[Bin] [nvarchar](max) NULL,
	[LotNo] [nvarchar](max) NULL,
	[PickQty] [float] NULL,
	[ActualQty] [float] NULL,
	[ShipmentNo] [nvarchar](max) NULL,
	[SalesNo] [nvarchar](max) NULL,
	[ShippingCarrierCode] [nvarchar](max) NULL,
	[TenantId] [int] NULL,
	[OrderDate] [date] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_WarehousePickStaging] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehousePutAwayLines]    Script Date: 10/13/2024 10:43:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehousePutAwayLines](
	[Id] [uniqueidentifier] NOT NULL,
	[PutawayNo] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[Unit] [nvarchar](max) NULL,
	[JournalQty] [float] NULL,
	[TransQty] [float] NULL,
	[Bin] [nvarchar](max) NULL,
	[Status] [int] NULL,
	[IsDeleted] [bit] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[LotNo] [nvarchar](50) NULL,
	[TenantId] [int] NULL,
	[ExpirationDate] [date] NULL,
 CONSTRAINT [PK_WarehousePutAwayLines] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehousePutAways]    Script Date: 10/13/2024 10:43:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehousePutAways](
	[Id] [uniqueidentifier] NOT NULL,
	[PutawayNo] [nvarchar](max) NOT NULL,
	[ReceiptNo] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[TenantId] [int] NOT NULL,
	[TransDate] [date] NOT NULL,
	[DocumentDate] [date] NULL,
	[DocumentNo] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[PostedDate] [date] NULL,
	[PostedBy] [nvarchar](max) NULL,
	[Status] [int] NULL,
	[IsDeleted] [bit] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
 CONSTRAINT [PK_WarehousePutAways] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehousePutAwayStaging]    Script Date: 10/13/2024 10:43:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehousePutAwayStaging](
	[Id] [uniqueidentifier] NOT NULL,
	[PutawayNo] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[Unit] [nvarchar](max) NULL,
	[JournalQty] [float] NULL,
	[TransQty] [float] NULL,
	[Bin] [nvarchar](max) NULL,
	[Status] [int] NULL,
	[IsDeleted] [bit] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[LotNo] [nvarchar](50) NULL,
	[ExpiryDate] [date] NULL,
	[PutawayLineId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_WarehousePutAwayStaging] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehouseReceiptOrder]    Script Date: 10/13/2024 10:43:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehouseReceiptOrder](
	[Id] [uniqueidentifier] NOT NULL,
	[ReceiptNo] [nvarchar](max) NOT NULL,
	[VendorCode] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NOT NULL,
	[ExpectedDate] [date] NULL,
	[ArrivalType] [nvarchar](50) NULL,
	[TenantId] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
	[PostedDate] [date] NULL,
	[PostedBy] [nvarchar](max) NULL,
	[ScheduledArrivalNumber] [int] NULL,
	[SupplierCode] [nvarchar](max) NULL,
	[PersonInCharge] [nvarchar](max) NULL,
	[ConfirmedBy] [nvarchar](max) NULL,
	[ConfirmedDate] [date] NULL,
	[DocumentNo] [nvarchar](max) NULL,
	[SupplierId] [int] NULL,
 CONSTRAINT [PK_WarehouseReceiptOrder] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehouseReceiptOrderLine]    Script Date: 10/13/2024 10:43:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehouseReceiptOrderLine](
	[Id] [uniqueidentifier] NOT NULL,
	[ReceiptNo] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NOT NULL,
	[UnitName] [nvarchar](max) NULL,
	[OrderQty] [float] NULL,
	[TransQty] [float] NULL,
	[Bin] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
	[LotNo] [nvarchar](50) NULL,
	[Putaway] [bit] NULL,
	[ExpirationDate] [date] NULL,
	[UnitId] [int] NOT NULL,
 CONSTRAINT [PK_WarehouseReceiptOrderLine] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehouseReceiptStaging]    Script Date: 10/13/2024 10:43:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehouseReceiptStaging](
	[id] [uniqueidentifier] NOT NULL,
	[ReceiptNo] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[OrderQty] [float] NULL,
	[TransQty] [float] NULL,
	[UnitId] [int] NOT NULL,
	[Bin] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[Status] [nvarchar](max) NULL,
	[LotNo] [nvarchar](50) NULL,
	[ExpirationDate] [date] NULL,
	[ReceiptLineId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_WarehouseReceiptStaging] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehouseShipmentLines]    Script Date: 10/13/2024 10:43:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehouseShipmentLines](
	[Id] [uniqueidentifier] NOT NULL,
	[ShipmentNo] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[Unit] [nchar](10) NULL,
	[ShipmentQty] [float] NULL,
	[Location] [nvarchar](max) NULL,
	[Bin] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_WarehouseShipmentLines] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehouseShipments]    Script Date: 10/13/2024 10:43:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehouseShipments](
	[Id] [uniqueidentifier] NOT NULL,
	[ShipmentNo] [nvarchar](max) NOT NULL,
	[SalesNo] [nvarchar](max) NULL,
	[TenantId] [int] NOT NULL,
	[Location] [nvarchar](max) NULL,
	[PlanShipDate] [nchar](10) NULL,
	[Status] [int] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[PersonInCharge] [nvarchar](max) NULL,
	[ShippingCarrierCode] [nvarchar](max) NULL,
	[ShippingAddress] [nvarchar](max) NULL,
	[Telephone] [nvarchar](max) NULL,
	[TrackingNo] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
 CONSTRAINT [PK_WarehouseShipments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehouseTrans]    Script Date: 10/13/2024 10:43:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehouseTrans](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductCode] [nvarchar](max) NOT NULL,
	[Qty] [float] NOT NULL,
	[DatePhysical] [date] NOT NULL,
	[TransType] [nvarchar](max) NOT NULL,
	[TransNumber] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[Bin] [nvarchar](max) NULL,
	[LotNo] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
	[StatusIssue] [int] NULL,
	[StatusReceipt] [int] NULL,
	[TenantId] [int] NULL,
	[TransId] [uniqueidentifier] NULL,
	[TransLineId] [uniqueidentifier] NULL,
	[PutAwayNo] [nvarchar](max) NULL,
	[PickingNo] [nvarchar](max) NULL,
	[PackingNo] [nvarchar](max) NULL,
 CONSTRAINT [PK_WarehouseTrans] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AuthUsers_Email]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_AuthUsers_Email] ON [authp].[AuthUsers]
(
	[Email] ASC
)
WHERE ([Email] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AuthUsers_TenantId]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_AuthUsers_TenantId] ON [authp].[AuthUsers]
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AuthUsers_UserName]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_AuthUsers_UserName] ON [authp].[AuthUsers]
(
	[UserName] ASC
)
WHERE ([UserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RefreshTokens_AddedDateUtc]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_RefreshTokens_AddedDateUtc] ON [authp].[RefreshTokens]
(
	[AddedDateUtc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RoleToPermissions_RoleType]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_RoleToPermissions_RoleType] ON [authp].[RoleToPermissions]
(
	[RoleType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RoleToPermissionsTenant_TenantsTenantId]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_RoleToPermissionsTenant_TenantsTenantId] ON [authp].[RoleToPermissionsTenant]
(
	[TenantsTenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Tenants_ParentDataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_Tenants_ParentDataKey] ON [authp].[Tenants]
(
	[ParentDataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Tenants_ParentTenantId]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_Tenants_ParentTenantId] ON [authp].[Tenants]
(
	[ParentTenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Tenants_TenantFullName]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tenants_TenantFullName] ON [authp].[Tenants]
(
	[TenantFullName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserToRoles_RoleName]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserToRoles_RoleName] ON [authp].[UserToRoles]
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ApiCodes_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_ApiCodes_DataKey] ON [dbo].[ApiCodes]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ArrivalInstructions_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_ArrivalInstructions_DataKey] ON [dbo].[ArrivalInstructions]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Categories_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_Categories_DataKey] ON [dbo].[Categories]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ChannelMasters_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_ChannelMasters_DataKey] ON [dbo].[ChannelMasters]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Channels_ChannelMasterCode]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_Channels_ChannelMasterCode] ON [dbo].[Channels]
(
	[ChannelMasterCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Channels_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_Channels_DataKey] ON [dbo].[Channels]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Companies_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_Companies_DataKey] ON [dbo].[Companies]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_CountryMasters_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_CountryMasters_DataKey] ON [dbo].[CountryMasters]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Currencies_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_Currencies_DataKey] ON [dbo].[Currencies]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_CurrencyPairSettings_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_CurrencyPairSettings_DataKey] ON [dbo].[CurrencyPairSettings]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ExchangeRates_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_ExchangeRates_DataKey] ON [dbo].[ExchangeRates]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_OrderItems_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_DataKey] ON [dbo].[OrderItems]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderItems_OrderHeaderId]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_OrderHeaderId] ON [dbo].[OrderItems]
(
	[OrderHeaderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_OrderReturnItems_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderReturnItems_DataKey] ON [dbo].[OrderReturnItems]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderReturnItems_OrderHeaderId]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderReturnItems_OrderHeaderId] ON [dbo].[OrderReturnItems]
(
	[OrderHeaderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Orders_ChannelCode]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_Orders_ChannelCode] ON [dbo].[Orders]
(
	[ChannelCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Orders_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_Orders_DataKey] ON [dbo].[Orders]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_OrderStatuses_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderStatuses_DataKey] ON [dbo].[OrderStatuses]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PackageAttachProducts_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_PackageAttachProducts_DataKey] ON [dbo].[PackageAttachProducts]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PackageAttachProducts_PackageId]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_PackageAttachProducts_PackageId] ON [dbo].[PackageAttachProducts]
(
	[PackageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Packages_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_Packages_DataKey] ON [dbo].[Packages]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ProductBundles_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_ProductBundles_DataKey] ON [dbo].[ProductBundles]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Products_CategoryId]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_Products_CategoryId] ON [dbo].[Products]
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Products_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_Products_DataKey] ON [dbo].[Products]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ProductStatuses_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_ProductStatuses_DataKey] ON [dbo].[ProductStatuses]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ProductStocks_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_ProductStocks_DataKey] ON [dbo].[ProductStocks]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SalesData_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_SalesData_DataKey] ON [dbo].[SalesData]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ShippingCountries_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_ShippingCountries_DataKey] ON [dbo].[ShippingCountries]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_StockInOutItems_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_StockInOutItems_DataKey] ON [dbo].[StockInOutItems]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_StockInOuts_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_StockInOuts_DataKey] ON [dbo].[StockInOuts]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_StockLocations_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_StockLocations_DataKey] ON [dbo].[StockLocations]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_StockTypes_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_StockTypes_DataKey] ON [dbo].[StockTypes]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SystemClassCompanies_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_SystemClassCompanies_DataKey] ON [dbo].[SystemClassCompanies]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ThirdPLWarehouses_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_ThirdPLWarehouses_DataKey] ON [dbo].[ThirdPLWarehouses]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserSettings_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserSettings_DataKey] ON [dbo].[UserSettings]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserVendors_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserVendors_DataKey] ON [dbo].[UserVendors]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Vendors_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_Vendors_DataKey] ON [dbo].[Vendors]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_WarehouseUserSettings_DataKey]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_WarehouseUserSettings_DataKey] ON [dbo].[WarehouseUserSettings]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [iExpirationTime_wms_RefreshTokens]    Script Date: 10/13/2024 10:43:36 AM ******/
CREATE NONCLUSTERED INDEX [iExpirationTime_wms_RefreshTokens] ON [wms].[RefreshTokens]
(
	[ExpirationTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [authp].[AuthUsers] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsDisabled]
GO
ALTER TABLE [authp].[RoleToPermissions] ADD  DEFAULT (CONVERT([tinyint],(0))) FOR [RoleType]
GO
ALTER TABLE [authp].[Tenants] ADD  DEFAULT (CONVERT([bit],(0))) FOR [HasOwnDb]
GO
ALTER TABLE [dbo].[BatchCalendars] ADD  DEFAULT ('0') FOR [ScheduleType]
GO
ALTER TABLE [dbo].[BatchSchedules] ADD  CONSTRAINT [DF__Batches__creat__1AA9E072]  DEFAULT (getdate()) FOR [CreateAt]
GO
ALTER TABLE [dbo].[BatchSchedules] ADD  CONSTRAINT [DF_BatchSchedules_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[HistoryOrderChanges] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[LogTime] ADD  CONSTRAINT [DF_LogTime_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[LogTime] ADD  CONSTRAINT [DF_LogTime_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[ProductBundles] ADD  DEFAULT ((0)) FOR [BundlePriceRatio]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [SupplierId]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [ProductType]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [ProductStatus]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [CategoryId]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [ShippingLimitDays]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [StockAvailableQuanitty]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [UnitId]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [VendorId]
GO
ALTER TABLE [dbo].[Suppliers] ADD  CONSTRAINT [DF__Suppliers__Suppl__40F9A68C]  DEFAULT ((0)) FOR [SupplierId]
GO
ALTER TABLE [dbo].[SystemMaxPKNos] ADD  CONSTRAINT [DF_SystemMaxPKNos_CreateAt]  DEFAULT (getdate()) FOR [CreateAt]
GO
ALTER TABLE [dbo].[SystemMaxPKNos] ADD  CONSTRAINT [DF_SystemMaxPKNos_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[TaskChatHistories] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[TaskChatHistories] ADD  DEFAULT (sysdatetime()) FOR [CreateAt]
GO
ALTER TABLE [dbo].[TaskChatHistories] ADD  DEFAULT ((0)) FOR [IsModified]
GO
ALTER TABLE [wms].[Batches] ADD  CONSTRAINT [DF_Batches_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [wms].[Bins] ADD  CONSTRAINT [DF_Bins_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [wms].[Devices] ADD  CONSTRAINT [DF_Devices_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [wms].[Locations] ADD  CONSTRAINT [DF_Locations_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [wms].[LogTime] ADD  CONSTRAINT [DF_LogTime_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [wms].[LogTime] ADD  CONSTRAINT [DF_LogTime_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [wms].[NumberSequences] ADD  CONSTRAINT [DF_NumberSequences_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [wms].[ReturnOrderLines] ADD  CONSTRAINT [DF_ReturnOrderLines_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [wms].[ReturnOrders] ADD  CONSTRAINT [DF_ReturnOrders_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [wms].[ShippingBoxes] ADD  CONSTRAINT [DF_ShippingBoxes_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [wms].[ShippingCarriers] ADD  CONSTRAINT [DF_ShippingCarrier_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [wms].[WarehousePackingLines] ADD  CONSTRAINT [DF_WarehousePackingLines_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [wms].[WarehousePackingList] ADD  CONSTRAINT [DF_WarehousePacks_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [wms].[WarehousePickingLines] ADD  CONSTRAINT [DF_WarehousePickLines_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [wms].[WarehousePickingList] ADD  CONSTRAINT [DF_WarehousePicks_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [wms].[WarehousePickingStaging] ADD  CONSTRAINT [DF_WarehousePickStaging_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [wms].[WarehousePutAways] ADD  CONSTRAINT [DF_WarehousePutAways_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [wms].[WarehouseReceiptOrder] ADD  CONSTRAINT [DF_WarehouseReceiptOrder_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [wms].[WarehouseReceiptOrderLine] ADD  CONSTRAINT [DF_WarehouseReceiptOrderLine_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [wms].[WarehouseReceiptStaging] ADD  CONSTRAINT [DF_WarehouseReceiptStaging_id]  DEFAULT (newid()) FOR [id]
GO
ALTER TABLE [wms].[WarehouseShipmentLines] ADD  CONSTRAINT [DF_WarehouseShipmentLines_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [wms].[WarehouseShipments] ADD  CONSTRAINT [DF_WarehouseShipments_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [wms].[WarehouseTrans] ADD  CONSTRAINT [DF_WarehouseTrans_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [authp].[AuthUsers]  WITH CHECK ADD  CONSTRAINT [FK_AuthUsers_Tenants_TenantId] FOREIGN KEY([TenantId])
REFERENCES [authp].[Tenants] ([TenantId])
GO
ALTER TABLE [authp].[AuthUsers] CHECK CONSTRAINT [FK_AuthUsers_Tenants_TenantId]
GO
ALTER TABLE [authp].[RoleToPermissionsTenant]  WITH CHECK ADD  CONSTRAINT [FK_RoleToPermissionsTenant_RoleToPermissions_TenantRolesRoleName] FOREIGN KEY([TenantRolesRoleName])
REFERENCES [authp].[RoleToPermissions] ([RoleName])
ON DELETE CASCADE
GO
ALTER TABLE [authp].[RoleToPermissionsTenant] CHECK CONSTRAINT [FK_RoleToPermissionsTenant_RoleToPermissions_TenantRolesRoleName]
GO
ALTER TABLE [authp].[RoleToPermissionsTenant]  WITH CHECK ADD  CONSTRAINT [FK_RoleToPermissionsTenant_Tenants_TenantsTenantId] FOREIGN KEY([TenantsTenantId])
REFERENCES [authp].[Tenants] ([TenantId])
ON DELETE CASCADE
GO
ALTER TABLE [authp].[RoleToPermissionsTenant] CHECK CONSTRAINT [FK_RoleToPermissionsTenant_Tenants_TenantsTenantId]
GO
ALTER TABLE [authp].[Tenants]  WITH CHECK ADD  CONSTRAINT [FK_Tenants_Tenants_ParentTenantId] FOREIGN KEY([ParentTenantId])
REFERENCES [authp].[Tenants] ([TenantId])
GO
ALTER TABLE [authp].[Tenants] CHECK CONSTRAINT [FK_Tenants_Tenants_ParentTenantId]
GO
ALTER TABLE [authp].[UserToRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserToRoles_AuthUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [authp].[AuthUsers] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [authp].[UserToRoles] CHECK CONSTRAINT [FK_UserToRoles_AuthUsers_UserId]
GO
ALTER TABLE [authp].[UserToRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserToRoles_RoleToPermissions_RoleName] FOREIGN KEY([RoleName])
REFERENCES [authp].[RoleToPermissions] ([RoleName])
ON DELETE CASCADE
GO
ALTER TABLE [authp].[UserToRoles] CHECK CONSTRAINT [FK_UserToRoles_RoleToPermissions_RoleName]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Channels]  WITH CHECK ADD  CONSTRAINT [FK_Channels_ChannelMasters_ChannelMasterCode] FOREIGN KEY([ChannelMasterCode])
REFERENCES [dbo].[ChannelMasters] ([ChannelMasterCode])
GO
ALTER TABLE [dbo].[Channels] CHECK CONSTRAINT [FK_Channels_ChannelMasters_ChannelMasterCode]
GO
ALTER TABLE [dbo].[OrderDispatchProducts]  WITH CHECK ADD  CONSTRAINT [FK_OrderDispatchProducts_OrderDispatches_DispatchHeaderId] FOREIGN KEY([DispatchHeaderId])
REFERENCES [dbo].[OrderDispatches] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderDispatchProducts] CHECK CONSTRAINT [FK_OrderDispatchProducts_OrderDispatches_DispatchHeaderId]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderItems_Orders_OrderHeaderId] FOREIGN KEY([OrderHeaderId])
REFERENCES [dbo].[Orders] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK_OrderItems_Orders_OrderHeaderId]
GO
ALTER TABLE [dbo].[OrderReturnItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderReturnItems_Orders_OrderHeaderId] FOREIGN KEY([OrderHeaderId])
REFERENCES [dbo].[Orders] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderReturnItems] CHECK CONSTRAINT [FK_OrderReturnItems_Orders_OrderHeaderId]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Channels_ChannelCode] FOREIGN KEY([ChannelCode])
REFERENCES [dbo].[Channels] ([ChannelCode])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Channels_ChannelCode]
GO
ALTER TABLE [dbo].[PackageAttachProducts]  WITH CHECK ADD  CONSTRAINT [FK_PackageAttachProducts_Packages_PackageId] FOREIGN KEY([PackageId])
REFERENCES [dbo].[Packages] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PackageAttachProducts] CHECK CONSTRAINT [FK_PackageAttachProducts_Packages_PackageId]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Categories_CategoryId]
GO
/****** Object:  StoredProcedure [dbo].[ManualProcCreateSchedule]    Script Date: 10/13/2024 10:43:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO







/* ***************************************************************
 Batchスケジュール登録 schedule_division "00": 1回のみ "01":繰り返し登録
*************************************************************** */

CREATE PROCEDURE [dbo].[ManualProcCreateSchedule]
	@company_id As varchar(50),
	@target_day_plus As int
AS
BEGIN

SET NOCOUNT ON;

	DECLARE @batch_id			VARCHAR(50)
	DECLARE @schedule_division	VARCHAR(50)
	DECLARE @schedule_time		TIME
	DECLARE @start_time			TIME
	DECLARE @end_time			TIME
	DECLARE @end_date			datetime
	DECLARE @start_date			datetime
	DECLARE @set_datetime		datetime
	DECLARE @target_date		varchar(50)
	DECLARE @schedule_type		varchar(10)
	DECLARE @target_day_of_week	VARCHAR(1)

	set @target_date = CONVERT(varchar, dateadd(day,+@target_day_plus,getdate()), 111);
	set @schedule_type = ISNULL((SELECT top 1 ScheduleType FROM BatchCalendars where [Date]=@target_date),'0');
	set @target_day_of_week = DATEPART(WEEKDAY, dateadd(day,+@target_day_plus,getdate()));
	

	DECLARE curBatchScheduleDT CURSOR FOR
	select 
		 A.CompanyId
		,A.BatchId
		,A.StartTime
		,IsNULL(A.EndTime,convert(time,'23:59:59:999')) as EndTime
		,A.ScheduleDivision
		,A.ScheduleTime
	from BatchSchedules A
		 inner join Batches B
			on B.BatchId	= A.BatchId
			and B.IsDeleted = 0
			and A.IsDeleted = 0
			and
			(
				(ISNULL([DayOfWeek],'0') IN ('','0') and ISNULL(ScheduleType,'0') = @schedule_type)
				or
				(ISNULL([DayOfWeek],'0') = @target_day_of_week)
			)
			
	where a.CompanyId = @company_id

	BEGIN TRY
		BEGIN TRANSACTION;
			

			OPEN curBatchScheduleDT
			FETCH NEXT 
			FROM 
			curBatchScheduleDT
			INTO @company_id,@batch_id,@start_time,@end_time,@schedule_division,@schedule_time

	
			WHILE @@FETCH_STATUS = 0
			BEGIN 
				
				-- 2020/1/14 add start
				select	 @start_date = convert(datetime,convert(varchar,convert(date,getDate() + @target_day_plus)) + ' ' + Left(convert(varchar,@start_time),8))
						,@end_date  = convert(datetime,convert(varchar,convert(date,getDate() + @target_day_plus)) + ' ' + Left(convert(varchar,@end_time),8))
				-- 2020/1/14 add end

				set @set_datetime = @start_date;
				
				WHILE @set_datetime < @end_date
				BEGIN

					--don't insert past Date
					IF @set_datetime > getDate()
					BEGIN

						MERGE INTO
						  TaskSchedules AS XX
						USING
						  (
						  SELECT
							  @company_id AS company_id
							  ,@batch_id AS batch_id
							  ,@set_datetime AS set_datetime
              
						  ) AS YY ON
						  (
								XX.CompanyId = YY.company_id 
							AND XX.BatchId = YY.batch_id
							AND XX.ScheduleDatetime = YY.set_datetime
							)
						WHEN NOT MATCHED THEN

							INSERT ([ScheduleDatetime]
									  ,[CompanyId]
									   ,[BatchId]
									   ,[Priority]
									   ,[IsFailed]
									   ,[IsStopped]
									   ,[IsBatchInRequest]
									   ,[CreateOperatorId]
									   ,[CreateAt]
									   ,[UpdateOperatorId]
									   ,[UpdatedAt]
									   ,[IsDeleted])
								 VALUES
									   (@set_datetime
									   ,@company_id
									   ,@batch_id
									   ,'0'
									   ,0
									   ,0
									   ,0
									   ,'9099999'
									   ,getDate()
									   ,'9099999'
									   ,getDate()
									   ,0
									   );
					END

					IF @schedule_division = '00'
					BEGIN
						SET @set_datetime = DATEADD(year, 1, @set_datetime);
					END
					Else
					BEGIN
						set @set_datetime = dateadd(mi,datediff(mi,convert(time,'00:00:00'),@schedule_time), @set_datetime)
					END
				END


				FETCH NEXT FROM curBatchScheduleDT 
				INTO @company_id,@batch_id,@start_time,@end_time,@schedule_division,@schedule_time
				;
			END
		 COMMIT TRANSACTION
		 
		 CLOSE curBatchScheduleDT;
		 DEALLOCATE curBatchScheduleDT;

	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION
		SELECT  
         ERROR_NUMBER() AS ErrorNumber  
        ,ERROR_SEVERITY() AS ErrorSeverity  
        ,ERROR_STATE() AS ErrorState  
        ,ERROR_PROCEDURE() AS ErrorProcedure  
        ,ERROR_LINE() AS ErrorLine  
        ,ERROR_MESSAGE() AS ErrorMessage;  
		
		IF CURSOR_STATUS('global','curBatchScheduleDT') > 0
			BEGIN
				CLOSE curBatchScheduleDT;
				DEALLOCATE curBatchScheduleDT;
			END


		RETURN 1

	END CATCH

	RETURN 0

END








GO
/****** Object:  StoredProcedure [dbo].[ProcCreateSchedule]    Script Date: 10/13/2024 10:43:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO








/* ***************************************************************
 Batchスケジュール登録 schedule_division "00": 1回のみ "01":繰り返し登録
*************************************************************** */

CREATE PROCEDURE [dbo].[ProcCreateSchedule]
	@company_id As varchar(50)
AS
BEGIN

SET NOCOUNT ON;

	DECLARE @batch_id			VARCHAR(50)
	DECLARE @schedule_division	VARCHAR(50)
	DECLARE @schedule_time		TIME
	DECLARE @start_time			TIME
	DECLARE @end_time			TIME
	DECLARE @end_date			datetime
	DECLARE @start_date			datetime
	DECLARE @set_datetime		datetime
	DECLARE @target_day_plus	int
	DECLARE @target_date		varchar(50)
	DECLARE @schedule_type		varchar(10)
	DECLARE @target_day_of_week	VARCHAR(1)
	DECLARE @target_day_of_month	VARCHAR(2)

	set @target_day_plus = 2
	set @target_date = CONVERT(varchar, dateadd(day,+@target_day_plus,getdate()), 111);
	set @schedule_type = ISNULL((SELECT top 1 ScheduleType FROM BatchCalendars where [Date]=@target_date),'0');
	set @target_day_of_week = DATEPART(WEEKDAY, dateadd(day,+@target_day_plus,getdate()));
	set @target_day_of_month = DATEPART(day, dateadd(day,+@target_day_plus,getdate()));

	DECLARE curBatchScheduleDT CURSOR FOR
	select 
		 A.CompanyId
		,A.BatchId
		,A.StartTime
		,IsNULL(A.EndTime,convert(time,'23:59:59:999')) as EndTime
		,A.ScheduleDivision
		,A.ScheduleTime
	from BatchSchedules A
		 inner join Batches B
			on B.BatchId	= A.BatchId
			and B.IsDeleted = 0
			and A.IsDeleted = 0
			and
			(
				(ISNULL([DayOfWeek],'0') IN ('','0') and ISNULL(ScheduleType,'0') = @schedule_type)
				or
				(ISNULL([DayOfWeek],'0') = @target_day_of_week)
			)
			and
			(
				(ISNULL([DayOfWeek],'0') IN ('','0') and ISNULL(ScheduleType,'0') = @schedule_type)
				or
				(ISNULL([DayOfWeek],'0') = @target_day_of_month)
			)
			
	where a.CompanyId = @company_id

	BEGIN TRY
		BEGIN TRANSACTION;
			

			OPEN curBatchScheduleDT
			FETCH NEXT 
			FROM 
			curBatchScheduleDT
			INTO @company_id,@batch_id,@start_time,@end_time,@schedule_division,@schedule_time

	
			WHILE @@FETCH_STATUS = 0
			BEGIN 
				
				/*当日のスケジュール作成*/
				--select	 @start_date = convert(datetime,convert(varchar,convert(date,getDate() )) + ' ' + Left(convert(varchar,@start_time),8))
				--		,@end_date  = convert(datetime,convert(varchar,convert(date,getDate() )) + ' ' + Left(convert(varchar,@end_time),8))
				/*翌日のスケジュール作成*/
				--select	 @start_date = convert(datetime,convert(varchar,convert(date,getDate() + 1)) + ' ' + Left(convert(varchar,@start_time),8))
				--		,@end_date  = convert(datetime,convert(varchar,convert(date,getDate() + 1)) + ' ' + Left(convert(varchar,@end_time),8))
				
				-- 2020/1/14 add start
				select	 @start_date = convert(datetime,convert(varchar,convert(date,getDate() + @target_day_plus)) + ' ' + Left(convert(varchar,@start_time),8))
						,@end_date  = convert(datetime,convert(varchar,convert(date,getDate() + @target_day_plus)) + ' ' + Left(convert(varchar,@end_time),8))
				-- 2020/1/14 add end

				set @set_datetime = @start_date;
				--set @set_datetime = dateadd(mi,datediff(mi,convert(time,'00:00:00'),@schedule_time), @set_datetime)

				WHILE @set_datetime < @end_date
				BEGIN

					--don't insert past Date
					IF @set_datetime > getDate()
					BEGIN

						-- MERGE INTO
						--   [trn_task_schedule] AS XX
						-- USING
						--   (
						--   SELECT
						-- 	  @company_id AS company_id
						-- 	  ,@batch_id AS batch_id
						-- 	  ,@set_datetime AS set_datetime
              			-- 
						--   ) AS YY ON
						--   (
						-- 		XX.company_id = YY.company_id 
						-- 	AND XX.batch_id = YY.batch_id
						-- 	AND XX.schedule_datetime = YY.set_datetime
						-- 	)
						-- WHEN NOT MATCHED THEN

							INSERT into TaskSchedules ([ScheduleDatetime]
									   ,[CompanyId]
									   ,[BatchId]
									   ,[Priority]
									   ,[IsFailed]
									   ,[IsStopped]
									   ,[IsBatchInRequest]
									   ,[CreateOperatorId]
									   ,[CreateAt]
									   ,[UpdateOperatorId]
									   ,[UpdatedAt]
									   ,[IsDeleted])
								 VALUES
									   (@set_datetime
									   ,@company_id
									   ,@batch_id
									   ,'0'
									   ,0
									   ,0
									   ,0
									   ,'9099999'
									   ,getDate()
									   ,'9099999'
									   ,getDate()
									   ,0
									   );
					END

					IF @schedule_division = '00'
					BEGIN
						SET @set_datetime = DATEADD(year, 1, @set_datetime);
					END
					Else
					BEGIN
						set @set_datetime = dateadd(mi,datediff(mi,convert(time,'00:00:00'),@schedule_time), @set_datetime)
					END
				END


				FETCH NEXT FROM curBatchScheduleDT 
				INTO @company_id,@batch_id,@start_time,@end_time,@schedule_division,@schedule_time
				;
			END
		 COMMIT TRANSACTION
		 
		 CLOSE curBatchScheduleDT;
		 DEALLOCATE curBatchScheduleDT;

	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION
		SELECT  
         ERROR_NUMBER() AS ErrorNumber  
        ,ERROR_SEVERITY() AS ErrorSeverity  
        ,ERROR_STATE() AS ErrorState  
        ,ERROR_PROCEDURE() AS ErrorProcedure  
        ,ERROR_LINE() AS ErrorLine  
        ,ERROR_MESSAGE() AS ErrorMessage;  
		
		IF CURSOR_STATUS('global','curBatchScheduleDT') > 0
			BEGIN
				CLOSE curBatchScheduleDT;
				DEALLOCATE curBatchScheduleDT;
			END


		RETURN 1

	END CATCH

	RETURN 0

END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Input by user , document date' , @level0type=N'SCHEMA',@level0name=N'wms', @level1type=N'TABLE',@level1name=N'WarehouseTrans', @level2type=N'COLUMN',@level2name=N'DatePhysical'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Primary key of master' , @level0type=N'SCHEMA',@level0name=N'wms', @level1type=N'TABLE',@level1name=N'WarehouseTrans', @level2type=N'COLUMN',@level2name=N'TransId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Transaction line Id of each warehouse table' , @level0type=N'SCHEMA',@level0name=N'wms', @level1type=N'TABLE',@level1name=N'WarehouseTrans', @level2type=N'COLUMN',@level2name=N'TransLineId'
GO
USE [master]
GO
ALTER DATABASE [FBT_DEV] SET  READ_WRITE 
GO
