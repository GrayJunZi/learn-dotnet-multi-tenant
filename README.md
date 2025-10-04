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

## 三、数据库初始化

- 定义初始化数据库时的种子数据类。
- 将种子数据类注册到容器中。
- 启动程序时自动执行迁移及添加种子数据操作。

## 四、自定义异常与全局响应包装器

- 定义常见异常类。
- 定义API响应包装类。

## 五、权限基础之鉴权

- 定义权限数据类
- 定义权限提供器类。
- 定义鉴权处理类。

## 六、JWT生成之认证

- 定义Token生成类。
- 注册身份认证与鉴权。

在 `Infrastructure` 类库中安装 `Jwt` 生成组件。

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

## 七、OpenAPI配置

- 定义Scalar配置类。
- 注册Scalar到容器中。