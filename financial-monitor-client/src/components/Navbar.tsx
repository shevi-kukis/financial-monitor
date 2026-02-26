import { Link, useLocation } from "react-router-dom";
import "../styles/monitor.css";

function Navbar() {
  const location = useLocation();

  return (
    <nav className="navbar">
      <span className="navbar-brand">FinMonitor</span>

      <Link
        to="/monitor"
        className={`nav-link${location.pathname === "/monitor" ? " active" : ""}`}
      >
        Monitor
      </Link>

      <Link
        to="/add"
        className={`nav-link${location.pathname === "/add" ? " active" : ""}`}
      >
        Add
      </Link>
    </nav>
  );
}

export default Navbar;