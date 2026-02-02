import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuthStore } from './store/authStore'
import Layout from './components/Layout'
import Login from './pages/Login'
import Register from './pages/Register'
import Products from './pages/Products'
import ProductForm from './pages/ProductForm'
import Categories from './pages/Categories'
import Notifications from './pages/Notifications'
import Reports from './pages/Reports'

// Helper para verificar si es admin (maneja número o string)
const checkIsAdmin = (role) => {
  if (typeof role === 'string') {
    return role === 'Administrator' || role === 'administrator'
  }
  return role === 2 // Administrator = 2 en el enum del backend
}

function PrivateRoute({ children, adminOnly = false }) {
  const { isAuthenticated, user } = useAuthStore()
  
  if (!isAuthenticated) return <Navigate to="/login" replace />
  if (adminOnly && !checkIsAdmin(user?.role)) return <Navigate to="/products" replace />
  
  return children
}

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      
      <Route path="/" element={<PrivateRoute><Layout /></PrivateRoute>}>
        <Route index element={<Navigate to="/products" replace />} />
        <Route path="products" element={<Products />} />
        <Route path="products/new" element={<PrivateRoute adminOnly><ProductForm /></PrivateRoute>} />
        <Route path="products/:id/edit" element={<PrivateRoute adminOnly><ProductForm /></PrivateRoute>} />
        <Route path="categories" element={<PrivateRoute adminOnly><Categories /></PrivateRoute>} />
        <Route path="notifications" element={<Notifications />} />
        <Route path="reports" element={<PrivateRoute adminOnly><Reports /></PrivateRoute>} />
      </Route>
      
      <Route path="*" element={<Navigate to="/products" replace />} />
    </Routes>
  )
}
