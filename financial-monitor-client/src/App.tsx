import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import AddPage from "./pages/AddPage";
import MonitorPage from "./pages/MonitorPage";

import Navbar from "./components/Navbar";

function App() {
  return (
    <BrowserRouter>
      <Navbar />
      <Routes>
        <Route path="/" element={<Navigate to="/monitor" />} />
        <Route path="/add" element={<AddPage />} />
        <Route path="/monitor" element={<MonitorPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;