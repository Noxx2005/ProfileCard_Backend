# Profile API — Stage 0 Task

## 🚀 Overview
A simple RESTful API built with .NET 8 that returns my profile information and a random cat fact.

## 🧩 Endpoint
**GET** `/me`  
Returns:
```json
{
  "status": "success",
  "user": {
    "email": "your_email@example.com",
    "name": "Your Full Name",
    "stack": ".NET Core 8 / ASP.NET Core Web API"
  },
  "timestamp": "2025-10-19T12:00:00.000Z",
  "fact": "Cats sleep for 70% of their lives."
}
