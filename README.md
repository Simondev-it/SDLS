[README_SDLS.md](https://github.com/user-attachments/files/26450317/README_SDLS.md)
# 🚗 Smart Driving Learning System (SDLS)

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8-blue?style=for-the-badge&logo=dotnet" />
  <img src="https://img.shields.io/badge/PostgreSQL-DB-blue?style=for-the-badge&logo=postgresql" />
  <img src="https://img.shields.io/badge/JWT-Auth-green?style=for-the-badge&logo=jsonwebtokens" />
  <img src="https://img.shields.io/badge/AI-Gemini%20%7C%20Ollama-purple?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Docker-Ready-blue?style=for-the-badge&logo=docker" />
</p>

---

## 🌟 Overview

**Smart Driving Learning System (SDLS)** is an intelligent platform for learning and practicing driving theory, integrated with AI to personalize the learning experience.

💡 The system is built with an **API-first approach**, making it easy to scale and integrate with frontend applications (React, Vue, Mobile Apps).

---

## ✨ Features

### 👤 User
- 🔐 Register / Login (JWT)
- 📝 Practice tests
- 📊 View results & progress
- 🤖 AI chatbot support

### 📚 Learning
- Topic-based learning
- Auto-generated quizzes
- AI-generated questions
- Weakness analysis

### 🤖 AI System
- Learning support chatbot
- Personalized recommendations
- Behavior analysis
- Smart exam generation

### 💳 Payment
- VNPay / PayOS integration
- Payment callback handling
- Payment status update

### 🛠️ Admin / Staff
- Content management
- User management
- Content approval
- Analytics dashboard

---

## 🧱 Architecture

```mermaid
graph TD
    A[Client] --> B[Controller]
    B --> C[Service]
    C --> D[Repository]
    D --> E[(Database)]
    C --> F[AI Service]
```

Architecture pattern:

```
Controller → Service → Repository → Database
```

---

## 🖼️ Demo UI (Mockup)

### 📱 User Dashboard
![Dashboard](https://via.placeholder.com/900x400.png?text=User+Dashboard+UI)

### 🧠 AI Chatbot
![AI Chat](https://via.placeholder.com/900x400.png?text=AI+Chatbot+UI)

### 📝 Quiz System
![Quiz](https://via.placeholder.com/900x400.png?text=Quiz+System+UI)

---

## ⚙️ Tech Stack

| Layer       | Technology |
|------------|------------|
| Backend     | ASP.NET Core (.NET 8) |
| Database    | PostgreSQL |
| ORM         | Entity Framework Core |
| Auth        | JWT Bearer |
| AI          | Gemini API / Ollama |
| DevOps      | Docker, Railway |
| Mapping     | AutoMapper |

---

## 🔑 Authentication

- JWT Bearer Token
- Role-based authorization:
  - Admin
  - Staff
  - User

---

## 🔄 Payment Flow

```mermaid
sequenceDiagram
    participant U as User
    participant API
    participant Pay as Payment Gateway

    U->>API: Create Payment
    API->>Pay: Request Payment URL
    U->>Pay: Pay
    Pay->>API: Callback
    API->>API: Update Status
```

---

## 🤖 AI Flow

```mermaid
sequenceDiagram
    participant U as User
    participant API
    participant AI

    U->>API: Ask Question
    API->>AI: Send Prompt
    AI-->>API: Response
    API-->>U: Answer
```

---

## 🚀 Getting Started

### 1. Clone repository
```
git clone <your-repo>
cd SDLS
```

### 2. Install dependencies
```
dotnet restore
```

### 3. Configure database

Edit `appsettings.json`:

```
"ConnectionStrings": {
  "DefaultConnection": "Host=...;Port=5432;Database=...;Username=...;Password=..."
}
```

### 4. Run migrations
```
dotnet ef database update
```

### 5. Run project
```
dotnet run
```

---

## 📡 API Documentation

Swagger:
```
https://localhost:<port>/swagger
```

---

## 📁 Project Structure

```
SDLS/
│
├── SDLS.API
│   ├── Controllers
│
├── SDLS.Services
│   ├── Interfaces
│   ├── Implementations
│
├── SDLS.Repositories
│   ├── Interfaces
│   ├── Implementations
│
├── SDLS.Model
│   ├── Models
│   ├── DTOs
│
└── SDLS.Database
```

---

## 🔮 Future Enhancements

- 📱 Mobile App (Flutter / React Native)
- 🧠 Advanced AI Recommendations
- 🎥 Video Learning
- 🏆 Gamification
- 🌐 Multi-language support

---

## 📄 License

This project is for educational purposes.

---

## ⭐ Highlights

> 🚀 Clean architecture  
> 🤖 Deep AI integration  
> ⚡ Optimized backend API  
> 🔥 Suitable for academic & production projects  
