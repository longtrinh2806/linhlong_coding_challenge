import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Login, Register, Home } from './pages';
import ProtectedRoute from './components/ProtectedRoute';
import { AUTH_STORAGE_KEYS } from './constants';

function App() {
  const isAuthenticated = localStorage.getItem(AUTH_STORAGE_KEYS.ACCESS_TOKEN);

  return (
    <BrowserRouter>
      <Routes>
        <Route 
          path="/login" 
          element={isAuthenticated ? <Navigate to="/home" replace /> : <Login />} 
        />
        <Route 
          path="/register" 
          element={isAuthenticated ? <Navigate to="/home" replace /> : <Register />} 
        />
        <Route 
          path="/home" 
          element={
            <ProtectedRoute>
              <Home />
            </ProtectedRoute>
          } 
        />
        <Route path="/" element={<Navigate to={isAuthenticated ? "/home" : "/login"} replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
