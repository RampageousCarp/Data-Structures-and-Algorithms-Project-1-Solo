# Project Overview
A cconsole-based To-Do and Kanban management system built in C#. The main rule was: "No built-in collections (`System.Collection.Generic`)" (`List<T>`, `Dictionary<T>`, LINQ, etc.)

# Architecture
The project follows a Clean Architecture split:
- **Model** — `TaskItem`, `User`, enums, ViewModels
- **Repository** — `IGenericRepository<T>` with a JSON file implementation
- **Service** — business logic using custom collections `TaskService`, `UserService`, `IMyCollection`
- **View** — console views (task CRUD, Kanban board, user management, dependency menu)
- **AppController / Program** — wires everything together
  
# Custom Collections
`IMyCollection<T>` interface implementations:
| Class | Sort algorithm |
|---|---|
| `MyArrayCollection` | QuickSort (in-place) |
| `MyLinkedListCollection` | MergeSort |
| `MyBSTCollection` | None |
| `MyBSTCollectionSortable` | Cached QuickSort snapshot |
| `MyHashMapCollection` | None |
| `MyHashMapCollectionSortable` | Cached QuickSort snapshot |

# Implemented Functionalities
## Task Management (base)
- Add, update, and remove tasks
- Each task has a description, priority (Low / Medium / High), status (Not Started / In Progress / Done), due date, and creation date
- Toggle task status directly from the list view
- Filter tasks by status, priority, assignee, keyword, and due/created date ranges
- Sort tasks by any field (ID, description, priority, created date, due date, assignee) in ascending or descending order
- JSON persistence — tasks are saved to a file and reloaded on startup; auto-save triggers every 10 changes
## Kanban Board
- Tasks are grouped into three columns: **To Do**, **In Progress**, **Done**
- Filters and sorting also apply to the Kanban view
- Moving a task across columns updates its status
## User Management & Assignment
- Add and remove users
- Assign or unassign a task to a specific user
- Only the assigned user (or any user if the task is unassigned) can edit or remove a task
- Removing a user automatically unassigns all their tasks
## Task Dependencies
- Add dependencies between tasks (a task can depend on multiple others)
- A task is shown as **BLOCKED** if any of its dependencies are not yet done
- Blocked tasks list which tasks are blocking them
- Cycle detection — adding a dependency that would create a circular chain is rejected
- Remove individual dependencies or clear all at once

# How to Run

Requires .NET 10 SDK.
 
```
dotnet run --project Project1
```
 
JSON files are created automatically in `Repositories/JSON/` on first save.
 
