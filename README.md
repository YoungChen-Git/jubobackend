# Order Backend API

這是一個使用 ASP.NET Core 10.0 和 PostgreSQL 建立的 RESTful API 專案，提供病患管理、醫囑管理和 JWT 身份驗證功能。

## 📋 功能特色

- ✅ JWT 身份驗證與授權 (Authentication & Authorization)
- ✅ 使用者註冊與登入
- ✅ 病患管理 API (Patient Management)
- ✅ 醫囑管理 API (Medical Order Management)
- ✅ 病患與醫囑的關聯查詢 (一對多關係)
- ✅ PostgreSQL 資料庫整合
- ✅ Entity Framework Core ORM
- ✅ 自動遞增 ID (Auto-increment Primary Keys)
- ✅ Request/Response 日誌記錄 Middleware
- ✅ OpenAPI/Swagger 文檔支援
- ✅ 外鍵約束與串聯刪除

## 🛠️ 技術堆疊

- **框架**: .NET 10.0
- **資料庫**: PostgreSQL
- **ORM**: Entity Framework Core 10.0
- **身份驗證**: JWT Bearer Token
- **密碼雜湊**: SHA256 (建議生產環境使用 BCrypt 或 Argon2)
- **API 風格**: RESTful
- **文檔**: OpenAPI (Swagger)

## 📦 專案結構

```
OrderBackend/
├── Data/
│   └── ApplicationDbContext.cs      # 資料庫上下文
├── Models/
│   ├── User.cs                      # 使用者模型
│   ├── Patient.cs                   # 病患模型
│   ├── MedicalOrder.cs              # 醫囑模型
│   ├── JwtSettings.cs               # JWT 設定模型
│   ├── LoginRequest.cs              # 登入請求 DTO
│   ├── PatientCreateRequest.cs     # 病患建立請求 DTO
│   ├── PatientUpdateRequest.cs     # 病患更新請求 DTO
│   ├── MedicalOrderCreateRequest.cs # 醫囑建立請求 DTO
│   └── MedicalOrderUpdateRequest.cs # 醫囑更新請求 DTO
├── Services/
│   └── JwtTokenService.cs           # JWT Token 服務
├── Middleware/
│   └── RequestResponseLoggingMiddleware.cs  # 日誌記錄中間件
├── Migrations/                      # EF Core 遷移檔案
├── Program.cs                       # 應用程式進入點
├── appsettings.json                 # 應用程式設定
├── OrderBackend.http                # HTTP 測試檔案
└── OrderBackend.csproj              # 專案檔
```

## 📝 資料模型

### User (使用者)
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "username": "testuser",
  "email": "test@example.com",
  "passwordHash": "hashed_password",
  "createdAt": "2026-01-15T00:00:00Z"
}
```

### Patient (病患)
```json
{
  "id": 1,
  "name": "張三",
  "medicalOrders": []
}
```

### MedicalOrder (醫囑)
```json
{
  "id": 1,
  "message": "每日服藥三次",
  "patientId": 1,
  "patient": null
}
```

### 資料關聯
- 一個病患 (Patient) 可以有多個醫囑 (MedicalOrder)
- MedicalOrder 透過 `patientId` 外鍵關聯到 Patient
- 刪除病患時會自動串聯刪除相關的醫囑 (Cascade Delete)
- ID 使用資料庫自動遞增的整數

## 🚀 開始使用

### 前置需求

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) 資料庫
- [Entity Framework Core Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)

### 安裝步驟

1. **複製專案**
```bash
git clone <repository-url>
cd jubobackend/OrderBackend
```

2. **設定資料庫連線**

編輯 `appsettings.json`，更新 PostgreSQL 連線字串和 JWT 設定：
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=orderbackend;Username=postgres;Password=your_password"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-at-least-32-characters-long",
    "Issuer": "OrderBackendAPI",
    "Audience": "OrderBackendClients",
    "ExpirationMinutes": 60
  }
}
```

⚠️ **重要**: JWT SecretKey 必須至少 32 個字元（256 bits）

3. **安裝 EF Core Tools**
```bash
dotnet tool install --global dotnet-ef
```

4. **安裝相依套件**
```bash
dotnet restore
```

5. **套用資料庫遷移**
```bash
dotnet ef database update
```

6. **執行應用程式**
```bash
dotnet run
```

應用程式將在 `http://localhost:5284` 啟動。

## 📡 API 端點

### 身份驗證 API (Authentication)

| Method | Endpoint | 說明 | 需要授權 |
|--------|----------|------|---------|
| POST | `/api/auth/register` | 註冊新使用者 | ❌ |
| POST | `/api/auth/login` | 使用者登入 | ❌ |

### 病患 API (Patient)

| Method | Endpoint | 說明 | 需要授權 |
|--------|----------|------|---------|
| GET | `/api/patients` | 取得所有病患（含醫囑） | ✅ |
| GET | `/api/patients/{id}` | 取得特定病患（含醫囑） | ✅ |
| POST | `/api/patients` | 新增病患 | ✅ |
| PUT | `/api/patients/{id}` | 更新病患資料 | ✅ |
| DELETE | `/api/patients/{id}` | 刪除病患 | ✅ |

### 醫囑 API (Medical Order)

| Method | Endpoint | 說明 | 需要授權 |
|--------|----------|------|---------|
| GET | `/api/medicalorders` | 取得所有醫囑 | ✅ |
| GET | `/api/medicalorders/{id}` | 取得特定醫囑 | ✅ |
| GET | `/api/patients/{patientId}/medicalorders` | 取得特定病患的所有醫囑 | ✅ |
| POST | `/api/medicalorders` | 新增醫囑 | ✅ |
| PUT | `/api/medicalorders/{id}` | 更新醫囑 | ✅ |
| DELETE | `/api/medicalorders/{id}` | 刪除醫囑 | ✅ |

## 💡 使用範例

### 1. 註冊使用者
```bash
curl -X POST http://localhost:5284/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "passwordHash": "Password123"
  }'
```

回應：
```json
{
  "message": "User registered successfully",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "username": "testuser",
    "email": "test@example.com"
  }
}
```

### 2. 登入
```bash
curl -X POST http://localhost:5284/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "Password123"
  }'
```

### 3. 新增病患（需要 Token）
```bash
curl -X POST http://localhost:5284/api/patients \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{
    "name": "張三"
  }'
```

回應：
```json
{
  "id": 1,
  "name": "張三",
  "medicalOrders": []
}
```

### 4. 取得所有病患
```bash
curl http://localhost:5284/api/patients \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### 5. 新增醫囑
```bash
curl -X POST http://localhost:5284/api/medicalorders \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{
    "message": "每日服藥三次",
    "patientId": 1
  }'
```

回應：
```json
{
  "id": 1,
  "message": "每日服藥三次",
  "patientId": 1,
  "patient": null
}
```

### 6. 取得特定病患的所有醫囑
```bash
curl http://localhost:5284/api/patients/1/medicalorders \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

回應範例：
```json
[
  {
    "id": 1,
    "message": "每日服藥三次",
    "patientId": 1,
    "patient": null
  },
  {
    "id": 2,
    "message": "飯後服用",
    "patientId": 1,
    "patient": null
  }
]
```

## 🧪 測試 API

專案包含 `OrderBackend.http` 檔案，可以使用 VS Code 的 REST Client 擴充套件進行測試：

1. 安裝 [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) 擴充套件
2. 開啟 `OrderBackend.http`
3. 點擊 `Send Request` 來測試各個 API 端點

測試流程：
1. 先執行註冊或登入取得 Token
2. 將 Token 填入 `@token` 變數
3. 測試其他需要授權的端點

## 🔍 開發工具

### OpenAPI 文檔
在開發模式下，可以存取 OpenAPI 規格：
- OpenAPI JSON: `http://localhost:5284/openapi/v1.json`

### 日誌記錄
系統會自動記錄所有 HTTP Request 和 Response，包括：
- HTTP Method 和 Path
- Request/Response Headers
- Request/Response Body
- 處理時間

日誌輸出範例：
```
HTTP Request Information:
Method: POST
Path: /api/auth/login
Headers: Content-Type: application/json
Body: {"username":"testuser","password":"Password123"}

HTTP Response Information:
StatusCode: 200
Headers: Content-Type: application/json
Body: {"token":"eyJhbGc...","user":{...}}
```

## 🗄️ 資料庫指令

### 查看所有 Migrations
```bash
dotnet ef migrations list
```

### 新增 Migration
```bash
dotnet ef migrations add <MigrationName>
```

### 更新資料庫
```bash
dotnet ef database update
```

### 回復到特定 Migration
```bash
dotnet ef database update <MigrationName>
```

### 刪除最後一個 Migration
```bash
dotnet ef migrations remove
```

### 刪除資料庫
```bash
dotnet ef database drop
```

## 🔐 安全性建議

### 生產環境注意事項

1. **JWT SecretKey**
   - 使用至少 256 bits (32 字元) 的強密鑰
   - 儲存在環境變數或 Azure Key Vault
   - 定期輪換密鑰

2. **密碼雜湊**
   - 目前使用 SHA256，建議改用 BCrypt 或 Argon2
   - 加入 Salt 提高安全性

3. **HTTPS**
   - 生產環境必須使用 HTTPS
   - 配置 SSL/TLS 憑證

4. **資料庫**
   - 使用強密碼
   - 限制資料庫存取權限
   - 定期備份

5. **授權**
   - 所有敏感操作都需要 JWT Token 驗證
   - 實作角色權限管理 (Role-based Access Control)

## 🎯 未來改進

- [ ] 實作 Refresh Token 機制
- [ ] 加入角色權限管理 (Admin, User)
- [ ] 實作 API 速率限制 (Rate Limiting)
- [ ] 使用 BCrypt 或 Argon2 進行密碼雜湊
- [ ] 加入分頁功能
- [ ] 實作更詳細的錯誤處理
- [ ] 加入單元測試和整合測試
- [ ] 實作 CORS 政策
- [ ] 加入 API 版本控制
- [ ] 實作快取機制 (Redis)

## 📚 相關文件

- [ASP.NET Core 文檔](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core 文檔](https://docs.microsoft.com/ef/core)
- [PostgreSQL 文檔](https://www.postgresql.org/docs/)
- [JWT 介紹](https://jwt.io/introduction)

## 📄 授權

此專案採用 MIT 授權條款。

## 👥 貢獻者

- YoungChen-Git

## 📞 聯絡方式

如有問題或建議，請開啟 Issue 或 Pull Request。
