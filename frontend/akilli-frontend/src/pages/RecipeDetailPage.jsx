import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import api from "../api/client";
import "./RecipeDetailPage.css";

// bu sayfa tariflerin detay sayfası
// kullanıcı bir tarif kartına tıklayınca buraya geliyor
// buradan tarifin açıklamasını, besin değerlerini, malzemelerini, adım adım hazırlanışını görür
// mantık olarak çok basit sadece id alıyoruz ve backendden o tarifin detayını çekiyoruz

export default function RecipeDetailPage() {

  // url’den gelen tarif idsi
  const { id } = useParams();

  // geri dönmek için
  const navigate = useNavigate();

  // tarif datamız
  const [recipe, setRecipe] = useState(null);

  // loading ve hata durumu
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");


  // sayfa açılınca seçilen tarifin detayını fetch ediyoruz
  useEffect(() => {
    const loadRecipe = async () => {
      setIsLoading(true);
      setError("");

      try {
        const res = await api.get(`/RecipeApi/${id}`);
        setRecipe(res.data);
      } catch (err) {
        console.error(err);
        setError("Tarif yüklenirken bir hata oluştu");
      } finally {
        setIsLoading(false);
      }
    };

    if (id) {
      loadRecipe();
    }
  }, [id]);


  // loading ekranı
  if (isLoading) {
    return (
      <div className="recipe-detail-wrapper">
        <p className="muted-text">Tarif yükleniyor...</p>
      </div>
    );
  }

  // hata varsa kullanıcıya gösteriyoruz
  if (error) {
    return (
      <div className="recipe-detail-wrapper">
        <div className="alert alert-error">{error}</div>
        <button className="back-button" onClick={() => navigate(-1)}>
          ← Geri dön
        </button>
      </div>
    );
  }

  // tarif bulunamazsa bu çıkıyor
  if (!recipe) {
    return (
      <div className="recipe-detail-wrapper">
        <p className="muted-text">Tarif bulunamadı</p>
        <button className="back-button" onClick={() => navigate(-1)}>
          ← Geri dön
        </button>
      </div>
    );
  }

  // backendden gelen alanlar
  const nutrition = recipe.nutritionFacts || {};
  const ingredients = recipe.recipeIngredients || [];
  const steps = recipe.instructions || recipe.steps || "";


  // buradan sonrası ekranın çizildiği yer
  return (
    <div className="recipe-detail-wrapper">

      {/* geri dön butonu */}
      <button className="back-button" onClick={() => navigate(-1)}>
        ← Geri dön
      </button>

      {/* başlık kısmı */}
      <div className="detail-header">
        <div>
          {/* tarif adı */}
          <h1 className="detail-title">{recipe.name}</h1>

          {/* süre diyet tipi kalori falan */}
          <div className="detail-meta">
            <span>
              ⏱ Süre {recipe.cookingTime != null ? `${recipe.cookingTime} dk` : "?"}
            </span>

            {recipe.dietType && <span> Diyet {recipe.dietType}</span>}

            {nutrition.calories != null && (
              <span> Kalori {nutrition.calories} kcal</span>
            )}
          </div>
        </div>

        {/* tarif açıklaması */}
        {recipe.description && (
          <p className="detail-description">{recipe.description}</p>
        )}
      </div>

      {/* tarif resmi varsa gösteriyoruz */}
      {recipe.imageUrl && (
        <div className="detail-image-wrapper">
          <img
            src={recipe.imageUrl}
            alt={recipe.name}
            className="detail-image"
          />
        </div>
      )}

      {/* iki kartlık grid alanı malzemeler ve hazırlanış */}
      <div className="detail-grid">

        {/* malzemeler */}
        <div className="detail-card">
          <h2>Malzemeler</h2>

          {ingredients.length === 0 && (
            <p className="muted-text">Malzeme bilgisi yok</p>
          )}

          <ul className="ingredients-list">
            {ingredients.map((ing, idx) => {
              // backend bazen ingredient.name dönderiyor bazen ingredientName
              const name =
                ing.ingredient?.name ||
                ing.ingredientName ||
                ing.name ||
                "Malzeme";

              return (
                <li key={idx}>
                  <span className="ingredient-name">{name}</span>
                  {ing.quantity && (
                    <span className="qty"> · {ing.quantity}</span>
                  )}
                </li>
              );
            })}
          </ul>
        </div>

        {/* hazırlanış */}
        <div className="detail-card">
          <h2>Hazırlanış</h2>

          {steps ? (
            <p className="steps-text">{steps}</p>
          ) : (
            <p className="muted-text">
              Bu tarif için hazırlanış bilgisi yok
            </p>
          )}
        </div>

      </div>
    </div>
  );
}
