# Project Overview
A cconsole-based To-Do and Kanban management system built in C#. The main rule was: "No built-in collections (`System.Collection.Generic`)" (`List<T>`, `Dictionary<T>`, LINQ, etc.)

# Architecture
The project follows a Clean Architecture split:
- **Model** — `TaskItem`, `User`, enums, ViewModels
- **Repository** — `IGenericRepository<T>` with a JSON file implementation
- **Service** — business logic using custom collections `TaskService`, `UserService`, `IMyCollection`
- **View** — console views (task CRUD, Kanban board, user management, dependency menu)
- **AppController / Program** — wires everything together
- 
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

# Key Technical Decisions & Deviations
# Persistence
# How to Run
