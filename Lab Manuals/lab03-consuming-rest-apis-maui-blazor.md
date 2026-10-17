# Lab 03 – Consuming REST APIs in MAUI Blazor

**Official task:** Configure `HttpClient`; asynchronously fetch and display remote API data through DTOs; show loading and errors.

**Home task:** Dynamically query the API using search/filter query parameters.

---

## 3.1 Start with the completed Lab 2 solution

1. Apply the Lab 3 stage if using the scripts, or follow every code-listing step below manually. This lab adds `CampusDesk.Api` as a live local backend.
2. In the **Application** project add `StudentDto.cs` and `IStudentApiClient.cs`. `StudentDto` represents the network payload; it is separate from the `Domain.Student` entity used by Lab 2.

**File: `Application/StudentDto.cs`**

```csharp
namespace CampusDesk.Application;

public sealed record StudentDto(int Id, string FullName, string Department, string Email);
```

**File: `Application/IStudentApiClient.cs`**

```csharp
namespace CampusDesk.Application;

public interface IStudentApiClient
{
    Task<IReadOnlyList<StudentDto>> SearchAsync(
        string search, string department, CancellationToken cancellationToken = default);
}
```

3. In the **Infrastructure** project add `HttpStudentApiClient.cs`. It serializes URL query parameters safely, sends the GET request, checks the response status and deserializes JSON.

**File: `Infrastructure/HttpStudentApiClient.cs`**

```csharp
using System.Net.Http.Json;
using CampusDesk.Application;

namespace CampusDesk.Infrastructure;

public sealed class HttpStudentApiClient : IStudentApiClient
{
    private readonly HttpClient _client;

    public HttpStudentApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<StudentDto>> SearchAsync(
        string search, string department, CancellationToken cancellationToken = default)
    {
        var path = "api/students?search=" + Uri.EscapeDataString(search ?? "") +
                   "&department=" + Uri.EscapeDataString(department ?? "");

        using var response = await _client.GetAsync(path, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<StudentDto>>(
            cancellationToken: cancellationToken) ?? new List<StudentDto>();
    }
}
```

4. Open `CampusDesk.Api/Program.cs` and replace all contents with the complete minimal ASP.NET Core API below. It serves a health endpoint and eight in-memory student DTOs on port 5088.

**File: `CampusDesk.Api/Program.cs`**

```csharp
using CampusDesk.Application;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Deliberately fixed port for this classroom exercise. In production use HTTPS.
var students = new List<StudentDto>
{
    new(1, "Ayesha Khan", "CS", "ayesha@example.com"),
    new(2, "Bilal Ahmad", "IT", "bilal@example.com"),
    new(3, "Sara Malik", "SE", "sara@example.com"),
    new(4, "Hamza Ali", "CS", "hamza@example.com"),
    new(5, "Hira Noor", "IT", "hira@example.com"),
    new(6, "Usman Tariq", "SE", "usman@example.com"),
    new(7, "Zainab Raza", "CS", "zainab@example.com"),
    new(8, "Danish Farooq", "IT", "danish@example.com")
};

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapGet("/api/students", (string? search, string? department) =>
{
    IEnumerable<StudentDto> result = students;

    if (!string.IsNullOrWhiteSpace(search))
        result = result.Where(s => s.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                   s.Email.Contains(search, StringComparison.OrdinalIgnoreCase));

    if (!string.IsNullOrWhiteSpace(department))
        result = result.Where(s => s.Department.Equals(department, StringComparison.OrdinalIgnoreCase));

    return Results.Ok(result.ToArray());
});

app.Run("http://0.0.0.0:5088");
```

---

## 3.2 Start and inspect the API BEFORE launching MAUI

5. Open Visual Studio Terminal or a separate PowerShell/Terminal window in the extracted source folder. Run the following command from the root folder containing Lab01/Lab02/Lab03:

```
dotnet run --no-launch-profile --project CampusDesk/CampusDesk.Api
```

6. In a browser, visit `http://localhost:5088/health`, followed by `http://localhost:5088/api/students`. Confirm the health JSON and an array containing eight students.
7. Run the filtered URL below. Confirm that only the CS student whose name/email matches Ayesha is returned:

```
http://localhost:5088/api/students?search=Ayesha&department=CS
```

8. Leave the API process running throughout the MAUI client demonstration. Closing the API terminal makes the network request fail (which you will deliberately test later).

---

## 3.3 Configure the MAUI HttpClient and Android network

9. Replace `MauiProgram.cs` with the Lab 3 version. Windows/Mac Catalyst use `localhost`; Android emulator uses `10.0.2.2`. All clients target port 5088.

**File: `Presentation.Client/MauiProgram.cs`**

```csharp
using CampusDesk.Application;
using CampusDesk.Domain;
using CampusDesk.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices;
using Presentation.Client.Services;

namespace Presentation.Client;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton<IPlatformService, MauiPlatformService>();
        builder.Services.AddSingleton<INativeNavigator, NativeNavigator>();
        builder.Services.AddSingleton<StudentValidator>();
        builder.Services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
        builder.Services.AddTransient<StudentRegistrationHandler>();
        builder.Services.AddTransient<StudentQueryHandler>();

        // Android emulator reaches the development host through 10.0.2.2.
        var baseUrl = DeviceInfo.Current.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5088/"
            : "http://localhost:5088/";

        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(15)
        });
        builder.Services.AddSingleton<IStudentApiClient, HttpStudentApiClient>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
```

10. For Android debugging only, replace `Platforms/Android/MainApplication.cs` with the supplied file. Do not enable unrestricted HTTP in production.

**File: `Presentation.Client/Platforms/Android/MainApplication.cs`**

```csharp
using Android.App;
using Android.Runtime;

namespace Presentation.Client;

#if DEBUG
[Application(UsesCleartextTraffic = true)]
#else
[Application]
#endif
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

---

## 3.4 Build the UI, loading state, error state and search

11. Create `Presentation.Client/Components/Pages/Students.razor`. Paste all markup and `@code` content, including `IDisposable` cancellation cleanup.

**File: `Presentation.Client/Components/Pages/Students.razor`**

```razor
@page "/students"
@implements IDisposable
@inject CampusDesk.Application.IStudentApiClient Api

<PageTitle>Remote API students</PageTitle>
<h1>Remote students from ASP.NET Core API</h1>
<p>Lab 3: HttpClient, DTOs, query parameters, loading states, and errors.</p>

<div class="cd-card">
    <label for="remote-search">Search name or email</label>
    <input id="remote-search" placeholder="Try Ayesha or example.com"
           @bind="searchText" @bind:event="oninput" @bind:after="OnFiltersChangedAsync" />

    <label for="remote-dept">Department</label>
    <select id="remote-dept" @bind="department" @bind:after="OnFiltersChangedAsync">
        <option value="">All departments</option>
        <option value="CS">CS</option>
        <option value="IT">IT</option>
        <option value="SE">SE</option>
    </select>

    <div><button class="cd-button" @onclick="ReloadAsync">Reload from API</button></div>
</div>

@if (isLoading)
{
    <p role="status">Loading students from the API...</p>
}
else if (errorMessage is not null)
{
    <p role="alert" class="cd-error">@errorMessage</p>
}
else if (students.Count == 0)
{
    <p>No matching students were found.</p>
}
else
{
    <div class="cd-card cd-scroll">
        <p><strong>@students.Count result(s)</strong></p>
        <table>
            <thead><tr><th>ID</th><th>Name</th><th>Department</th><th>Email</th></tr></thead>
            <tbody>
                @foreach (var student in students)
                {
                    <tr>
                        <td>@student.Id</td>
                        <td>@student.FullName</td>
                        <td>@student.Department</td>
                        <td>@student.Email</td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
}

@code {
    private List<CampusDesk.Application.StudentDto> students = new();
    private string searchText = "";
    private string department = "";
    private bool isLoading;
    private string? errorMessage;
    private CancellationTokenSource? searchCancellation;

    protected override Task OnInitializedAsync() => LoadAsync(CancellationToken.None);

    private async Task OnFiltersChangedAsync()
    {
        searchCancellation?.Cancel();
        searchCancellation?.Dispose();
        searchCancellation = new CancellationTokenSource();
        var token = searchCancellation.Token;

        try
        {
            await Task.Delay(300, token); // debounce typing
            await LoadAsync(token);
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // A newer search replaced this one.
        }
    }

    private async Task ReloadAsync()
    {
        searchCancellation?.Cancel();
        await LoadAsync(CancellationToken.None);
    }

    private async Task LoadAsync(CancellationToken token)
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            var result = await Api.SearchAsync(searchText, department, token);
            if (!token.IsCancellationRequested)
                students = result.ToList();
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // Ignore superseded requests.
        }
        catch (HttpRequestException)
        {
            errorMessage = "Cannot reach the API. Start CampusDesk.Api on port 5088 and try again.";
        }
        catch (TaskCanceledException)
        {
            errorMessage = "The API request timed out. Please retry.";
        }
        finally
        {
            if (!token.IsCancellationRequested)
                isLoading = false;
        }
    }

    public void Dispose()
    {
        searchCancellation?.Cancel();
        searchCancellation?.Dispose();
    }
}
```

12. Build. Start `CampusDesk.Api` first. Then run `Presentation.Client`, open **Remote API Students**, and verify the eight DTO records display in a table.

---

## 3.5 Mac Catalyst local HTTP option (if needed)

For Mac Catalyst testing, Apple Transport Security may block local HTTP even though the API is working. If that occurs, open `Presentation.Client/Platforms/MacCatalyst/Info.plist`. Inside its outer `<dict>` (immediately before the closing `</dict>`), add:

```xml
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsLocalNetworking</key>
    <true/>
</dict>
```

Do not add a second `NSAppTransportSecurity` entry if one exists; merge the `NSAllowsLocalNetworking` key into its dictionary. This is a local-development exception only. For real deployed services use HTTPS and trusted certificates.

---

## 3.6 Validate ALL requirements of Lab 3 and its home task

1. **No filters:** remote table displays 8 student records.
2. **Search `Ayesha`:** only Ayesha Khan should remain.
3. **Clear search, select CS:** results should be Ayesha Khan, Hamza Ali and Zainab Raza (3 records).
4. **Type a search quickly:** after about 300 ms without further typing, the component queries the API. This is debouncing, which avoids unnecessary requests.
5. **Select IT and enter `bilal`:** one matching result should remain; both URL parameters are sent.
6. **Enter a string that matches no name or email:** "No matching students were found" appears.
7. **Stop the API process and click Reload from API:** a red error message appears instead of an uncaught crash.
8. **Restart the API, click Reload from API:** verify the table recovers.
9. Use the Visual Studio Output window or debugger to verify `HttpStudentApiClient.SearchAsync` is called; note how `Uri.EscapeDataString` protects spaces, `@` and other query characters.

---

## 3.7 API vs. client records – common viva trick

`Register.razor` from Lab 2 uses an in-process singleton repository in the MAUI process. `Students.razor` from Lab 3 uses the separate API process and its eight seeded DTOs. They deliberately have **independent data stores**. Creating a local student does **NOT** add it to the remote API list; saving, synchronization and the shared database are later topics.
