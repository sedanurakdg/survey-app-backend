# SurveyApp Backend

SurveyApp, kullanıcıların kendilerine atanan anketleri doldurabildiği ve yöneticilerin anket/soru/rapor yönetimi yapabildiği bir **ASP.NET Core 8 Web API** uygulamasıdır.

## 🚀 Teknoloji Stack

- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server**
- **ASP.NET Core Identity**
- **JWT Authentication**
- **FluentValidation**
- **Swagger / OpenAPI**

---

## 🧱 Mimari

Proje **Clean Architecture** prensiplerine uygun olarak katmanlı tasarlanmıştır.

SurveyApp
│
├── SurveyApp.Api → HTTP layer (Controllers, Auth, Swagger)
├── SurveyApp.Application → Business logic, Services, DTOs
├── SurveyApp.Core → Domain (Entities, Interfaces, Abstractions)
├── SurveyApp.Infrastructure → EF Core, Repositories, Identity, Persistence
└── SurveyApp.sln



### Katman Sorumlulukları

#### **SurveyApp.Core**
- Entity’ler (`Survey`, `Question`, `AnswerTemplate`, vb.)
- Repository interface’leri
- Domain abstraction’ları

#### **SurveyApp.Application**
- Use-case’ler / Service’ler
- DTO’lar
- Validation (FluentValidation)
- Business kuralları

#### **SurveyApp.Infrastructure**
- `DbContext`
- EF Core repository implementasyonları
- Identity & Authentication altyapısı
- Migration’lar

#### **SurveyApp.Api**
- Controllers
- Auth işlemleri
- Swagger
- Middleware pipeline

---

## 🔐 Authentication & Authorization

- **JWT Bearer Authentication**
- **ASP.NET Core Identity**
- Rol bazlı yetkilendirme

### Roller
- **Admin**
- **User**

JWT token içinde rol bilgisi `ClaimTypes.Role` olarak taşınır.

---

## 📌 API Endpoints

### 🔑 Auth

| Method | Endpoint | Açıklama |
|------|---------|---------|
| POST | `/api/auth/register` | Yeni kullanıcı oluşturur |
| POST | `/api/auth/login` | JWT token üretir |

**Register Body**
```json
{
  "email": "user1@surveyapp.local",
  "password": "Test123!"
}
