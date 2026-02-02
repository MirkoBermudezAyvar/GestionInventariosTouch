import axios from 'axios'
import { useAuthStore } from '../store/authStore'

// En desarrollo usamos el proxy de Vite (/api -> https://localhost:53057/api)
// En producción usamos la URL directa
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '/api',
  headers: { 'Content-Type': 'application/json' }
})

api.interceptors.request.use(config => {
  const token = useAuthStore.getState().token
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

api.interceptors.response.use(
  response => response,
  async error => {
    const originalRequest = error.config
    
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true
      
      const { refreshToken, refresh, logout } = useAuthStore.getState()
      
      if (refreshToken) {
        try {
          await refresh()
          const newToken = useAuthStore.getState().token
          originalRequest.headers.Authorization = `Bearer ${newToken}`
          return api(originalRequest)
        } catch {
          logout()
        }
      } else {
        logout()
      }
    }
    return Promise.reject(error)
  }
)

export default api
