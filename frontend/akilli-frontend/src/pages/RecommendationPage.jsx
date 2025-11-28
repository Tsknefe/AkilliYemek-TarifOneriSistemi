import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api/client";
import "./RecommendationPage.css";

/*
Bu sayfa bizim akıllı öneri motorumuzun sayfasıdır.
Biz bu sayfadan kullanıcıdan 
1-elindeki malzemeleri
2-max pişirme sürelerini
3-hedef kalorilerini
4-diyet tiplerini
5-totalde kaç adet sonuç istediklerini 

alıp backenddeki  RecommendationApi üzerinden uygun tarif önerilerini listeliyoruz.

işleyiş mantığı olarak ise:
1- Sayfa yüklenince malzemeler API’den çekilir.
2- Kullanıcı malzemeleri seçer.
3- Filtreler girilir.
4- "Önerileri Getir" butonuna basılınca backend'e POST atılır.
5- Gelen tarifler ekrana grid şeklinde basılır.

kodu daha rahat takip edebilmeniz için stateler ile grupladım ...
*/


export default function RecommendationPage() {
  //APIden gelen tüm malzemeler
  const [ingredients, setIngredients] = useState([]);

  //Kullanıcının seçtiği malzeme ID’leri
  const [selectedIngredients, setSelectedIngredients] = useState([]);
  const [maxTime, setMaxTime] = useState("");
  const [targetCalories, setTargetCalories] = useState("");
  const [dietType, setDietType] = useState("");
  const [top, setTop] = useState(5);

  //APIden gelen sonuçlar
  const [results, setResults] = useState([]);

  //UI state yönetimi 
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");
  const [hasSearched, setHasSearched] = useState(false);

  const navigate = useNavigate();


  //sayfayı yüklediğimizde verileri çek klasik...
  useEffect(() => {
    api
      .get("/IngredientApi")
      .then((res) => setIngredients(res.data))
      .catch((err) => {
        console.error(err);
        setError("Malzemeler yüklenirken bir hata oluştu.");
      });
  }, []);

  //malzemeyi seç kaldır (toggle) işlemleri bu da klasik 
  const toggleIngredient = (id) => {
    setSelectedIngredients((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    );
  };

  //önerileri getirme fonksiyonu 
  const getRecommendations = async () => {
    //kullanıcı malzeme seçmeden istek göndermesin
    if (selectedIngredients.length === 0) {
      setError("En az bir malzeme seçmelisiniz.");
      return;
    }

    //gösterilecek sonuçların geçerlililk kontrolü
    const topNumber = Number(top) || 5;
    if (topNumber < 1 || topNumber > 20) {
      setError("Gösterilecek sonuç sayısı 1 ile 20 arasında olmalı.");
      return;
    }

    //form geçerli sorgu başlıyor...
    setError("");
    setIsLoading(true);
    setHasSearched(true);

    try {
      //backende fırlatıcağımız payload
      const response = await api.post("/RecommendationApi/basic", {
        ingredientIds: selectedIngredients,
        maxCookingTime: maxTime ? Number(maxTime) : null,
        targetCalories: targetCalories ? Number(targetCalories) : null,
        dietType: dietType || null,
        top: topNumber,
      });

      const data = response.data || [];
      setResults(data);

      //sonuç yoksa kullanıcıyı bilgilendir
      if (data.length === 0) {
        setError(
          "Bu kombinasyonla uygun tarif bulunamadı. Malzemeleri veya filtreleri biraz gevşetmeyi deneyin."
        );
      }
    } catch (err) {
      console.error(err);
      setError(
        "Öneriler alınırken bir hata oluştu. Lütfen daha sonra tekrar deneyin."
      );
    } finally {
      setIsLoading(false);
    }
  };


  //sayfa render işlemleri
  return (
    <div className="recommend-wrapper">
      <h1 className="page-title"> Akıllı Öneri Motoru</h1>
      <p className="page-subtitle">
        Elindeki malzemeleri ve beslenme hedeflerini gir, sana en uygun
        tarifleri önerelim.
      </p>

      {/* 1. MALZEMELER */}
      <div className="section">
        <h2>1. Elindeki malzemeleri seç</h2>
        <p className="section-help">
          Listeden elinde bulunan malzemelere tıkla. Birden fazla malzeme
          seçebilirsin.
        </p>

        <div className="ingredients-grid">
          {ingredients.map((ing) => (
            <button
              key={ing.id}
              type="button"
              className={`ingredient-item ${
                selectedIngredients.includes(ing.id) ? "selected" : ""
              }`}
              onClick={() => toggleIngredient(ing.id)}
            >
              {ing.name}
            </button>
          ))}

          {ingredients.length === 0 && (
            <p className="muted-text">
              Malzeme listesi yükleniyor veya şu anda boş.
            </p>
          )}
        </div>
      </div>

      {/* 2. BESLENME HEDEFLERİ */}
      <div className="section">
        <h2>2. Beslenme hedeflerini gir</h2>

        <div className="input-group">
          <label>
            Max pişirme süresi (dk)
            <input
              type="number"
              placeholder="Örn: 30"
              value={maxTime}
              onChange={(e) => setMaxTime(e.target.value)}
              min={0}
            />
          </label>

          <label>
            Hedef kalori
            <input
              type="number"
              placeholder="Örn: 600"
              value={targetCalories}
              onChange={(e) => setTargetCalories(e.target.value)}
              min={0}
            />
          </label>

          <label>
            Diyet tipi (opsiyonel)
            <select
              value={dietType}
              onChange={(e) => setDietType(e.target.value)}
            >
              <option value="">Seçilmedi</option>
              <option value="Normal">Normal</option>
              <option value="Vejetaryen">Vejetaryen</option>
              <option value="Vegan">Vegan</option>
              <option value="Ketojenik">Ketojenik</option>
            </select>
          </label>

          <label>
            Gösterilecek sonuç sayısı
            <input
              type="number"
              value={top}
              onChange={(e) => setTop(e.target.value)}
              min={1}
              max={20}
            />
          </label>
        </div>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      <button
        className="search-btn"
        onClick={getRecommendations}
        disabled={isLoading}
      >
        {isLoading ? "Öneriler getiriliyor..." : "Önerileri Getir"}
      </button>

      <div className="results">
        {!isLoading && results.length > 0 && (
          <h2 className="results-title">Senin için önerilen tarifler</h2>
        )}

        {isLoading && (
          <p className="muted-text">
            Öneriler hesaplanıyor, lütfen bekleyin...
          </p>
        )}

        {!isLoading && hasSearched && results.length === 0 && !error && (
          <p className="muted-text">
            Şu anda gösterilecek sonuç yok. Filtreleri değiştirerek tekrar
            deneyebilirsin.
          </p>
        )}

        <div className="results-grid">
          {results.map((r) => (
            <div key={r.recipeId} className="result-card">
              <div className="result-header">
                <h3
                  className="result-title-clickable"
                  onClick={() => navigate(`/recipes/${r.recipeId}`)}
                >
                  {r.name}
                </h3>
                <span className="score-pill">
                  Skor: {Number(r.score ?? 0).toFixed(3)}
                </span>
              </div>

              {r.description && (
                <p className="result-description">{r.description}</p>
              )}

              <div className="result-meta">
                <span>⏱ Süre: {r.cookingTime ?? "?"} dk</span>
                {r.dietType && <span> Diyet: {r.dietType}</span>}
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
