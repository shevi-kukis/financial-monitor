import { Link } from "react-router-dom";

function Navbar() {
  return (
    <nav style={{ padding: 10, background: "#eee" }}>
      <Link to="/monitor" style={{ marginRight: 10 }}>
        Monitor
      </Link>
      <Link to="/add">
        Add
      </Link>
    </nav>
  );
}

export default Navbar;