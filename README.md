# 🚌 Akıllı Şehir Ulaşım Optimizasyonu


## 📋 Proje Amacı

Bu projede, şehir içi duraklar arasında kullanıcıya en uygun rotayı sunmak, aynı zamanda trafik yoğunluğunu göz önünde bulundurarak alternatif yollar önermek hedeflenmiştir. Kullanıcı, başlangıç ve bitiş durağını seçer, sistem en az trafikli ve en kısa rotaları listeler.

## 🔧 Kullanılan Veri Yapıları ve İşlevleri

### CityGraph (Graf Veri Yapısı)
- **Amaç:** Durakları ve aralarındaki mesafeleri (kenarları) tutar
- **Yapı:**
  - `Dictionary<string, (double lat, double lng)> Nodes`
  - `Dictionary<string, List<(string to, double distance)>> Edges`
- **Algoritma:** Genişletilmiş Dijkstra tabanlı yol bulma, K alternatif yol üretir

### RouteHashTable (Hash Tabanlı Trafik Tablosu)
- **Amaç:** Her yol parçası için trafik yoğunluk değerlerini tutmak
- **Yarar:** Trafik verisi O(1) sürede erişilir, toplam trafik hesabı hızlı yapılır

### RouteLinkedList (Bağlı Liste ile Yol Optimize Etme)
- **Amaç:** Alternatif yolların optimize edilmesi ve sıralanması
- **Yarar:** Liste yapısı sayesinde dinamik uzunlukta yollar kolayca işlenebilir

## 📊 Algoritma Analizi ve Karmaşıklıklar

### Yol Bulma Algoritması (FindKShortestPaths)
- **Kullanılan algoritma:** Dijkstra'nın genişletilmiş versiyonu (K-en iyi yol)
- **Zaman karmaşıklığı:**
  - En iyi durumda: O(E log V) (Dijkstra için)
  - K farklı yol bulunduğu için: O(K × V × log V) olabilir
- **Alan karmaşıklığı:** O(V + E + K) (kuyruk + yol listesi + sonuçlar)

### Trafik Verisi Hesabı
- Her yol için trafik: O(n) (n = yol üzerindeki durak sayısı)

## 💹 Maliyet Karşılaştırması (Veri Yapıları)

| Veri Yapısı | Alternatif | Zaman Karmaşıklığı | Avantajları |
|-------------|------------|-------------------|------------|
| Dictionary | Array, List | O(1) erişim | Trafik verisine hızlı erişim (RouteHashTable) |
| LinkedList | Array, Stack | O(n) tarama | Dinamik boyut, rotaların esnek yapısı |
| Graph (Adj. List) | Matrix, SetMap | O(V + E) gezi | Bellek verimli, yol bulmada etkili |

> Sabit uzunlukta rotalar için array, esnek/dinamik rotalar için linked list daha uygundur.

## 🔄 Sistem Nasıl Çalışıyor?

1. Kullanıcı başlangıç ve bitiş noktalarını girer
2. Sistem CityGraph üzerinde K kısa yol arar
3. Bulunan her rota için RouteHashTable ile trafik değerleri alınır
4. Trafik değeri %70'in altında olanlar "optimize edilebilir rota" olarak işaretlenir
5. Sistem kullanıcıya tercih sırasına göre 1. ve 2. önerileri sunar

## 🏁 Sonuç

- Başarıyla yapılandırılmış bir yol bulma ve trafik analiz sistemidir
- Dictionary, grafik ve bağlı liste mantıklı kullanılmıştır
- Performanslı ve genişletilebilir bir altyapı sağlar
- Proje, gerçek zamanlı trafik verisi veya harita servisleriyle (örn. Google Maps API) entegre edilerek daha ileri taşınabilir

## 🤔 Neden Bu Yöntemler Seçildi?

Bu projede amacımız, şehir içi ulaşımda hem en kısa hem de trafik açısından en uygun rotayı sunan bir sistem geliştirmekti. Bu amaca ulaşabilmek için özgün yapıların birlikte uyumlu çalışmasına ihtiyaç vardı.

### CityGraph — Grafik Veri Yapısı
#### Neden Grafik?
Ulaşım sistemi, doğası gereği düğümler (duraklar) ve kenarlar (yollar) şeklinde modellenir. Bu durum tam olarak bir ağırlıklı yönsüz grafik modelidir.

#### ✔ Neden Adjacency List (Komşuluk Listesi)?
- Daha az bellek kullanır (O(V + E))
- Düğüm sayısı fazla, ama her düğüm az bağlantılıysa en uygunudur
- Gerçek hayattaki şehir haritalarında, her durak yalnızca belli başlı duraklara bağlıdır

Bu yüzden `Dictionary<string, List<(string, double)>>` yapısı tercih edilmiştir.

### FindKShortestPaths() — K-En İyi Yol Algoritması
#### Neden Sadece En Kısa Yol Değil?
Sadece en kısa yol değil, birden fazla alternatif rota sunulmak istendi. Bu kullanıcı deneyimini iyileştirir ve trafik durumuna göre seçim yapılmasını sağlar.

#### ✔ Neden Dijkstra Tabanlı Gelişmiş Algoritma?
- Dijkstra en kısa yolu en verimli şekilde bulur
- Genişletilerek K farklı yol elde edilebilir
- Öncelik kuyruğu kullanımı sayesinde iyi performans sağlar (O(E log V))

Bu nedenle klasik BFS veya DFS yerine, öncelik kuyruklu yol keşif algoritması kullanılmıştır.

### RouteHashTable — Hash Tabanlı Trafik Tablosu
#### Neden Hash Tabanlı Yapı?
Trafik verileri sürekli güncellenebilir ve sorgulanabilir olmalıdır.

#### ✔ Neden Dictionary?
- Anahtar-değer eşleşmeleri çok hızlıdır (O(1) erişim süresi)
- ("Kadikoy-Taksim") → trafik değeri gibi doğrudan sorgulama yapılabilir
- Alternatif: Veritabanı kullanımı daha yavaş ve karmaşık olurdu

Hash tabanlı yapı bu yüzden hız ve sadelik açısından seçilmiştir.

### RouteLinkedList — Yol Listesi Yapısı
#### Neden Bağlı Liste?
Alternatif yolların tutulduğu veri yapısıdır ve her bir yol farklı uzunlukta olabilir.

#### ✔ Neden Dizi (Array) yerine LinkedList?
- Rotalar dinamik olarak oluşur, uzunlukları önceden bilinmez
- Bağlı listeler, değişken uzunlukta veriler için daha esnektir
- Her bir adım ve trafik değeriyle eşleşmeli yapılar kolayca modellenebilir

Bu sayede sistem her rotayı gerektiği gibi yapılandırır ve kullanıcıya uygun hale getirir.

## 📝 Genel Değerlendirme

Bu proje, gerçek dünya problemlerini doğru veri yapılarıyla temsil etme başarısı göstermektedir. Seçilen yöntemler sayesinde:

- Rotaya göre trafik analizi yapılabilir
- Alternatif öneriler geliştirilebilir
- Performans kaybı olmadan geniş veriyle çalışılabilir

Bu nedenle, kullanılan her yapı ve algoritma, probleme özel olarak en uygun yöntemler olarak belirlenmiş ve başarıyla uygulanmıştır.

---

