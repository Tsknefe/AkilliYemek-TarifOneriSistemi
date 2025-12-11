import { useEffect, useState } from "react";
import api from "../api/client";
import "./MealPlanPage.css";
import { useNavigate } from "react-router-dom";

// burada haftanın günlerini sabit tuttuk
const DAYS = [
  "Pazartesi",
  "Salı",
  "Çarşamba",
  "Perşembe",
  "Cuma",
  "Cumartesi",
  "Pazar",
];

// burada da bir günde planladığımız öğün tipleri
const MEALS = ["Kahvaltı", "Öğle", "Akşam"];

export default function MealPlanPage() {
  // tarifleri burada tutuyoruz backendden gelenler
  const [recipes, setRecipes] = useState([]);

  // oluşturulan haftalık plan buraya düşüyor
  const [plan, setPlan] = useState([]);

  // kullanıcının günlük kalori hedefi
  const [dailyTarget, setDailyTarget] = useState("");

  // yükleniyor ve hata durumları
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");

  const navigate = useNavigate();

  // sayfa açılınca tarifleri backendden çekiyoruz
  useEffect(() => {
    const load = async () => {
      setIsLoading(true);
      setError("");

      try {
        const res = await api.get("/RecipeApi");
        setRecipes(res.data || []);
      } catch (err) {
        console.error(err);
        setError("Tarifler yüklenirken bir hata oluştu");
      } finally {
        setIsLoading(false);
      }
    };

    load();
  }, []);

  // burası plan oluşturma fonksiyonu
  // elimizdeki tarifleri sırayla günlere ve öğünlere dağıtıyoruz
  const generatePlan = () => {
    // hiç tarif yoksa kullanıcıya error ver
    if (!recipes.length) {
      setError("Plan oluşturmak için en az bir tarif gerekli");
      return;
    }
    setError("");

    let idx = 0;

    // her gün için bir obje oluşturuyoruz
    const built = DAYS.map((day) => {
      // gün içindeki kahvaltı öğle akşam gibi öğünler
      const meals = MEALS.map((mealName) => {
        // tarifleri sırayla dolaşmak için idx kullanıyoruz
        const recipe = recipes[idx % recipes.length];
        idx += 1;

        const cal = recipe.nutritionFacts?.calories ?? null;
        return { mealName, recipe, calories: cal };
      });

      // o güne ait toplam kaloriyi hesaplıyoruz
      const totalCalories = meals.reduce(
        (sum, m) => sum + (typeof m.calories === "number" ? m.calories : 0),
        0
      );

      // her gün için day, meals, totalCalories şeklinde dönen yapı
      return { day, meals, totalCalories };
    });

    setPlan(built);
  };

  // render kısmı
  return (
    <div className="meal-wrapper">
      {/* başlık ve kısa açıklama */}
      <h1 className="page-title">Haftalık Beslenme Planı</h1>
      <p className="page-subtitle">
        Tariflerini kullanarak 7 günlük örnek bir beslenme planı oluştur
      </p>

      {/* plan oluşturma kontrolleri */}
      <div className="meal-controls">
        <label>
          Günlük kalori hedefi (opsiyonel)
          <input
            type="number"
            placeholder="Örn: 2000"
            value={dailyTarget}
            onChange={(e) => setDailyTarget(e.target.value)}
          />
        </label>

        <button
          className="generate-btn"
          onClick={generatePlan}
          disabled={isLoading}
        >
          {isLoading ? "Tarifler yükleniyor..." : "Plan Oluştur"}
        </button>
      </div>

      {/* hata varsa gösterelim */}
      {error && <div className="alert alert-error">{error}</div>}

      {/* daha plan oluşturulmadıysa bilgilendirme yazısı */}
      {!error && !isLoading && plan.length === 0 && (
        <p className="muted-text">
          Henüz plan oluşturmadın üstten "Plan Oluştur" butonuna tıkla
        </p>
      )}

      {/* haftalık plan grid */}
      <div className="meal-grid">
        {plan.map((dayPlan) => {
          // kullanıcı hedef girmişse hedef ile gün toplamı arasındaki farkı hesaplıyoruz
          const target = Number(dailyTarget) || null;
          const diff = target ? dayPlan.totalCalories - target : null;

          return (
            <div key={dayPlan.day} className="meal-day-card">
              {/* gün başlığı ve toplam kalori bilgisi */}
              <div className="meal-day-header">
                <h2>{dayPlan.day}</h2>
                <div className="meal-cal">
                  Toplam{" "}
                  {dayPlan.totalCalories
                    ? `${dayPlan.totalCalories} kcal`
                    : "?"}
                  {target && (
                    <span className="meal-cal-diff">
                      {diff > 0
                        ? ` (+${diff.toFixed(0)} kcal)`
                        : ` (${diff.toFixed(0)} kcal)`}
                    </span>
                  )}
                </div>
              </div>

              {/* o güne ait öğün listesi */}
              <div className="meal-list">
                {dayPlan.meals.map((m, i) => (
                  <div key={i} className="meal-item">
                    <div className="meal-title-row">
                      {/* kahvaltı öğle akşam gibi tür */}
                      <span className="meal-type">{m.mealName}</span>

                      {/* öğüne ait tarifin kalorisi varsa gösteriyoruz */}
                      {typeof m.calories === "number" && (
                        <span className="meal-kcal">
                          {m.calories} kcal
                        </span>
                      )}
                    </div>

                    {/* tarif ismi tıklanabilir detaya götürüyor */}
                    <div
                      className="meal-recipe-name clickable"
                      onClick={() => {
                        if (m.recipe?.id) {
                          navigate(`/recipes/${m.recipe.id}`);
                        }
                      }}
                    >
                      {m.recipe?.name || "Tarif adı yok"}
                    </div>
                  </div>
                ))}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
