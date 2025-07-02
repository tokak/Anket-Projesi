# 📝 Anket Projesi – Anket Oluşturma ve Yönetim Sistemi

**Anket Projesi**, yöneticilerin (Admin) anket oluşturmasını, bu anketlere soru ve cevap seçenekleri eklemesini, kullanıcıların anketleri doldurmasını ve sonuçların analiz edilmesini sağlayan kapsamlı bir web uygulamasıdır.

---

## 🚀 Özellikler

### 👤 Kullanıcı Kimlik Doğrulama & Yetkilendirme
- ✅ Kullanıcı Kayıt (RegisterDto)
- 🔐 Giriş Yapma (LoginDto)
- 🔁 Şifremi Unuttum & Şifre Sıfırlama (ForgotPasswordDto, ResetPasswordDto)
- 🔄 Şifre Değiştirme (ChangePasswordDto)
- 📧 E-posta Onayı (AccountController)
- 🛡️ Rol Bazlı Yetkilendirme (Admin rolü – `RoleConsts.Admin`)

---

### 📊 Anket Yönetimi (Yönetici Paneli)
- ➕ Anket Oluşturma (`CreateSurveyDto`)
- 📋 Anket Listeleme (`ListSurveyDto`)
- ✏️ Anket Güncelleme (`UpdateSurveyDto`)
- ❓ Anket Soruları & Cevap Seçenekleri Yönetimi  
  (`CreateSurveyQuestionDto`, `AnswerOptionInputDto`, `QuestionInputDto`, `ListSurveyQuestionsDto`)

---

### 📝 Anket Cevaplama (Üye Paneli)
- 🟢 Aktif Anketleri Görüntüleme (`ListTakeSurveyDto`)
- ✅ Anket Tamamlama (`SubmitAnswerRequestDto`)
- 🙏 Teşekkür Sayfası (`ThankYouDto`)

---

### 📈 Raporlama & Analiz
- 📊 Genel Anket İstatistikleri (`DashboardSummaryDto`)
- 📉 Anket Bazlı Grafikler (`SurveyStatisticsDto`)
- 📑 Kullanıcı Cevaplarına Göre Detaylı Analiz

---

## 🧱 Proje Mimarisi
Proje, **katmanlı mimari (Layered Architecture)** yapısına uygun olarak yapılandırılmıştır:

- `Entities`
- `DTOs`
- `DataAccess`
- `Business`
- `WebUI` (MVC)

---

## ⚙️ Kurulum ve Başlangıç

### 1-) appsettings.Development.json Dosyasını Düzenleyin

Aşağıdaki alanları kendi bilgilerinize göre doldurun:

![Adsız](https://github.com/user-attachments/assets/c810bcc1-ed65-48e2-a30c-2de84b0d9346)
![Adsız2](https://github.com/user-attachments/assets/0156ce44-3413-4042-9efa-137464e71962)
**Yardımcı Videolar**
https://www.youtube.com/watch?v=621WoYkkwQU  Mail iki adımlı doğrulama işlemleri 

https://www.youtube.com/watch?v=D8DMj2lQMwo  Google ClientId, ClientSecret alma 


### 2-) Packege Manager Console'dan Migration işlemini gerçekleştirme veritabanı bağlantısı kurulduktan sonra veritaban oluşturulur

![image](https://github.com/user-attachments/assets/68e098e4-cf3b-4987-a394-a035c1dfdd99)

### 3-) Projeyi çalıştırın Kayıt oluştur sayfasından kayıt işleminizi yapın sonra veritabanınızda  Admin rolü oluşturun kullanıcıya **Admin** rolü atayın

![Adsız4](https://github.com/user-attachments/assets/225024a5-094f-45c2-8c5b-255cac3ff74b)

### 4-)   Test Süreci

Admin rolü oluşturulduktan sonra, Admin hesabıyla sisteme giriş yapılır.

Giriş yaptıktan sonra anket oluşturulur, anket için sorular ve her soruya ait cevap seçenekleri tanımlanır.

Anket tamamlandığında, **Link Oluştur** butonuna tıklanarak paylaşılabilir bir bağlantı elde edilir.

Bu bağlantı farklı kullanıcılarla paylaşılır.

Kullanıcı, bağlantıya tıkladıktan sonra kayıt olup giriş yapar ve anket sorularını cevaplar.

Kullanıcının verdiği cevaplar, yönetici (Admin) panelinde listelenir ve analiz edilebilir.
**Not**: Test işlemi sırasında bağlantıyı farklı bir tarayıcıda veya gizli sekmede açarak kullanıcı kaydı oluşturup deneyiniz.

---

# Ekran Görüntüleri
  
![1](https://github.com/user-attachments/assets/bcfc95ff-3442-487e-a615-9fe5677c3068)
![2](https://github.com/user-attachments/assets/e8369e5e-f382-4673-8c97-c70ee81e82cb)
![3](https://github.com/user-attachments/assets/8efbe4ac-bc60-4924-a8d2-b3af4e052cb9)
![4](https://github.com/user-attachments/assets/a2cd10fd-5078-45aa-ba72-bad071066cb0)
![5](https://github.com/user-attachments/assets/cc02e298-cc13-48fd-8068-225d871a6423)
![6](https://github.com/user-attachments/assets/35779194-6296-4c95-96ec-00039fe0efd4)
![7](https://github.com/user-attachments/assets/fa98e9f9-47d7-4372-a818-8e25ccf9e019)
![8](https://github.com/user-attachments/assets/f27cf0c1-84d3-4abd-b8b8-c8167d7efc85)
![9](https://github.com/user-attachments/assets/f52a5b86-0104-4702-a32a-80355081afaf)
![10](https://github.com/user-attachments/assets/b526ceb5-2447-4550-ad5a-17fd6975e2dc)
![11](https://github.com/user-attachments/assets/4e00a0a8-571b-4720-9ed4-56e91c0a5dd5)
![12](https://github.com/user-attachments/assets/c35d5b98-7f39-469e-a893-1ae7b35c787f)
![13](https://github.com/user-attachments/assets/589779e1-f5a0-4399-8e5c-377430229a35)
![14](https://github.com/user-attachments/assets/bc47c88d-1f30-4c71-b915-12a2323b87ce)
![15](https://github.com/user-attachments/assets/1870126f-8b8a-4723-b650-d22c36a1525d)
![16](https://github.com/user-attachments/assets/a2bb01e0-1cb2-4d1a-89bb-280b07b8adaa)
![17](https://github.com/user-attachments/assets/b199552f-22bb-4d23-b892-914a309b41a8)
![18](https://github.com/user-attachments/assets/4b1188cb-55a7-4148-929f-bbf304fece10)
![19](https://github.com/user-attachments/assets/875e1417-92ae-4f81-8e27-d459d80cd695)
![20](https://github.com/user-attachments/assets/5f76c5a4-4e63-4d6c-ac8b-5d7eb21a6863)
![21](https://github.com/user-attachments/assets/e49c7c20-f496-45df-a7ed-9bc6bd366883)
![22](https://github.com/user-attachments/assets/5801372f-73ca-4ce7-b374-ef0ff4e171f5)
![23](https://github.com/user-attachments/assets/5b55b168-fb75-4955-9889-fb488e953ded)
![24](https://github.com/user-attachments/assets/7ab3887f-417b-4347-b412-1a6bff5c03c7)
![25](https://github.com/user-attachments/assets/0e82f70e-e85f-4967-84e1-30bd5d01ea96)
![26](https://github.com/user-attachments/assets/0f8ca378-aaa9-4fb0-b899-c77c7fa34a13)
![27](https://github.com/user-attachments/assets/609fbb3b-8d54-4b9c-80d9-eaa8d8edb845)
![28](https://github.com/user-attachments/assets/088bef8a-a4a5-4671-86ea-4cc15d91bcfd)
![29](https://github.com/user-attachments/assets/274123ea-b990-48e3-af15-566aded182af)
![30](https://github.com/user-attachments/assets/0ba49ae0-91cf-4b38-9746-6bf17150c521)



















