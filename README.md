# Order Backend API

這是一個使用 ASP.NET Core 10.0 和 PostgreSQL 建立的 RESTful API 專案，用於管理病人資料和醫囑系統。

## 📋 功能特色

- ✅ 病人管理 API (Patient Management)
- ✅ 醫囑管理 API (Medical Order Management)
- ✅ 病人與醫囑的關聯查詢 (一對多關係)
- ✅ PostgreSQL 資料庫整合
- ✅ Entity Framework Core ORM
- ✅ Request/Response 日誌記錄 Middleware
- ✅ OpenAPI/Swagger 文檔支援
- ✅ 外鍵約束與串聯刪除

## 🛠️ 技術堆疊

- **框架**: .NET 10.0
- **資料庫**: PostgreSQL
- **ORM**: Entity Framework Core 10.0
- **API 風格**: RESTful
- **文檔**: OpenAPI (Swagger)

## 📦 專案結構

```
OrderBackend/
├── Data/
│   └── ApplicationDbContext.cs      # 資料庫上下文
├── Models/
│   ├── Patient.cs                   # 病人模型
│   └── MedicalOrder.cs              # 醫囑模型
├── Middleware/
│   └── RequestResponseLoggingMiddleware.cs  # 日誌記錄中間件
├── Program.cs                       # 應用程式進入點
├── appsettings.json                 # 應用程式設定
└── OrderBackend.csproj              # 專案檔
```

## 📝 資料模型

### Patient (病人)
```json
{
  "id": "1",
  "name": "小民",
  "orderId": "1"
}
```

### MedicalOrder (醫囑)
```json
{
  "id": "1",
  "message": "超過120請施打8u",
  "patientId": "1"
}
```

### 資料關聯
- 一個病人 (Patient) 可以有多個醫囑 (MedicalOrder)
- MedicalOrder 透過 `patientId` 外鍵關聯到 Patient
- 刪除病人時會自動串聯刪除相關的醫囑 (Cascade Delete)

## 🚀 開始使用

### 前置需求

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) 資料庫

### 安裝步驟

1. **複製專案**
```bash
git clone <repository-url>
cd jubobackend/OrderBackend
```

2. **設定資料庫連線**

編輯 `appsettings.json`，更新 PostgreSQL 連線字串：
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=orderdb;Username=postgres;Password=your_password"
  }
}
```

3. **安裝相依套件**
```bash
dotnet restore
```

4. **建立資料庫 Migration**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

5. **執行應用程式**
```bash
dotnet run
```

應用程式將在 `https://localhost:5001` 啟動。

## 📡 API 端點

### 病人 API (Patient)

| Method | Endpoint | 說明 |
|--------|----------|------|
| GET | `/api/patients` | 取得所有病人 |
| GET | `/api/patients/{id}` | 取得特定病人 |
| POST | `/api/patients` | 新增病人 |
| PUT | `/api/patients/{id}` | 更新病人資料 |
| DELETE | `/api/patients/{id}` | 刪除病人 |

### 醫囑 API (Medical Order)

| Method | Endpoint | 說明 |
|--------|----------|------|
| GET | `/api/medicalorders` | 取得所有醫囑 |
| GET | `/api/medicalorders/{id}` | 取得特定醫囑 |
| GET | `/api/patients/{patientId}/medicalorders` | 取得特定病人的所有醫囑 |
| POST | `/api/medicalorders` | 新增醫囑 |
| PUT | `/api/medicalorders/{id}` | 更新醫囑 |
| DELETE | `/api/medicalorders/{id}` | 刪除醫囑 |

## 💡 使用範例

### 新增病人
```bash
curl -X POST https://localhost:5001/api/patients \
  -H "Content-Type: application/json" \
  -d '{
    "id": "1",
    "name": "小民",
    "orderId": "1"
  }'
```

### 取得所有病人
```bash
curl https://localhost:5001/api/patients
```

### 新增醫囑
```bash
curl -X POST https://localhost:5001/api/medicalorders \
  -H "Content-Type: application/json" \
  -d '{
    "id": "1",
    "message": "超過120請施打8u",
    "patientId": "1"
  }'
```

### 取得特定病人的所有醫囑
```bash
curl https://localhost:5001/api/patients/1/medicalorders
```

### 回應範例
```json
[
  {
    "id": "1",
    "message": "超過120請施打8u",
    "patientId": "1"
  },
  {
    "id": "2",
    "message": "每日測量血壓",
    "patientId": "1"
  }
]
```

## 🔍 開發工具

### OpenAPI 文檔
在開發模式下，可以存取 OpenAPI 規格：
- OpenAPI JSON: `https://localhost:5001/openapi/v1.json`

### 日誌記錄
系統會自動記錄所有 HTTP Request 和 Response，包括：
- HTTP Method 和 Path
- Request/Response Headers
- Request/Response Body
- 處理時間

## 🗄️ 資料庫指令

### 新增 Migration
```bash
dotnet ef migrations add <MigrationName>
```

### 更新資料庫
```bash
dotnet ef database update
```

### 回復 Migration
```bash
dotnet ef database update <PreviousMigrationName>
```

### 刪除最後一個 Migration
```bash
dotnet ef migrations remove
```

## 🧪 測試

```bash
dotnet test
```

## 📄 授權

此專案採用 MIT 授權條款。

## 👥 貢獻者

- YoungChen-Git

## 📞 聯絡方式

如有問題或建議，請開啟 Issue 或 Pull Request。
