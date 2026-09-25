import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import Layout from './components/Layout';
import Login from './pages/Login';
import AnaSayfa from './pages/AnaSayfa';
import HastaKayit from './pages/HastaKayit';
import Randevu from './pages/Randevu';
import Poliklinik from './pages/Poliklinik';
import SistemYonetimi from './pages/SistemYonetimi';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <Layout />
              </ProtectedRoute>
            }
          >
            <Route index element={<AnaSayfa />} />
            <Route path="hasta-kayit" element={<HastaKayit />} />
            <Route path="randevu" element={<Randevu />} />
            <Route path="poliklinik" element={<Poliklinik />} />
            <Route path="sistem-yonetimi" element={<SistemYonetimi />} />
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
