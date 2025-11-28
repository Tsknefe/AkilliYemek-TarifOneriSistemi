import "./HomePage.css";

// bu sayfa bizim landing yani giriş sayfası
// kullanıcı ilk geldiğinde sistemi high level tanıttığımız yer burası
// burada sadece frontend tarafı var herhangi bir api isteği yok
// ana fikir sistemi 3 ana özelliği ile özetlemek

export default function HomePage() {
  return (
    <div className="home-wrapper">
      <div className="home-hero">

        {/* üstte küçük rozet gibi bir alan beta olduğunu gösteriyor */}
        <span className="home-badge">Beta • Akıllı Beslenme Asistanı</span>

        {/* ürünün ana başlığı */}
        <h1>Akıllı Yemek Öneri Sistemi</h1>

        {/* kısa açıklama kısmı sistem ne yapıyor tek paragrafta anlatıyoruz */}
        <p className="home-lead">
          Elindeki malzemelere ve beslenme hedeflerine göre akıllı tarifler,
          hafta içi için otomatik planlar ve kişisel diyet önerileri üret.
        </p>

        {/* ana özellikler listesi */}
        <ul className="home-features">
          <li> Tarif listesi ve gelişmiş arama</li>
          <li> Öneri motoru (malzeme + kalori + diyet tipi)</li>
          <li> Haftalık beslenme planı</li>
        </ul>
      </div>
    </div>
  );
}
