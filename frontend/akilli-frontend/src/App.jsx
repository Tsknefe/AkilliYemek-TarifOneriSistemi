import { BrowserRouter, Routes, Route } from "react-router-dom";
import Navbar from "./components/Navbar.jsx";   
import RecommendationPage from "./pages/RecommendationPage.jsx";
import RecipeDetailPage from "./pages/RecipeDetailPage.jsx";
import MealPlanPage from "./pages/MealPlanPage.jsx";

import HomePage from "./pages/HomePage.jsx";
import RecipesPage from "./pages/RecipesPage.jsx";
import "./style.css"; 

export default function App() {
  return (
    <BrowserRouter>
      <div className="app-shell">
        <Navbar />

        <main className="content">
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/recipes" element={<RecipesPage />} />
              <Route path="/recipes/:id" element={<RecipeDetailPage />} />
            <Route path="/recommendations" element={<RecommendationPage />} />
            <Route path="/meal-plan" element={<MealPlanPage />} />


          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}
