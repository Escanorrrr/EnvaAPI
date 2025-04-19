# EnvaTest API Dokümantasyonu

## 1. Kimlik Doğrulama (Authentication)

### Login
- **Endpoint:** `POST /api/Auth/login`
- **Yetkilendirme:** Gerekli değil
- **Request Body:**
```json
{
    "username": "string",
    "password": "string"
}
```
- **Response:**
```json
{
    "statusCode": 200,
    "data": {
        "token": "string",
        "expiration": "datetime"
    },
    "message": "string"
}
```

## 2. Müşteri İşlemleri (Customer)

### Tüm Müşterileri Getir
- **Endpoint:** `GET /api/Customer/GetAllCustomers`
- **Yetkilendirme:** Admin rolü gerekli
- **Response:**
```json
{
    "statusCode": 200,
    "data": [
        {
            "id": "long",
            "username": "string",
            "email": "string",
            "isActive": "boolean"
        }
    ],
    "message": "string"
}
```

### Müşteri Detayları
- **Endpoint:** `GET /api/Customer/GetCustomerById/{id}`
- **Yetkilendirme:** Admin rolü gerekli
- **Response:**
```json
{
    "statusCode": 200,
    "data": {
        "id": "long",
        "username": "string",
        "email": "string",
        "isActive": "boolean"
    },
    "message": "string"
}
```

### Müşteri Oluştur
- **Endpoint:** `POST /api/Customer/CreateCustomer`
- **Yetkilendirme:** Admin rolü gerekli
- **Request Body:**
```json
{
    "username": "string",
    "email": "string",
    "password": "string"
}
```

### Müşteri Güncelle
- **Endpoint:** `PUT /api/Customer/UpdateCustomer/{id}`
- **Yetkilendirme:** Admin rolü gerekli
- **Request Body:**
```json
{
    "username": "string",
    "email": "string"
}
```

### Şifre Değiştir
- **Endpoint:** `PUT /api/Customer/ChangePassword/{id}`
- **Yetkilendirme:** Admin rolü gerekli
- **Request Body:**
```json
{
    "currentPassword": "string",
    "newPassword": "string"
}
```

## 3. Fatura İşlemleri (Invoice)

### Fatura Yükle
- **Endpoint:** `POST /api/Invoice/Upload`
- **Yetkilendirme:** Admin rolü gerekli
- **Content-Type:** `multipart/form-data`
- **Request Body:**
  - `InvoiceFile`: Dosya (PDF, JPG, JPEG)
  - `InvoiceTypeId`: long
  - `Amount`: decimal
- **Response:**
```json
{
    "statusCode": 200,
    "data": {
        "id": "long",
        "invoicePath": "string",
        "amount": "decimal",
        "customerId": "long",
        "customerName": "string",
        "invoiceTypeId": "long",
        "invoiceTypeName": "string",
        "createdAt": "datetime",
        "updatedAt": "datetime"
    },
    "message": "string"
}
```

### Faturaları Listele
- **Endpoint:** `GET /api/Invoice/MyInvoices`
- **Yetkilendirme:** Admin rolü gerekli
- **Response:**
```json
{
    "statusCode": 200,
    "data": [
        {
            "id": "long",
            "invoicePath": "string",
            "amount": "decimal",
            "customerId": "long",
            "customerName": "string",
            "invoiceTypeId": "long",
            "invoiceTypeName": "string",
            "createdAt": "datetime",
            "updatedAt": "datetime"
        }
    ],
    "message": "string"
}
```

### Fatura Detayı
- **Endpoint:** `GET /api/Invoice/{id}`
- **Yetkilendirme:** Admin rolü gerekli
- **Response:**
```json
{
    "statusCode": 200,
    "data": {
        "id": "long",
        "invoicePath": "string",
        "amount": "decimal",
        "customerId": "long",
        "customerName": "string",
        "invoiceTypeId": "long",
        "invoiceTypeName": "string",
        "createdAt": "datetime",
        "updatedAt": "datetime"
    },
    "message": "string"
}
```

## 4. Hesaplama İşlemleri (Calculator)

### Tehlikesiz Atık Hesaplama
- **Endpoint:** `POST /api/Calculator/tehlikesiz-atik`
- **Yetkilendirme:** Gerekli
- **Request Body:**
```json
{
    "faaliyetVerisiKg": "double",
    "toplamUretimKg": "double"
}
```

### Yangın Tüpü Hesaplama
- **Endpoint:** `POST /api/Calculator/yangin-tupu`
- **Yetkilendirme:** Gerekli
- **Request Body:**
```json
{
    "faaliyetVerisiKg": "double",
    "tupSayisi": "int",
    "toplamUretimTon": "double"
}
```

### Doğalgaz Hesaplama
- **Endpoint:** `POST /api/Calculator/dogalgaz`
- **Yetkilendirme:** Gerekli
- **Request Body:**
```json
{
    "faaliyetVerisiM3": "double",
    "toplamUretimTon": "double"
}
```

### Dizel Hesaplama
- **Endpoint:** `POST /api/Calculator/dizel`
- **Yetkilendirme:** Gerekli
- **Request Body:**
```json
{
    "faaliyetVerisiLitre": "double",
    "toplamUretimTon": "double"
}
```

## Genel Notlar:

1. **Yetkilendirme:**
   - JWT token gerektiren endpointler için header'da `Authorization: Bearer {token}` şeklinde token gönderilmelidir
   - Admin rolü gerektiren endpointler için kullanıcının Admin rolüne sahip olması gerekir

2. **Hata Yönetimi:**
   - Tüm endpointler `Result<T>` tipinde response döner
   - Başarılı durumda `statusCode: 200`
   - Hata durumunda `statusCode: 400` veya `404`
   - Hata mesajları `message` alanında döner

3. **Dosya Yükleme:**
   - Fatura yükleme endpointi için dosya boyutu maksimum 10MB olmalıdır
   - İzin verilen dosya formatları: PDF, JPG, JPEG

4. **Validasyon:**
   - Tüm gerekli alanlar için validasyon kuralları mevcuttur
   - Hatalı isteklerde uygun hata mesajları döner 