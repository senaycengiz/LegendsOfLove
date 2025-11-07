# Legends of Love

## Proje Bilgileri
**Ders:** Yazılım Geliştirme Laboratuvarı – I  
**Dönem:** 2025–2026 Güz  
**Bölüm:** Kocaeli Üniversitesi – Teknoloji Fakültesi, Bilişim Sistemleri Mühendisliği  
**Grup Üyeleri:**  
- Zehra Korkmaz 231307066 
- Şenay Cengiz  231307027
- Yasemin Atış  231307023

---

## 1. Proje Tanımı
Bu proje kapsamında **Unity oyun motoru** kullanılarak yapay zekâ destekli bir **Third Person Shooter (TPS)** oyunu geliştirilmiştir.  
Amaç, temel TPS mekaniklerini (hareket, nişan, ateş etme, taktiksel pozisyon alma, düşman yapay zekâ davranışı) içeren, düşük poligonlu ve optimize bir sahne üzerinde çalışan bir oyun ortaya koymaktır.

**Oyun Adı:** *Legends of Love*  
**Tema:**  Zombilerle savaşarak prensi kurtarma 
**Platform:** PC  
**Oyun Motoru:** Unity 2022.3 LTS (C# diliyle)

---

## 2. Senaryo ve Oyun Dünyası

### Ana Karakter
Oyuncu, prensi zombilerin elinden kurtarmaya çalışan cesur bir kadın savaşçıdır.  
Zindan içerisinde çeşitli bölgelerde dolaşan zombiler, oyuncuya saldırır.  
Kadın savaşçı tüm zombileri öldürerek **puan** toplar.  
Toplam 10 puana ulaşıldığında zindanın kapısı açılır ve prense ulaşılır.

### Oyun Alanı
- **Zindan:** Oyuncu ve zombiler tek seviyelik bir haritada yer alır.Siper alınabilecek taş bloklar ve sandıklara sahiptir.
- **Son Kapı:** 10 puan toplandığında açılır → prense giden oda

### 🏁 Oyun Sonu
Oyuncu, tüm zombileri öldürüp yeterli puanı topladığında **büyük kapı açılır** ve **kapı açılma sesi efekti** çalar.  
Ardından ekranda **“YOU WIN!”** yazısı belirir.  
Bu ekran, prensi başarıyla kurtardığını ve görevi tamamladığını ifade eder.   
Ekrandaki *“Your Legend Begins”* ifadesi, oyuncunun hikâyesinin burada efsaneye dönüştüğünü vurgular.

---

## 3. Oyun Mekanikleri

| Mekanik | Açıklama |
|----------|-----------|
| **Hareket** | W-A-S-D ile yürüme, Shift ile koşma, Space ile zıplama |
| **Kamera** | TPS kamera karakterin arkasında konumlanır |
| **Nişan & Ateş** | Sol tıkla ateş etme |
| **Puan Toplama** | Düşman vurulunca işaret bırakır ve toplanır , sayaç artar |
| **Kapı Açma** | 10 puan toplanınca `DoorController` aktif olur |
| **UI Elemanları** | Kalp sayacı, sağlık barı, pause menüsü |
| **AI FSM** | Idle →  Chase → Attack döngüsü |
| **Pathfinding** | NavMesh Agent ile oyuncuya en kısa yoldan ulaşım |


### Pause Menüsü

Oyuncu oyun sırasında **ESC tuşuna bastığında**, oyun **duraklatma (pause)** moduna geçer.
Bu menüde üç temel seçenek yer alır:

- **Continue:** Oyuna kaldığı yerden devam eder.  
- **Restart:** Oyun yeniden başlatılır.
- **Main Menu:** Oyun Başlangiç ekranına döner. 

Bu ekran, oyuncunun oyunu dilediği anda durdurup devam edebilmesini sağlar.  
Ayrıca bu sahnede **Time.timeScale = 0** kullanılarak oyun durdurulur; “Play” seçeneği seçildiğinde zaman tekrar **1** değerine getirilir.  
Bu sistem sayesinde hem arka plan müziği hem de zombi hareketleri geçici olarak durur.  

Pause menüsü, kullanıcıya daha profesyonel ve kontrol edilebilir bir deneyim sunar.

---

##  4. Geliştirdiğimiz Sistem Şeması ve Oyun Mekanik Blok Diyagramı

### Sistem Şeması
Aşağıdaki diyagram, oyunumuzun temel bileşenleri arasındaki ilişkiyi göstermektedir:
[Player Controller]
│ ↑
│ │ Girdi (Input)
↓ │
[Game Manager] ─────→ [UI System]
│
↓
[Enemy AI System] ──→ [Player Controller] (Saldırı / Hasar)
│
↓
[Door System] ←────── [Game Manager] (Puan = 10 olduğunda kapı açılır)
│
↓
[Audio System] (Müzik, saldırı ve kapı sesi efektleri)

Bu şema, oyundaki ana bileşenlerin veri ve kontrol akışını göstermektedir.  
Oyuncudan gelen girdiler **Player Controller** aracılığıyla işlenir.  
Oyun ilerleyişi ve puanlama **Game Manager** tarafından takip edilir.  
**Enemy AI System**, FSM yapısını kullanarak zombilerin davranışını kontrol eder ve oyuncu ile etkileşime girer.  
**UI System**, oyun içi sayaçları (puan, sağlık) ve kazanç ekranını günceller.  
**Door System**, oyuncu belirli bir puana ulaştığında devreye girer ve **Audio System** ile birlikte kapı açılma sesi efekti çalınır.  
Tüm bu sistemler birlikte çalışarak oyuncuya dinamik ve etkileşimli bir TPS deneyimi sunar.

### Oyun Mekanik Blok Diyagramı

Aşağıdaki diyagram, oyun içindeki temel olay akışını ve mekaniklerin birbirine nasıl bağlı olduğunu göstermektedir:
[OYUN BAŞLANGICI]
↓
[Oyuncu hareket eder ve çevredeki zombileri fark eder]
↓
[Zombiler oyuncuyu algılar → FSM'de Idle'dan Chase'e geçer]
↓
[Oyuncu ateş eder → Zombi öldürülür]
↓
[Puan Sayacı +1]
↓
[Game Manager puanı kontrol eder → Eğer 10 puan olursa]
↓
[Door System aktifleşir → Kapı açılma animasyonu ve ses efekti]
↓
[Oyuncu kapıdan geçer]
↓
["YOU WIN!" ekranı görünür → Oyun tamamlanır]

Bu blok diyagram, oyunun genel oynanış akışını temsil eder.  
Başlangıçta oyuncu hareket ederken zombiler pasif durumdadır (**Idle**).  
Oyuncu görüş alanına girdiğinde, zombiler **Chase** durumuna geçer ve saldırmaya çalışır (**Attack**).  
Oyuncu her zombi öldürdüğünde puan kazanır; **Game Manager** bu puanı takip eder.  
Toplam puan 10’a ulaştığında **Door System** aktifleşir ve büyük kapı açılır.  
Kapıdan geçildiğinde **“You Win!”** ekranı görünür ve oyun başarıyla tamamlanır.  
Bu döngü, oyunun temel mekanik akışını oluşturur.


---

## 5. Kullanılan Mimari, Yöntem ve Teknikler

| Kategori | Kullanılan Teknoloji / Yöntem |
|-----------|-------------------------------|
| **Yazılım Mimarisi** | Bileşen tabanlı Unity OOP yapısı |
| **Programlama Dili** | C# |
| **Yapay Zekâ** | Finite State Machine (FSM) |
| **Yol Bulma (AI Pathfinding)** | Unity NavMesh Agent |
| **Fizik** | Collider, Rigidbody, Raycast tabanlı çarpışma algılama |
| **Görseller** | Low-Poly 3D modeller (Unity Asset Store) |
| **Ses** | Mixkit & FreeSound kütüphaneleri (müzik ve efektler) |
| **Versiyon Kontrol** | Git + GitHub (branch tabanlı süreç yönetimi) |
| **Platform** | PC – Unity 2022.3 LTS ortamında geliştirilmiştir |

Bu yöntemler sayesinde proje, performans açısından optimize edilmiş ve temiz kod yapısıyla modüler biçimde tasarlanmıştır.

---

## 6. Karşılaşılan Zorluklar ve Çözümler

| Zorluk | Çözüm |
|---------|--------|
| **NavMesh agent çakışmaları** | Zemin katmanları yeniden tanımlandı. |
| **Kamera donması (Pause sonrası)** | `Time.timeScale = 1` satırı sahne geçişlerinde manuel olarak eklendi. |
| **Kapı açılmama sorunu** | `DoorController` scripti yeniden yapılandırıldı, puan kontrolü `GameManager` üzerinden yapılacak şekilde düzenlendi. |
| **FPS düşüşleri** | Gereksiz ışık kaynakları ve materyaller temizlenerek sahne optimize edildi. |
| **Git çatışmaları** | Branch yapısı düzenlendi, `main` korumalı hale getirildi. |

Bu zorluklar, oyun geliştirmenin doğal sürecinin bir parçası olup, ekip içinde iş birliğiyle çözülmüştür.

---

## 7. Proje Süreci ve Görev Dağılımı

| Üye | Görevler |
|------|----------|
| **Şenay Cengiz (231307027)** | Ana karakter ve silah kurulumu , menüler ve  sahne geçişleri |
| **Zehra Korkmaz (231307066)** | Sahne kurulumu, TPS hareket, Orbit Kamera , URP/Input Kamera, kapı sistemi ve ses efektleri,Readme |
| **Yasemin Atış (231307023)** | Puan sayacı,Zombi ve prens kurulumu , Yapay zeka(FSM), Zombilerin oluşma alanları |
| **Tüm Ekip** | Test, hata ayıklama, versiyon kontrolü (GitHub),Proje Raporu |

Ekip çalışması, projede görevlerin eşit ve verimli dağılmasını sağlamıştır.

---
## 8. Literatür Taraması

Literatür taraması kapsamında benzer türde geliştirilen TPS (Third Person Shooter) oyunları incelenmiş ve bu oyunlarda kullanılan mekanikler, yapay zekâ yöntemleri ve oyun dinamikleri karşılaştırılmıştır.  
Amaç, mevcut örneklerden ilham alarak kendi projemizi özgün bir biçimde geliştirmek olmuştur.

| Çalışma | Açıklama | Bizim Proje ile Karşılaştırma |
|----------|-----------|------------------------------|
| **Resident Evil 4 (Capcom, 2005)** | TPS kamera yapısı, zombi düşmanlar ve görev odaklı ilerleyiş içerir. | Bizim oyunumuzda benzer kamera kullanımı vardır; ancak oynanış tek seviyelidir ve laboratuvar projesine uygun şekilde sadeleştirilmiştir. |
| **Left 4 Dead (Valve, 2008)** | Takım temelli çok oyunculu zombi hayatta kalma oyunu. | Bizim oyunumuz tek oyunculu, bireysel bir kurtarma hikâyesine sahiptir. |

Yapılan bu karşılaştırmalar sonucunda projemiz:
- FPS ve TPS türlerinden esinlenmiştir,  
- Akademik düzeyde anlaşılabilirliği artırmak amacıyla sadeleştirilmiş mekanikler kullanılmıştır,  

Bu analiz, **“Legends of Love”** projesinin hem eğitsel hem teknik olarak mevcut TPS oyunlarıyla benzer temel mekaniklere sahip olduğunu,  
ancak **tema, sadelik ve öğrenme odaklılık açısından özgün bir yapıya** sahip olduğunu ortaya koymaktadır.


## 9. Projenin Katkıları ve Öğrenim Çıktıları

- Unity oyun motorunda FSM tabanlı yapay zekâ (AI) sistemi geliştirildi.    
- C# dilinde nesne tabanlı programlama pratiği kazanıldı.  
- GitHub üzerinden iş birliği, sürüm kontrolü ve proje yönetimi becerileri geliştirildi.  
- Oyun mekaniği tasarımı, optimizasyon ve kullanıcı deneyimi konularında tecrübe edinildi.  
- Ekip içi iletişim ve iş paylaşımı becerileri güçlendirildi.

---

## 10. Sonuç

> **Legends of Love**, temel TPS oyun mekaniklerini başarıyla uygulayan, yapay zekâ tabanlı bir zombi temalı aksiyon oyunudur.  
> Oyuncu, zekice tasarlanmış yapay zekâya sahip zombileri öldürerek puan toplar, kapıyı açar ve prensi kurtarır.  
> Oyun, yapay zekâ sistemleri, kullanıcı arayüzü, ses tasarımı ve sahne geçişleri açısından başarılı bir şekilde tamamlanmıştır.  
> Bu proje, ekip üyelerine hem teknik hem de ekip çalışması açısından önemli bir deneyim kazandırmıştır.

---

## 11. Kaynakça

- **Unity Documentation:** [https://docs.unity3d.com](https://docs.unity3d.com)  

- **Audio Kaynakları:** Mixkit & FreeSound  
- **Kocaeli Üniversitesi Yazılım Geliştirme Lab. I Proje Dokümanı (2025–2026)**  
- **Görsel Tasarım ve Konsept Oluşturma:** Gemini (Google AI)
- **3D Animasyonlar:** Mixamo (https://www.mixamo.com)
- **Teknik danışmanlık ve yönlendirme desteği:** ChatGPT (OpenAI, 2025)
- **Karakter ve sahne varlıkları :** Unity Asset Store [https://assetstore.unity.com](https://assetstore.unity.com)  




