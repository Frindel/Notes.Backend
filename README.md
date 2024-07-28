# Описание проекта

Данный проект представляет собой пример решения тривиальной задачи управления заметками. Сервис реализован в виде REST API на базе ASP.NET Core Web API.

### Используемые технологии:
- MediatoR
- FluentValidation
- AutoMapper
- Entity Framework

Сервис работает с тремя типами данных: пользователи, заметки и категории заметок. У каждой заметки и категории есть уникальный идентификатор (PersonalId), принадлежащий пользователю. Для всех методов, кроме `POST /api/users/register`, необходимо устанавливать заголовок `Authorization: Bearer [access-token]`.

## Развертывание проекта

Проект развертывается с помощью Docker-контейнеров: `notes-backend` и `notes-db`. В качестве базы данных используется PostgreSQL, данные которой хранятся в папке `../database`, расположенной рядом с папкой проекта. Также имеется возможность использовать Devcontainer для тестирования проекта.

## Пользователи

| Атрибут      | Тип    |
| ------------ | -------|
| id           | int    |
| name         | string |
| password     | string |
| refresh_token| string |

Каждая заметка и категория принадлежат конкретному пользователю. Для создания пользователей используется `POST /api/users/register`. Для аутентификации используются JWT токены: краткосрочные (access-токен) и долгосрочные (refresh-токен). Получить токены можно при регистрации (`POST /api/users/register`) и авторизации (`POST /api/users/login`).

### Методы пользователей

#### POST /api/users/register
Используется для регистрации пользователя. Возвращает access и refresh токены.

**Запрос:**
```json
{
  "login": "person 1",
  "password": "123456"
}
```
**Ответ:**
```json
{
  "login": "person 1",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5..."
}
```
**Ошибки:** передан пустой логин или пароль (400); указанный логин уже используется (409).

#### POST /api/users/login
Используется для получения access и refresh токенов.

**Запрос:**
```json
{
  "login": "person 1",
  "password": "123456"
}
```
**Ответ:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5..."
}
```
**Ошибки:** передан пустой логин или пароль (400); неверный логин или пароль (403).

#### POST /api/users/update-tokens
Выполняет переиздание access и refresh токенов по refresh-токену.

**Запрос:**
```json
{
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5..."
}
```
**Ответ:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5..."
}
```
**Ошибки:** передан пустой refresh-токен, либо он не валиден (400).

## Категории заметок

| Атрибут     | Тип    |
| ------------| -------|
| id          | int    |
| personal_id | int    |
| user_id     | int    |
| name        | string |

### Методы категорий

#### GET /api/categories
Возвращает все категории пользователя.

**Ответ:**
```json
[
  {
    "id": 3,
    "name": "category name",
    "color": "#AAA"
  }
  ...
]
```

#### GET /api/categories/{categoryId}
Возвращает информацию о конкретной категории.

**Ответ:**
```json
{
  "id": 3,
  "name": "category name",
  "color": "#AAA"
}
```
**Ошибки:** отрицательное id категории (400).

#### POST /api/categories/
Добавляет новую категорию.

**Запрос:**
```json
{
  "name": "category name",
  "color": "#AAA"
}
```
**Ответ:**
```json
{
  "id": 5,
  "name": "category name",
  "color": "#AAA"
}
```
**Ошибки:** передано пустое имя категории, либо категория с данным названием уже существует (400).

## Заметки

| Атрибут     | Тип      |
| ------------| ---------|
| id          | int      |
| personal_id | int      |
| user_id     | int      |
| name        | string   |
| description | string   |
| time        | datetime |
| is_completed| int      |

### Методы заметок

#### GET /api/notes
Возвращает все заметки пользователя.

**Ответ:**
```json
[
  {
    "id": 1,
    "name": "note name",
    "description": "note description",
    "time": "2024-07-26T19:10:57.869Z",
    "isCompleted": false,
    "categories": [1, 2]
  }
  ...
]
```

#### GET /api/notes/{noteId}
Возвращает информацию о конкретной заметке.

**Ответ:**
```json
{
  "id": 1,
  "name": "note name",
  "description": "note description",
  "time": "2024-07-26T19:10:57.869Z",
  "isCompleted": false,
  "categories": [1, 2]
}
```
**Ошибки:** отрицательное id заметки (400).

#### POST /api/notes
Добавляет заметку.

**Запрос:**
```json
{
  "name": "note name",
  "description": "note description",
  "time": "2024-07-26T19:10:57.869Z",
  "categoriesIds": [1, 2]
}
```
**Ответ:** 
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
**Ошибки:** передано пустое имя/описание категории (400); указанные категории не найдены (404).

#### PUT /api/notes
Редактирует заметку.

**Запрос:**
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
**Ответ:**
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
**Ошибки:** передано пустое имя/описание категории, либо id заметки отрицательно (400); указанные категории или заметка не найдены (404).

#### DELETE /api/notes/{noteId}
Удаляет заметку.

_При удалении заметки также удаляются относящиеся к ней категории, при условии, что они не присутствуют у других заметок._

**Ответ:** отсутствует тело ответа.

**Ошибки:** id заметки отрицательно (400); заметка не найдена (404).
