#  Akıllı Şehir Ulaşım Optimizasyonu


##  Proje Amacı

Bu proje, şehirdeki toplu taşıma sistemini dijital olarak modelleyerek kullanıcıya **trafik yoğunluğuna göre optimize edilmiş en kısa rotayı** sunmayı hedefler.  
Rota planlama sürecinde hem **duraklar arası mesafe** hem de **trafik yoğunluğu** birlikte değerlendirilir.

> ASP.NET Core MVC mimarisi ile geliştirilmiştir.

---

##  Kullanılan Teknolojiler

- ASP.NET Core MVC – Web uygulama çatısı  
- C# (.NET 8) – Backend programlama dili  
- Razor View Engine – Arayüz katmanı  
- Bootstrap – Responsive UI  
- Dictionary, LinkedList, PriorityQueue – Algoritmalar için veri yapıları

---

##  Proje Yapısı ve Sınıflar

### 1.  `CityGraph.cs` – Şehir Haritası (Graf)

- Şehir durakları ve yolları **graf** biçiminde modellenmiştir.  
- Düğümler (Nodes): Durak adları ve koordinatları  
- Kenarlar (Edges): İki durak arasındaki mesafe ve bağlantı  
- `FindKShortestPaths()` metodu ile **en kısa K yol** bulunur  
- **Veri yapısı:** `Dictionary`, `PriorityQueue`

---

### 2.  `RouteHashTable.cs` – Trafik Yoğunluğu

- İki durak arasındaki **trafik verilerini** tutar  
- `GetTrafficData()` ile bir rotaya ait trafik yoğunluklarını döndürür  
- **Veri yapısı:** `Dictionary<string, double>`

---

### 3.  `RouteLinkedList.cs` – Trafik Temelli Rota Optimizasyonu

- Trafik yoğunluğu **0.7 veya üzeri** olan bağlantılar rotadan çıkarılır  
- Bağlı liste üzerinde dinamik düzenleme yapılır  
- **Veri yapısı:** `LinkedList<string>`

---

### 4.  `HomeController.cs` – Ana Kontrolcü

- Kullanıcıdan alınan başlangıç ve bitiş durakları işlenir  
- Rota hesaplanır, trafik verisi alınır, optimize rota oluşturulur  
- Sonuç **View** katmanına gönderilir  

---

##  Uygulama Akışı

1. Kullanıcı **başlangıç** ve **bitiş** duraklarını girer.  
2. `CityGraph` üzerinden en kısa yollar hesaplanır.  
3. Her yol için trafik yoğunluğu `RouteHashTable` ile kontrol edilir.  
4. Yoğun trafik içeren yollar `RouteLinkedList` ile rotadan çıkarılır.  
5. **Optimize edilmiş rota** kullanıcıya gösterilir.

---
### 1. CityGraph.cs – Şehir Haritası ve Kısa Yol Algoritması

#### Ne İşe Yarar
- Şehirdeki durakları (noktaları) ve yolları (kenarları) temsil eden bir graf yapısıdır.
- Başlangıç ve bitiş noktaları arasında k-en kısa yolları bulur.

#### Kullanılan Veri Yapıları
- `Dictionary<string, (double lat, double lng)> Nodes`: Durağın koordinatlarını saklar.
- `Dictionary<string, List<(string to, double distance)>> Edges`: Graf kenarlarını tutar.
- `PriorityQueue`: En kısa yol hesaplamalarında kullanılır (Dijkstra benzeri).

#### Zaman Karmaşıklığı
- `AddStop`, `AddRoute`: O(1)
- `FindKShortestPaths`: O(k × (V + E) log V)
  - k: Bulunacak yol sayısı
  - V: Düğüm sayısı
  - E: Kenar sayısı

#### Alan Karmaşıklığı
- O(V + E): Tüm graf bellekte tutulur.

#### Neden Bu Yapı Seçildi
- `Dictionary` ile hızlı erişim sağlanır (O(1)).
- `PriorityQueue` sayesinde yollar önceliğe göre sıralanır.

#### Gerçek Hayat Kullanımı
- Akıllı şehirlerde güzergah önerme sistemleri
- Navigasyon uygulamaları
- Toplu taşıma planlaması

---

### 2. RouteHashTable.cs – Trafik Yoğunluğu Verisi

#### Ne İşe Yarar
- İki durak arasındaki trafik yoğunluğunu saklar.
- Bir rota boyunca trafik verilerini döndürür.

#### Kullanılan Veri Yapısı
- `Dictionary<string, double>`: Durağa özel trafik yoğunluklarını tutar.

#### Zaman Karmaşıklığı
- `AddTrafficData`: O(1)
- `GetTrafficData`: O(n) (n: rota uzunluğu)

#### Alan Karmaşıklığı
- O(E): Tüm kenar çiftleri için değer tutulur.

#### Neden Bu Yapı Seçildi
- `Dictionary` ile duraklar arası hızlı veri erişimi sağlanır.
- `GenerateKey` yöntemiyle yönsüz grafik uyumu sağlanır.

#### Gerçek Hayat Kullanımı
- Trafik yoğunluğunu analiz etme
- Alternatif yollar önerme
- Zaman bazlı rota optimizasyonu

---

### 3. RouteLinkedList.cs – Trafik Temelli Rota Optimizasyonu

#### Ne İşe Yarar
- Rota üzerindeki trafik yoğunluğu 0.7 ve üzeri olan noktaları listeden çıkararak rota optimize edilir.

#### Kullanılan Veri Yapısı
- `LinkedList<string>`: Esnek yapı sayesinde duraklar üzerinde rahat işlem yapılır.

#### Zaman Karmaşıklığı
- O(n): Liste üzerinden bir kez geçilir.

#### Alan Karmaşıklığı
- O(n): Bağlı liste bellekte tutulur.

#### Neden Bağlı Liste Seçildi
- Düğüm silme işlemleri O(1) sürede gerçekleşir.
- `ArrayList` veya `List` kullanılsaydı silme işlemi O(n) olurdu.

#### Gerçek Hayat Kullanımı
- Yoğun trafiğe göre rota dinamik olarak kısaltılabilir.
- Acil ulaşım senaryolarında daha hızlı güzergah sunar.




## 📊 Genel Değerlendirme

| Sınıf               | Görev                        | Veri Yapısı             | Karmaşıklık (Zaman / Alan)   | Tercih Nedeni                        |
|--------------------|------------------------------|--------------------------|------------------------------|--------------------------------------|
| `CityGraph`        | Rota hesaplama (K-en kısa yol) | `Dictionary`, `PQ`       | O(k × (V+E) log V) / O(V+E)  | Yönlendirilmiş grafik optimizasyonu  |
| `RouteHashTable`   | Trafik verisi yönetimi        | `Dictionary`             | O(1), O(n) / O(E)            | Hızlı ve anahtarlı erişim            |
| `RouteLinkedList`  | Trafik temelli rota düzeltme  | `LinkedList`             | O(n) / O(n)                  | Hızlı silme (trafik > 0.7)           |

---
