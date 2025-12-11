import { useEffect, useState } from "react";
import api from "../api/client";
import { useNavigate } from "react-router-dom";
import "./RecipesPage.css";

// bu sayfa bizim tarifleri listelediğimiz yer
// burda backendden gelen tüm tarifleri alıyoruz
// kullanıcı tarif arayabiliyor diyet tipine göre filtreleyebiliyor
// kartlara tıklayınca detay sayfasına gidiyor
// ekipteki herkes rahatça okuyabilsin diye her adımı basit şekilde açıklıyorum

export default function RecipesPage() {

  // backendden gelen tarifleri tutuyoruz
  const [recipes, setRecipes] = useState([]);

  // ekranda gösterilen filtrelenmiş liste
  const [filtered, setFiltered] = useState([]);

  // kullanıcının arama inputu
  const [search, setSearch] = useState("");

  // diyet filtresi
  const [dietFilter, setDietFilter] = useState("");

  // loading ve hata durumları
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");

  const navigate = useNavigate();


  // sayfa açılır açılmaz tarifleri çekiyoruz
  useEffect(() => {
    const loadRecipes = async () => {
      setIsLoading(true);
      setError("");

      try {
        const res = await api.get("/RecipeApi");
        const data = res.data || [];
        setRecipes(data);
        setFiltered(data); // ilk ekranda filtrelenmiş hali direkt tüm tarifler oluyor
      } catch (err) {
        console.error(err);
        setError("Tarifler yüklenirken sıkıntı oldu");
      } finally {
        setIsLoading(false);
      }
    };

    loadRecipes();
  }, []);


  // arama veya diyet filtresi değişince sonuçları tekrar hesaplıyoruz
  useEffect(() => {
    let data = [...recipes];

    // arama varsa filtrele
    if (search.trim() !== "") {
      const s = search.toLowerCase();
      data = data.filter((r) => (r.name || "").toLowerCase().includes(s));
    }

    // diyet tipi seçilmişse filtrele
    if (dietFilter !== "") {
      data = data.filter((r) => (r.dietType || "") === dietFilter);
    }

    setFiltered(data);
  }, [search, dietFilter, recipes]);


  // detay sayfasına yönlendirme fonksiyonu
  const goToDetail = (id) => {
    navigate(`/recipes/${id}`);
  };


  // render kısmı burası
  return (
    <div className="recipes-wrapper">

      {/* üst başlık kısmı */}
      <div className="recipes-header">
        <div>
          <h1 className="page-title">Tarifler</h1>
          <p className="page-subtitle">
            Tarifleri buradan inceleyebilirsin açıklamalar malzemeler adımlar hepsi detayda mevcut
          </p>
        </div>
      </div>

      {/* filtreler */}
      <div className="recipes-filters">
        <input
          type="text"
          placeholder="Tarif ara..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />

        <select
          value={dietFilter}
          onChange={(e) => setDietFilter(e.target.value)}
        >
          <option value="">Tüm diyet tipleri</option>
          <option value="Normal">Normal</option>
          <option value="Vejetaryen">Vejetaryen</option>
          <option value="Vegan">Vegan</option>
          <option value="Ketojenik">Ketojenik</option>
        </select>
      </div>

      {/* hata varsa mesaj */}
      {error && <div className="alert alert-error">{error}</div>}

      {/* yükleniyor animasyonu */}
      {isLoading && (
        <p className="muted-text">Tarifler yükleniyor bekleyelim...</p>
      )}

      {/* filtre boşa düştüyse */}
      {!isLoading && filtered.length === 0 && !error && (
        <p className="muted-text">
          Filtrelere uygun tarif bulunamadı
        </p>
      )}

      {/* tarif kartları */}
      <div className="recipes-grid">
        {filtered.map((r) => {
          const nutrition = r.nutritionFacts || {};

          return (
            <div
              key={r.id}
              className="recipe-card"
              onClick={() => goToDetail(r.id)}
            >
              <div className="recipe-card-body">

                {/* tarif adı */}
                <h3>{r.name}</h3>

                {/* açıklamanın kısa hali */}
                {r.description && (
                  <p className="recipe-description">
                    {r.description.length > 120
                      ? r.description.slice(0, 120) + "..."
                      : r.description}
                  </p>
                )}

                {/* besin değeri süre diyet tipi */}
                <div className="recipe-meta">
                  <span>
                    Kalori {nutrition.calories != null ? `${nutrition.calories} kcal` : "?"}
                  </span>

                  <span>
                    Süre {r.cookingTime != null ? `${r.cookingTime} dk` : "?"}
                  </span>

                  {r.dietType && <span>{r.dietType}</span>}
                </div>
              </div>

              {/* detay butonu */}
              <div className="recipe-card-footer">
                <button
                  type="button"
                  className="small-button"
                  onClick={(e) => {
                    e.stopPropagation();
                    goToDetail(r.id);
                  }}
                >
                  Detayı Gör
                </button>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
