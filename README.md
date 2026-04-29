# .NET Payment Integration Demo (VNPay, SePay & PayOS)

Dự án demo tích hợp bộ ba giải pháp thanh toán hàng đầu Việt Nam: **VNPay**, **SePay** và **PayOS** vào ứng dụng .NET 10. Giải pháp tập trung vào tính bảo mật, kiến trúc mở rộng và trải nghiệm người dùng hiện đại.

---

## 🚀 Thông Tin Dự Án

- **Phiên bản:** 1.0.0
- **Ngày cập nhật:** 29/04/2026
- **Công nghệ cốt lõi:**
  - **Backend:** .NET 10 Web API
  - **Frontend:** Vanilla JS, CSS.
  - **SDKs:**
    - `VNPAY.NET` (v2.1.0) - Phan Xuan Quang.
    - `payOS` (v2.1.0) - Official SDK.
  - **Docs:** `Scalar.AspNetCore` (API Reference hiện đại).

---

## 🛠 Hướng Dẫn Cài Đặt Chi Tiết Từ A-Z

### 1. Tích hợp VNPay (Qua SDK VNPAY.NET)

VNPay là cổng thanh toán quốc gia hỗ trợ QR-Code và thẻ nội địa/quốc tế.

- **Bước 1: Lấy thông tin Sandbox**
  - Truy cập [VNPAY Test Site](https://sandbox.vnpayment.vn/apis/vnpay-test/).
  - Ghi lại các thông tin test: `vnp_TmnCode`, `vnp_HashSecret`.
- **Bước 2: Cấu hình `appsettings.json`**
  ```json
  "Vnpay": {
    "TmnCode": "MÃ_TMN_CỦA_BẠN",
    "HashSecret": "CHUỖI_SECRET_CỦA_BẠN",
    "BaseUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "CallbackUrl": "http://localhost:5200/api/payment/vnpay-callback",
    "Version": "2.1.0"
  }
  ```
- **Bước 3: Tài liệu tham khảo**
  - [SDK VNPAY.NET GitHub](https://github.com/phanxuanquang/VNPAY.NET)
  - [Tài liệu API chính thức](https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html)

### 2. Tích hợp SePay (Webhook & QR Automation)

SePay giúp tự động hóa xác nhận chuyển khoản ngân hàng thông qua Webhook.

- **Bước 1: Thiết lập SePay Dashboard**
  - Đăng ký tài khoản tại [SePay.vn](https://sepay.vn/).
  - Kết nối tài khoản ngân hàng của bạn.
  - Vào mục **Tích hợp Webhook** -> Tạo Webhook mới trỏ về: `http://DOMAIN_CỦA_BẠN/api/payment/sepay-webhook`.
- **Bước 2: Cấu hình `appsettings.json`**
  ```json
  "Sepay": {
    "MerchantId": "ID_MÁY_CHỦ_SEPAY",
    "SecretKey": "API_KEY_HOẶC_SECRET_TOKEN",
    "BaseUrl": "https://pay.sepay.vn/checkout",
    "SuccessUrl": "http://localhost:5200/success.html?gateway=sepay"
  }
  ```
- **Bước 3: Tài liệu tham khảo**
  - [SePay Developer Guide](https://developer.sepay.vn/vi/cong-thanh-toan/bat-dau)

### 3. Tích hợp PayOS (Official SDK)

PayOS cung cấp trải nghiệm thanh toán link mượt mà và đối soát tự động.

- **Bước 1: Lấy API Keys**
  - Truy cập [PayOS Dashboard](https://dashboard.payos.vn/).
  - Tạo một ứng dụng mới và lấy: `Client ID`, `API Key`, `Checksum Key`.
- **Bước 2: Cấu hình `appsettings.json`**
  ```json
  "Payos": {
    "ClientId": "ID_CỦA_BẠN",
    "ApiKey": "KEY_CỦA_BẠN",
    "ChecksumKey": "CHECKSUM_CỦA_BẠN",
    "ReturnUrl": "http://localhost:5200/success.html?gateway=payos",
    "CancelUrl": "http://localhost:5200/error.html?gateway=payos"
  }
  ```
- **Bước 3: Tài liệu tham khảo**
  - [PayOS .NET SDK Docs](https://payos.vn/docs/sdks/back-end/net)

---

## 🏃 Cách Chạy Dự Án

1.  **Clone & Restore:**
    ```bash
    dotnet restore
    ```
2.  **Khởi chạy:**
    ```bash
    dotnet run
    ```
3.  **Truy cập:**
    - UI chính: [http://localhost:5200](http://localhost:5200)
    - Tài liệu API (Scalar): [http://localhost:5200/scalar/v1](http://localhost:5200/scalar/v1)

---

## 🛡 Kiến Trúc & Bảo Mật (Best Practices)

- **`IPaymentService`:** Interface thống nhất cho mọi cổng thanh toán, dễ dàng thay thế hoặc thêm mới provider.
- **`ApiResponse<T>`:** Chuẩn hóa dữ liệu trả về cho Frontend, bao gồm cả TraceId để debug.
- **Global Exception Handling:** Middleware tự động bắt lỗi và trả về format JSON chuẩn, không lộ stack trace nhạy cảm.
- **Security Policy:**
  - Không commit secret lên GitHub.
  - Kiểm tra chữ ký (Signature) cho mọi callback/webhook để tránh giả mạo giao dịch.
