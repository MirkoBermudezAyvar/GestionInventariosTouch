import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import { authService } from '../services'

export const useAuthStore = create(
  persist(
    (set, get) => ({
      user: null,
      token: null,
      refreshToken: null,
      isAuthenticated: false,

      login: async (email, password) => {
        const { data } = await authService.login({ email, password })
        if (data.isSuccess) {
          set({
            user: data.data.user,
            token: data.data.accessToken,
            refreshToken: data.data.refreshToken,
            isAuthenticated: true
          })
        }
        return data
      },

      register: async (userData) => {
        const { data } = await authService.register(userData)
        if (data.isSuccess) {
          set({
            user: data.data.user,
            token: data.data.accessToken,
            refreshToken: data.data.refreshToken,
            isAuthenticated: true
          })
        }
        return data
      },

      refresh: async () => {
        const { token, refreshToken } = get()
        const { data } = await authService.refreshToken({ 
          accessToken: token, 
          refreshToken 
        })
        if (data.isSuccess) {
          set({
            token: data.data.accessToken,
            refreshToken: data.data.refreshToken
          })
        }
        return data
      },

      logout: () => {
        set({
          user: null,
          token: null,
          refreshToken: null,
          isAuthenticated: false
        })
      }
    }),
    { name: 'auth-storage' }
  )
)
