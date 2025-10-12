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

## 八、CQRS和身份认证端点

### 在应用层安装类库

```bash
dotnet add package MediatR
dotnet add package Mapster
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions
```

## 九、租户管理

- 实现租户服务。

## 十、公司实体管理

- 定义公司服务接口与实现。
- 定义公司服务的CQRS。
- 定义公司接口。

## 十一、MediatR管道行为

- 定义 MediatR Pipeline Behavior 用于处理验证。

## 十二、身份认证子系统

- 实现角色服务。
- 实现用户服务。

## 十三、Blazor WebAssembly

创建 `Blazor WebAssembly` 项目。

```bash
dotnet new blazorwasm -n PMS.WebApp
```

安装 [MudBlazor](https://mudblazor.com) 主题。

```bash
dotnet add package MudBlazor
```

安装 Blazor 存储组件。

```bash
dotnet add package Blazored.LocalStorage
```

安装其他组件。

```bash
dotnet add package Microsoft.AspNetCore.Components.Authorization
dotnet add package Microsoft.AspNetCore.Components.WebAssembly
dotnet add package Microsoft.Extensions.Http
dotnet add package Toolbelt.Blazor.HttpClientInterceptor
```

## 十四、UI组件

### 组件基础语法

1. 参数属性，用于前后端传输数据。

```csharp
public partial class LifeCycle
{
    [Parameter]
    public string Message { get; set; }
}
```

2. 只读字段，用于前端展示（但不能进行修改）。

```csharp
public partial class LifeCycle
{
    private Guide _id = Guid.NewGuid();
    private int _renderCount = 0;
    private DateTime _initializedAt = DateTime.Now;
}
```

3. 状态变更事件，用于页面重新渲染。

```csharp
private void ForceRerender()
{
    StateHasChanged();
}
```

4. Razor 语法。

```html
<h3>Lifecycle demo (@_renderCount)</h3>

<ul>
    <li>Id: @_id</li>
    <li>Message: @Message</li>
    <li>Initialized at: @_initializedAt:HH:mm:ss</li>
</ul>

<button class="btn btn-primary" @onclick="ForceRerender">
    Force re-render
</button>
```

### 组件生命周期

1. 第一个生命周期事件（入口点）为 `SetParameterAsync`，当 `[Parameter]` 参数传递给组件时触发。

```csharp
public override Task SetParametersAsync(ParameterView parameters)
{
    return base.SetParametersAsync(parameters);
}
```

前端传递参数。

```html
<Lifecycle Message="这是一个消息。"/>
```

2. 第二个生命周期事件为 `OnInitialized` 初始化事件，用于初始化组件的字段。

```csharp
protected override void OnInitialized()
{
    base.OnInitialized();
}
```

同时该事件还有一个异步版本 `OnInitializedAsync` 异步初始化事件。

```csharp
protected override Task OnInitializedAsync()
{
    return base.OnInitialized();
}
```

4. 第三个生命周期事件为 `OnParametersSet` 参数变更事件，用于处理组件接收参数或参数从父组件更改时触发。

```csharp
protected override void OnParametersSet()
{
    Console.WriteLine($"OnParametersSet {Message}")
}
```

5. 第四个生命周期事件为 `ShouldRender`，用于决定是否渲染该组件的事件。

```csharp
protected override bool ShouldRender()
{
    Console.WriteLine("ShouldRender => true");
    return true;
}
```

6. 第五个生命周期事件为 `OnAfterRender`，每次渲染后触发。可用于统计被渲染的次数

```csharp
protected override void OnAfterRender()
{
    _renderCount++;
}
```

同时也有一个异步版本 `OnAfterRenderAsync`。

```csharp
protected override void OnAfterRenderAsync()
{
    _renderCount++;
}
```

7. 第六个生命周期事件为 `Dispose`，用于清理和释放内存的。

> 需要继承 `IDisposable` 并编写 `Dispose()` 方法。

```csharp
public partial class LifeCycle : IDisposable
{
    public void Dispose()
    {
    
    }
}
```