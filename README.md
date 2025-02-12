# Project description

This project is an example solution for a simple note management task. The service is implemented as a REST API based on ASP.NET Core Web API.

### Technologies used:
- MediatoR
- FluentValidation
- AutoMapper
- Entity Framework

The service works with three types of data: users, notes, and note categories. Each note and category has a unique identifier (`PersonalId`) that belongs to a user.
For all methods except `POST /api/users/register`, the `Authorization: Bearer [access-token]` header must be set.

## Project deployment

The project is deployed using Docker containers: `notes-backend` and `notes-db`.
PostgreSQL is used as the database, and its data is stored in the `../database` folder located next to the project folder.
It is also possible to use a Devcontainer for project testing.

To populate the database, execute the following commands in the project's root folder:
```
docker compose up
```
```
dotnet tool install --global dotnet-ef
```
```
dotnet ef database update --project ./Notes.Persistence/Notes.Persistence.csproj --startup-project 
./Notes.Persistence/Notes.Persistence.csproj
```

## Users

| Attribute      | Type    |
| -------------- | ------- |
| id             | int     |
| name           | string  |
| password       | string  |
| refresh_token  | string  |

Each note and category belongs to a specific user.
Users are created using `POST /api/users/register`.
JWT tokens are used for authentication: short-term (access token) and long-term (refresh token).
Tokens can be obtained during registration (`POST /api/users/register`) and login (`POST /api/users/login`).

### User methods

#### POST /api/users/register
Used for user registration. Returns access and refresh tokens.

**Request:**
```json
{
  "login": "person 1",
  "password": "123456"
}
```
**Response:**
```json
{
  "login": "person 1",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5..."
}
```
**Errors:** empty login or password provided (400); login already in use (409).

#### POST /api/users/login
Used to obtain access and refresh tokens.

**Request:**
```json
{
  "login": "person 1",
  "password": "123456"
}
```
**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5..."
}
```
**Errors:** empty login or password provided (400); incorrect login or password (403).

#### POST /api/users/update-tokens
Refreshes access and refresh tokens using a refresh token.

**Request:**
```json
{
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5..."
}
```
**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5..."
}
```
**Errors:** empty or invalid refresh token provided (400).

## Note categories

| Атрибут     | Тип    |
| ------------| -------|
| id          | int    |
| personal_id | int    |
| user_id     | int    |
| name        | string |

### Category methods

#### GET /api/categories?pageNumber={pageNumber}&pageSize={pageSize}
Returns all user categories with pagination.
`pageNumber` and `pageSize` are optional. Defaults: `pageNumber = 1`, `pageSize = 20`.

**Response:**
```json
[
  {
    "id": 3,
    "name": "category name",
    "color": "#AAAAAA"
  }
  ...
]
```
**Errors:** negative page number or size (400).

#### GET /api/categories/{categoryId}
Returns information about a specific category.

**Response:**
```json
{
  "id": 3,
  "name": "category name",
  "color": "#AAAAAA"
}
```
**Errors:** negative category ID (400).

#### POST /api/categories/
Adds a new category.

**Request:**
```json
{
  "name": "category name",
  "color": "#AAAAAA"
}
```
**Response:**
```json
{
  "id": 5,
  "name": "category name",
  "color": "#AAAAAA"
}
```
**Errors:** empty category name, category with this name already exists, or invalid color format (400).

## Notes

| Attribute     | Type      |
| ------------- | --------- |
| id            | int       |
| personal_id   | int       |
| user_id       | int       |
| name          | string    |
| description   | string    |
| time          | datetime  |
| is_completed  | int       |

### Note methods

#### GET /api/notes?pageNumber={pageNumber}&pageSize={pageSize}&categories[]={categoryId}
Returns all user notes.
`pageNumber`, `pageSize`, and `categories` are optional. Defaults: `pageNumber = 1`, `pageSize = 20`.
If `categories` is empty, notes from all user categories are selected.

**Response:**
```json
[
  {
    "id": 1,
    "name": "note name",
    "description": "note description",
    "time": "2024-07-26T19:10:57.869Z",
    "isCompleted": false,
    "categories": [
      {
        "id": 1,
        "name": "category name",
        "color": "#AAAAAA"
      }
      ...
    ]
  }
  ...
]
```
**Errors:** negative page number or size (400); specified categories not found (404).

#### GET /api/notes/{noteId}
Returns information about a specific note.

**Response:**
```json
{
  "id": 1,
  "name": "note name",
  "description": "note description",
  "time": "2024-07-26T19:10:57.869Z",
  "isCompleted": false,
  "categories": [
      {
        "id": 1,
        "name": "category name",
        "color": "#AAAAAA"
      }
      ...
    ]
}
```
**Errors:** negative note ID (400).

#### POST /api/notes
Adds a new note.

**Request:**
```json
{
  "name": "note name",
  "description": "note description",
  "time": "2024-07-26T19:10:57.869Z",
  "categoriesIds": [1, 2]
}
```
**Response:** 
```json
{
  "id": 1,
  "name": "note name",
  "description": "note description",
  "time": "2024-07-26T19:10:57.869Z",
  "isCompleted": false,
  "categoriesIds": [1, 2]
}
```
**Errors:** empty note name/description (400); specified categories not found (404).

#### PUT /api/notes
Edits a note.

**Request:**
```json
{
  "id": 1,
  "name": "note name",
  "description": "note description",
  "time": "2024-07-26T19:10:57.869Z",
  "isCompleted": false,
  "categoriesIds": [1, 2]
}
```
**Response:**
```json
{
  "id": 1,
  "name": "note name",
  "description": "note description",
  "time": "2024-07-26T19:10:57.869Z",
  "isCompleted": false,
  "categoriesIds": [1, 2]
}
```
**Errors:** empty note name/description, or negative note ID (400); specified categories or note not found (404).

#### DELETE /api/notes/{noteId}
Deletes a note.

_Deleting a note also removes its associated categories if they are not used by other notes._

**Response:** no response body.

**Errors:** negative note ID (400); note not found (404).
