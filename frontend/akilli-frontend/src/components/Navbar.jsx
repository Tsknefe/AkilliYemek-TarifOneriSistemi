// src/components/Navbar.jsx
import { Link, NavLink } from "react-router-dom";

export default function Navbar() {
  return (
    <nav className="navbar">
      <Link to="/" className="brand">
        AkıllıYemek
      </Link>

      <div className="nav-links">
        <NavLink to="/" end>
          Ana Sayfa
        </NavLink>

        <NavLink to="/recipes">
          Tarifler
        </NavLink>

        <NavLink to="/recommendations">
          Öneri Motoru
        </NavLink>

        <NavLink to="/meal-plan">
          Haftalık Plan
        </NavLink>
      </div>
    </nav>
  );
}
