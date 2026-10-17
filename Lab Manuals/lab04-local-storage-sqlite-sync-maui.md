# Lab 04: Local Storage & Database Sync in MAUI

**Air University | CS-301L | Full Stack Web Development Lab | Lab 04**
CampusDesk | Step-by-step Visual Studio manual | Windows | .NET 10

Instructor/student practical guide. Continues the CampusDesk solution created in Labs 1–3. No PowerShell is required.

| Course-guide requirement | What you will implement |
|---|---|
| Task focus (Week 04) | Integrate SQLite into a .NET MAUI Blazor Hybrid app and persist local records offline. |
| Home task (Week 04) | Implement a durable offline-first synchronization queue that flags local create/update/delete changes for later synchronization. |
| End result | A new **Offline SQLite Students** page, persistent student records, and a pending-change queue, all visible after restarting the application. |

> **Source scope:** These two requirements come directly from page 7 of the Full Stack Web Development Lab course guide (2026–2027). The student-management example, file names, GUI workflow and queue design are implementation choices for this manual.

## Learning outcomes

- Explain why in-memory C# lists lose data when a desktop app closes, while a local SQLite file persists.
- Install `sqlite-net-pcl` through the Visual Studio NuGet interface and register local storage with dependency injection.
- Create, read, edit and delete SQLite-backed student records from a Blazor Razor component.
- Record each local mutation in a durable queue in the same SQLite transaction as the data change.
- Demonstrate offline behavior, restart persistence, and queued changes using repeatable test steps.

## Estimated time and prerequisites

| Item | Requirement |
|---|---|
| Time | Approximately 2–3 hours including NuGet restore, coding, and testing. |
| Visual Studio | Visual Studio 2026 on Windows with the .NET MAUI workload and .NET 10 SDK. |
| Solution | CampusDesk solution with Domain, Application, Infrastructure, Presentation.Client and CampusDesk.Api. |
| Stage | Lab 3 source already in your Visual Studio solution; Lab 4 does NOT require the API server to be running. |
| Source files | `CampusDesk_Lab04_Source.zip` supplied with this manual. |

---

## A. Understand the architecture before coding

The new feature is added without changing the earlier Lab 2 in-memory registration page or the Lab 3 remote API page. It deliberately has a separate local database so you can prove that persistence is working without the backend running.

```
Offline.razor (Presentation.Client)
  → OfflineStudentHandler (Application)
  → IOfflineStudentStore (Application)
  → LocalStudentDatabase (Infrastructure)
  → campusdesk-offline.db3 (device app data)
```

When Save, Edit or Delete is clicked, the database records both the local mutation and its queue event in **one transaction**. This avoids a student record being modified while the corresponding pending-sync event is lost.

> **Important boundary:** The Week 04 guide asks for changes to be flagged for future synchronization. This sample records pending changes but does **not** upload them. The Lab 3 API currently exposes read-only GET endpoints. Do not tell students that pressing Save synchronizes the server.

---

## B. Visual Studio installation and setup (no terminal)

### B1. Open the existing solution

1. Start Visual Studio 2026 on Windows.
2. Select **Open a project or solution** and choose your existing `CampusDesk.sln`.
3. Confirm Solution Explorer lists the five existing projects.
4. Save a backup of the Lab 3 solution (File Explorer > Copy the CampusDesk folder). Do not overwrite your only working copy.

### B2. Install the SQLite NuGet package

1. In Solution Explorer, right-click **Infrastructure** and choose **Manage NuGet Packages**.
2. Open **Browse**, search for the exact package ID `sqlite-net-pcl` (author: SQLite-net).
3. Select the latest compatible stable version; this manual targets **1.11.285**, available in October 2026.
4. Click **Install** and accept license prompts; wait until the restore completes.
5. Open the **Installed** tab and confirm `sqlite-net-pcl` appears under Infrastructure. Do not install a similarly named SQLite package by mistake.

Official package guidance:
- Microsoft Learn, ".NET MAUI local databases": <https://learn.microsoft.com/dotnet/maui/data-cloud/database-sqlite>
- NuGet: <https://www.nuget.org/packages/sqlite-net-pcl/>

### B3. Confirm existing project references

If the references were already configured in Lab 1, leave them unchanged. Otherwise use right-click project > Dependencies > Add Project Reference:

| Project | Project references |
|---|---|
| Application | Domain |
| Infrastructure | Application, Domain |
| Presentation.Client | Application, Domain, Infrastructure |
| CampusDesk.Api | Application (from previous labs) |

---

## C. Add the Lab 4 files through Solution Explorer

Extract `CampusDesk_Lab04_Source.zip` using Windows File Explorer (right-click > Extract All). Do not open files directly inside the ZIP. Use **Add > Existing Item** for the NEW files and paste the two REPLACEMENT files into the existing Visual Studio files. This avoids accidental duplicate classes.

| Matching Visual Studio project | New files to add |
|---|---|
| Domain | `OfflineStudent.cs`; `PendingSyncChange.cs` |
| Application | `IOfflineStudentStore.cs`; `OfflineStudentHandler.cs` |
| Infrastructure | `Offline/LocalStudentDatabase.cs` (create Offline folder if absent) |
| Presentation.Client | `Components/Pages/Offline.razor` (add to Pages folder) |

**Files to REPLACE rather than duplicate:**

| Existing file | Action |
|---|---|
| `Presentation.Client/MauiProgram.cs` | Open it, select all content (Ctrl+A), paste `Lab04/Presentation.Client/MauiProgram.cs`, then save. |
| `Presentation.Client/Components/Layout/NavMenu.razor` | Open it, select all content (Ctrl+A), paste `Lab04/Presentation.Client/Components/Layout/NavMenu.razor`, then save. |

1. For each new file: right-click the correct project/folder > Add > Existing Item, browse inside the extracted Lab 4 folder, select the file, then click Add.
2. For Infrastructure, create the `Offline` folder first (right-click Infrastructure > Add > New Folder). Add `LocalStudentDatabase.cs` inside it.
3. For the Razor page, expand Presentation.Client > Components > Pages, choose Add > Existing Item, then add `Offline.razor`.
4. Replace `MauiProgram.cs` and `NavMenu.razor` using the provided texts, then save both files.
5. In Solution Explorer, check all eight source files are visible and there is exactly one `MauiProgram.cs` in Presentation.Client.

---

## D. Build and run Lab 4

1. Right-click **Presentation.Client** and choose **Set as Startup Project**.
2. Choose **Windows Machine** in the Visual Studio launch target selector.
3. Select **Build > Clean Solution**, followed by **Build > Rebuild Solution**.
4. If the Error List contains errors, consult the troubleshooting section before proceeding.
5. Press **F5**. You do NOT need to start `CampusDesk.Api` for this lab.
6. Open **Offline SQLite Students (Lab 4)** in the app navigation bar.

**Expected screen:** an Add Student form, a Saved Students table, and a Pending Synchronization Queue table. Both tables are initially empty the first time you launch the app on a fresh installation.

---

## E. Guided exercises (perform in this exact order)

### Exercise 1 – create a student

1. In Offline SQLite Students, enter Full name = `Ali Ahmed`, Department = `CS` and Email = `ali@example.com`.
2. Click **Save on device**.
3. Verify Ali appears in the Saved Students table and its Sync state says **Pending**.
4. Verify the queue contains one **Create** event referring to Ali's local student ID.

### Exercise 2 – validate input

1. Leave Full name empty and click **Save on device**. You should see a validation message.
2. Enter a valid name but an invalid email such as `ali-not-email`. You should see an email validation message.
3. Verify that invalid attempts do not create extra student records or queue entries.

### Exercise 3 – edit a saved student

1. In the Saved Students table, click **Edit** beside Ali Ahmed.
2. Change the department from CS to IT and click **Update on device**.
3. Verify Ali still has the same local student ID and the new department IT.
4. Verify the queue now contains a Create event and an Update event for that ID.

### Exercise 4 – prove offline persistence

1. Do not run the `CampusDesk.Api` project (it is not required).
2. Stop debugging (Shift + F5), then press F5 to launch the MAUI app again.
3. Open Offline SQLite Students and verify Ali still exists with department IT.
4. Verify both queued events remain. The original Lab 2 in-memory registration records do not have this same persistence guarantee.

### Exercise 5 – delete and inspect the queue

1. Click **Delete** beside Ali.
2. Verify he disappears from Saved Students.
3. Verify a Delete event appears in the queue along with previous Create and Update events.
4. Close and relaunch the app. The student must remain absent, but the three queue events must remain present.

> **Why is the delete event still visible?** The row has been deleted from the student table, but the queue stores a snapshot of the deleted record. A later synchronization service will need this information to tell the server what to remove.

---

## F. Verification checklist and expected results

| Test | Expected result |
|---|---|
| First launch | No stored students or queued actions on a fresh installation. |
| Valid Save | Student inserted; one Create queue event. |
| Invalid Save | Validation error; no student or queue insertion. |
| Edit | Existing student changes; one additional Update queue event. |
| Delete | Student removed; a Delete event remains in pending queue. |
| Close and relaunch | Saved records and queued events remain in SQLite. |
| API stopped | Offline SQLite page still functions; Remote API Students may show a connection error. |
| Database file | `campusdesk-offline.db3` is created inside `FileSystem.AppDataDirectory` on the current device. |

---

## G. Home task: offline-first synchronization queue

The source ZIP implements the course guide's home task, not just the in-class persistence task. Each mutation appends a queue row containing: queue ID, local student ID, operation (Create, Update or Delete), student snapshot, and UTC time. Both local change and queue insert execute in the same SQLite transaction.

**Required independent student demonstration:**

- [ ] Show a newly inserted student and its Create queue event.
- [ ] Update the student and show the additional Update event without changing the student ID.
- [ ] Delete the student and show the persisted Delete event even though the main record is gone.
- [ ] Restart the desktop app and demonstrate that the queue is not erased.
- [ ] Explain why actual HTTP synchronization is not implemented yet and what additional API endpoints would be needed.

> **Design limitation (intentional):** The sample retains all operation events instead of collapsing Create/Update/Delete into a single final event. A future lab can introduce queue coalescing, retries, unique remote IDs, conflict resolution and authentication.

---

## H. Troubleshooting

| Error / symptom | Visual Studio fix |
|---|---|
| CS0246: SQLite namespace missing | Right-click Infrastructure > Manage NuGet Packages > Installed; ensure `sqlite-net-pcl` is installed; rebuild. |
| CS0234: CampusDesk.Domain/Application not found | Verify project references under Dependencies > Projects and file namespaces. |
| CS0246: OfflineStudentHandler missing | Make sure `Application/OfflineStudentHandler.cs` was added to the Application project, not Presentation.Client. |
| CS0246: LocalStudentDatabase missing | Confirm `Infrastructure/Offline/LocalStudentDatabase.cs` is visible inside Infrastructure. |
| CS0118: Application is a namespace | Retain the corrected `App.xaml.cs` from Lab 1; do not revert to the original incorrect version. |
| Duplicate MauiProgram / ambiguous type | Do not add a second `MauiProgram.cs`. Replace the existing file contents. |
| Offline page returns Not Found | Ensure `Offline.razor` is under Components/Pages and contains `@page "/offline"`; rebuild. |
| SQLite error / database not writable | Use `FileSystem.AppDataDirectory`, not a database path under Program Files or a Visual Studio solution folder. |
| Old data remains | The app-data database is persistent by design; using a new package identity or deleting the app's stored data resets it. |
| Remote API page connection error | Lab 4 does not need the API; this only affects Lab 3's Remote API Students page. |

---

## I. Short viva and concept checks

1. Why does a `List<Student>` in memory not provide persistence across app restarts?
2. What does `FileSystem.AppDataDirectory` provide on Windows and mobile devices?
3. What is the role of an `IOfflineStudentStore` interface?
4. Why should a SQLite repository not be directly injected into every Razor UI component?
5. Why are insert/update/delete and queue entry creation performed within the same transaction?
6. How can the synchronization queue retain a Delete action after the student row is deleted?
7. How does the new Offline page differ from Lab 3's remote API page?
8. Why is it misleading to label a record "synced" merely because it was saved in SQLite?
9. What would a backend need to support robust synchronization of offline records?
10. What can go wrong if an offline Create event is retried twice without idempotency?

---

## J. Source code appendix (complete paste-ready files)

The following listings are identical to the companion ZIP. Prefer Add > Existing Item from the ZIP to avoid typographical errors. Existing core files from Labs 1–3 are intentionally not repeated here.

### 1. `Domain/OfflineStudent.cs`

Domain model for a device-local student. It has no SQLite attributes.

```csharp
namespace CampusDesk.Domain;

// A student stored on this device, independently of the remote API.
public sealed record OfflineStudent(
    int Id, string FullName, string Department, string Email, string LastChangedUtc);
```

### 2. `Domain/PendingSyncChange.cs`

Domain model describing one queued, unsynchronized change.

```csharp
namespace CampusDesk.Domain;

// One durable change waiting to be sent to a server in a future lab.
public sealed record PendingSyncChange(
    int Id, int LocalStudentId, string Operation, string FullName,
    string Department, string Email, string CreatedUtc);
```

### 3. `Application/IOfflineStudentStore.cs`

Abstraction separating the business layer from the SQLite implementation.

```csharp
using CampusDesk.Domain;

namespace CampusDesk.Application;

public interface IOfflineStudentStore
{
    Task<IReadOnlyList<OfflineStudent>> GetStudentsAsync();
    Task<IReadOnlyList<PendingSyncChange>> GetPendingChangesAsync();
    Task<OfflineStudent> AddAsync(string name, string department, string email);
    Task UpdateAsync(int id, string name, string department, string email);
    Task DeleteAsync(int id);
}
```

### 4. `Application/OfflineStudentHandler.cs`

Calls the existing `StudentValidator` from Lab 2 and delegates persistence through the interface.

```csharp
using CampusDesk.Domain;

namespace CampusDesk.Application;

// Business workflow is separated from the UI and SQLite implementation.
public sealed class OfflineStudentHandler
{
    private readonly IOfflineStudentStore _store;
    private readonly StudentValidator _validator;

    public OfflineStudentHandler(IOfflineStudentStore store, StudentValidator validator)
    {
        _store = store;
        _validator = validator;
    }

    public Task<IReadOnlyList<OfflineStudent>> GetStudentsAsync() =>
        _store.GetStudentsAsync();

    public Task<IReadOnlyList<PendingSyncChange>> GetQueueAsync() =>
        _store.GetPendingChangesAsync();

    public Task<OfflineStudent> AddAsync(string name, string department, string email)
    {
        _validator.Validate(name, department, email);
        return _store.AddAsync(name.Trim(), department.Trim(), email.Trim());
    }

    public Task UpdateAsync(int id, string name, string department, string email)
    {
        _validator.Validate(name, department, email);
        return _store.UpdateAsync(id, name.Trim(), department.Trim(), email.Trim());
    }

    public Task DeleteAsync(int id) => _store.DeleteAsync(id);
}
```

### 5. `Infrastructure/Offline/LocalStudentDatabase.cs`

SQLite infrastructure implementation with lazy table creation and transactional queue writes.

```csharp
using CampusDesk.Application;
using CampusDesk.Domain;
using SQLite;

namespace CampusDesk.Infrastructure.Offline;

// sqlite-net-pcl package must be installed into the Infrastructure project.
public sealed class LocalStudentDatabase : IOfflineStudentStore
{
    private readonly SQLiteAsyncConnection _db;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    public LocalStudentDatabase(string databasePath)
    {
        _db = new SQLiteAsyncConnection(databasePath,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache);
    }

    private async Task InitializeAsync()
    {
        if (_initialized) return;
        await _initLock.WaitAsync();
        try
        {
            if (_initialized) return;
            await _db.CreateTableAsync<LocalStudentRow>();
            await _db.CreateTableAsync<SyncQueueRow>();
            _initialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<IReadOnlyList<OfflineStudent>> GetStudentsAsync()
    {
        await InitializeAsync();
        var rows = await _db.Table<LocalStudentRow>().OrderBy(s => s.Id).ToListAsync();
        return rows.Select(ToStudent).ToList();
    }

    public async Task<IReadOnlyList<PendingSyncChange>> GetPendingChangesAsync()
    {
        await InitializeAsync();
        var rows = await _db.Table<SyncQueueRow>().OrderBy(c => c.Id).ToListAsync();
        return rows.Select(c => new PendingSyncChange(
            c.Id, c.LocalStudentId, c.Operation, c.FullName,
            c.Department, c.Email, c.CreatedUtc)).ToList();
    }

    public async Task<OfflineStudent> AddAsync(string name, string department, string email)
    {
        await InitializeAsync();
        var row = new LocalStudentRow
        {
            FullName = name,
            Department = department,
            Email = email,
            LastChangedUtc = DateTime.UtcNow.ToString("O")
        };

        // Insert the student and its pending queue item atomically.
        await _db.RunInTransactionAsync(connection =>
        {
            connection.Insert(row); // Fills row.Id (AUTOINCREMENT).
            connection.Insert(MakeQueueRow(row, "Create"));
        });

        return ToStudent(row);
    }

    public async Task UpdateAsync(int id, string name, string department, string email)
    {
        await InitializeAsync();
        await _db.RunInTransactionAsync(connection =>
        {
            var row = connection.Find<LocalStudentRow>(id)
                ?? throw new InvalidOperationException("Student not found.");
            row.FullName = name;
            row.Department = department;
            row.Email = email;
            row.LastChangedUtc = DateTime.UtcNow.ToString("O");
            connection.Update(row);
            connection.Insert(MakeQueueRow(row, "Update"));
        });
    }

    public async Task DeleteAsync(int id)
    {
        await InitializeAsync();
        await _db.RunInTransactionAsync(connection =>
        {
            var row = connection.Find<LocalStudentRow>(id)
                ?? throw new InvalidOperationException("Student not found.");

            // The queue keeps a snapshot even after the local row is removed.
            connection.Insert(MakeQueueRow(row, "Delete"));
            connection.Delete(row);
        });
    }

    private static SyncQueueRow MakeQueueRow(LocalStudentRow student, string operation) =>
        new()
        {
            LocalStudentId = student.Id,
            Operation = operation,
            FullName = student.FullName,
            Department = student.Department,
            Email = student.Email,
            CreatedUtc = DateTime.UtcNow.ToString("O")
        };

    private static OfflineStudent ToStudent(LocalStudentRow row) =>
        new(row.Id, row.FullName, row.Department, row.Email, row.LastChangedUtc);

    // SQLite models stay in Infrastructure, not in the Domain layer.
    public sealed class LocalStudentRow
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LastChangedUtc { get; set; } = string.Empty;
    }

    public sealed class SyncQueueRow
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int LocalStudentId { get; set; }
        public string Operation { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CreatedUtc { get; set; } = string.Empty;
    }
}
```

### 6. `Presentation.Client/Components/Pages/Offline.razor`

Blazor UI component with forms, local data list, edit/delete buttons and queue view.

```razor
@page "/offline"
@using CampusDesk.Domain
@inject CampusDesk.Application.OfflineStudentHandler Handler

<PageTitle>Offline SQLite students</PageTitle>
<h1>Offline students (SQLite)</h1>
<p>This page works without the API. All changes are saved on this device.</p>

<div class="cd-card">
    <h3>@(editingId is null ? "Add student" : "Edit student")</h3>

    <label for="offline-name">Full name</label>
    <input id="offline-name" @bind="fullName" placeholder="e.g. Ali Ahmed" />

    <label for="offline-department">Department</label>
    <select id="offline-department" @bind="department">
        <option value="">Choose department</option>
        <option value="CS">CS</option>
        <option value="IT">IT</option>
        <option value="SE">SE</option>
    </select>

    <label for="offline-email">Email address</label>
    <input id="offline-email" type="email" @bind="email" placeholder="ali@example.com" />

    <button class="cd-button" disabled="@busy" @onclick="SaveAsync">
        @(editingId is null ? "Save on device" : "Update on device")
    </button>
    <button disabled="@busy" @onclick="ClearForm">Clear</button>
</div>

@if (message is not null)
{
    <p role="status">@message</p>
}
@if (error is not null)
{
    <p role="alert" class="cd-error">@error</p>
}
@if (busy)
{
    <p>Working with local SQLite database...</p>
}

<div class="cd-card cd-scroll">
    <h3>Saved students (@students.Count)</h3>
    @if (students.Count == 0)
    {
        <p>No students saved locally yet.</p>
    }
    else
    {
        <table>
            <thead>
                <tr><th>ID</th><th>Name</th><th>Department</th><th>Email</th><th>Sync state</th><th>Actions</th></tr>
            </thead>
            <tbody>
                @foreach (var s in students)
                {
                    <tr>
                        <td>@s.Id</td>
                        <td>@s.FullName</td>
                        <td>@s.Department</td>
                        <td>@s.Email</td>
                        <td>@(queue.Any(q => q.LocalStudentId == s.Id) ? "Pending" : "No changes")</td>
                        <td>
                            <button disabled="@busy" @onclick="() => Edit(s)">Edit</button>
                            <button disabled="@busy" @onclick="() => DeleteAsync(s.Id)">Delete</button>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>

<div class="cd-card cd-scroll">
    <h3>Pending synchronization queue (@queue.Count)</h3>
    <p>These events remain on disk until a future sync implementation processes them.</p>
    @if (queue.Count == 0)
    {
        <p>There are no queued changes.</p>
    }
    else
    {
        <table>
            <thead><tr><th>Queue ID</th><th>Student ID</th><th>Action</th><th>Name</th><th>Created (UTC)</th></tr></thead>
            <tbody>
                @foreach (var q in queue)
                {
                    <tr>
                        <td>@q.Id</td>
                        <td>@q.LocalStudentId</td>
                        <td>@q.Operation</td>
                        <td>@q.FullName</td>
                        <td>@q.CreatedUtc</td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>

@code {
    private List<OfflineStudent> students = new();
    private List<PendingSyncChange> queue = new();
    private int? editingId;
    private string fullName = "";
    private string department = "";
    private string email = "";
    private string? error;
    private string? message;
    private bool busy;

    protected override async Task OnInitializedAsync()
    {
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        try
        {
            students = (await Handler.GetStudentsAsync()).ToList();
            queue = (await Handler.GetQueueAsync()).ToList();
        }
        catch (Exception ex)
        {
            error = "Local database error: " + ex.Message;
        }
    }

    private async Task SaveAsync()
    {
        if (busy) return;
        busy = true;
        error = null;
        message = null;
        try
        {
            if (editingId is int id)
            {
                await Handler.UpdateAsync(id, fullName, department, email);
                message = "Student updated and update queued.";
            }
            else
            {
                await Handler.AddAsync(fullName, department, email);
                message = "Student saved on device and create queued.";
            }
            ClearForm();
            await ReloadAsync();
        }
        catch (Exception ex)
        {
            error = ex.Message;
        }
        finally
        {
            busy = false;
        }
    }

    private void Edit(OfflineStudent student)
    {
        editingId = student.Id;
        fullName = student.FullName;
        department = student.Department;
        email = student.Email;
        message = null;
        error = null;
    }

    private async Task DeleteAsync(int id)
    {
        if (busy) return;
        busy = true;
        error = null;
        message = null;
        try
        {
            await Handler.DeleteAsync(id);
            if (editingId == id) ClearForm();
            message = "Student removed locally and delete queued.";
            await ReloadAsync();
        }
        catch (Exception ex)
        {
            error = ex.Message;
        }
        finally
        {
            busy = false;
        }
    }

    private void ClearForm()
    {
        editingId = null;
        fullName = "";
        department = "";
        email = "";
    }
}
```

### 7. `Presentation.Client/Components/Layout/NavMenu.razor`

Replacement navigation menu that adds the `/offline` route.

```razor
<nav class="cd-nav" aria-label="Main navigation">
    <NavLink href="/" Match="NavLinkMatch.All">Home</NavLink>
    <NavLink href="/platform">Platform &amp; Native Page</NavLink>
    <NavLink href="/register">Local Registration (Lab 2)</NavLink>
    <NavLink href="/students">Remote API Students (Lab 3)</NavLink>
    <NavLink href="/offline">Offline SQLite Students (Lab 4)</NavLink>
</nav>
```

### 8. `Presentation.Client/MauiProgram.cs`

Complete replacement of Lab 3 `MauiProgram.cs`, preserving earlier services and registering SQLite.

```csharp
using CampusDesk.Application;
using CampusDesk.Domain;
using CampusDesk.Infrastructure;
using CampusDesk.Infrastructure.Offline;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
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

        // Lab 2: in-memory repository and local registration.
        builder.Services.AddSingleton<StudentValidator>();
        builder.Services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
        builder.Services.AddTransient<StudentRegistrationHandler>();
        builder.Services.AddTransient<StudentQueryHandler>();

        // Lab 3: separate remote API. Required only for the Remote Students page.
        var baseUrl = DeviceInfo.Current.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5088/"
            : "http://localhost:5088/";
        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(15)
        });
        builder.Services.AddSingleton<IStudentApiClient, HttpStudentApiClient>();

        // Lab 4: persistent database in MAUI's private app-data directory.
        builder.Services.AddSingleton<IOfflineStudentStore>(_ =>
            new LocalStudentDatabase(
                Path.Combine(FileSystem.AppDataDirectory, "campusdesk-offline.db3")));
        builder.Services.AddTransient<OfflineStudentHandler>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
```

---

## Reference and scope notes

- **Primary course material:** Full Stack Course Guide 2026–2027, Full Stack Web Development Lab (CS-301L), page 7, Week 04: Local Storage & Database Sync in MAUI; task focus and home task.
- **Technical reference:** Microsoft Learn, .NET MAUI local databases: <https://learn.microsoft.com/dotnet/maui/data-cloud/database-sqlite>
- **NuGet package reference:** sqlite-net-pcl: <https://www.nuget.org/packages/sqlite-net-pcl/>

> **Build-status disclosure:** The source has been checked for consistency with the Lab 3 companion files but could not be compiled using a Windows MAUI toolchain in the authoring environment. Verify Build > Rebuild Solution and complete the exercise checks in Visual Studio on your Windows machine before teaching.
