# learn-dotnet-multi-tenant

.NET 多租户 课程学习

## 一、Clean Architecture

**项目结构：**

- Application - 应用层将包含CQRS的实现。
- Domain - 领域层中定义数据库实体。
- Infrastructure - 基础设施层包含外部依赖项。
- WebApi - 应用程序接口即各程序之间交互程序。

### Domain 领域层

- 定义实体类。

```csharp
/// <summary>
/// 公司
/// </summary>
public class Company
{
    /// <summary>
    /// 公司Id
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 公司名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 建立日期
    /// </summary>
    public DateTime EstablishedDate { get; set; }
}
```

### Infrastructure 基础设施层

#### 安装组件包

- 定义多租户上下文对象。
- 定义应用程序上下文对象。
- 定义身份认证上下文对象。

1. 安装多租户组件。

```bash
dotnet add package Finbuckle.MultiTenant
dotnet add package Finbuckle.MultiTenant.AspNetCore
dotnet add package Finbuckle.MultiTenant.EntityFrameworkCore
```

2. 安装数据持久化组件。

```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

### WebApi 接口层

#### 安装组件包

1. 安装迁移工具组件。

```bash
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

#### 迁移

1. 使用以下命令生成多租户数据库迁移文件。

```bash
add-migration InitialTenantDb -context TenantDbContext
```

2. 更新数据库

```bash
update-database -context TenantDbContext
```

3. 使用以下命令生成业务及身份认证的数据库迁移文件

```bash
add-migration InitialApplicatinDb -context ApplicationDbContext
```

4. 更新数据库

```bash
update-database -context ApplicationDbContext
```

## 二、常量

- 定义权限动作常量：增删改查。
- 定义权限特性常量：多租户、用户、角色。
- 定义权限列表常量：所有权限、管理员权限、普通权限。