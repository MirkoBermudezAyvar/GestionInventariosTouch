import api from './api'

export const authService = {
  login: (data) => api.post('/auth/login', data),
  register: (data) => api.post('/auth/register', data),
  refreshToken: (data) => api.post('/auth/refresh-token', data)
}

export const productService = {
  getAll: (params) => api.get('/products', { params }),
  getById: (id) => api.get(`/products/${id}`),
  create: (data) => api.post('/products', data),
  update: (id, data) => api.put(`/products/${id}`, data),
  delete: (id) => api.delete(`/products/${id}`),
  getLowStock: (threshold = 5) => api.get('/products/low-stock', { params: { threshold } }),
  reportLowStock: (id) => api.post(`/products/${id}/report-low-stock`)
}

export const categoryService = {
  getAll: () => api.get('/categories'),
  getById: (id) => api.get(`/categories/${id}`),
  create: (data) => api.post('/categories', data),
  update: (id, data) => api.put(`/categories/${id}`, data),
  delete: (id) => api.delete(`/categories/${id}`)
}

export const notificationService = {
  getAll: (params) => api.get('/notifications', { params }),
  getUnreadCount: () => api.get('/notifications/unread-count'),
  markAsRead: (id) => api.put(`/notifications/${id}/read`),
  markAllAsRead: () => api.put('/notifications/read-all')
}

export const reportService = {
  getLowStockPdf: (threshold = 5) => api.get('/reports/low-stock/pdf', { 
    params: { threshold }, 
    responseType: 'blob' 
  }),
  getInventoryPdf: () => api.get('/reports/inventory/pdf', { responseType: 'blob' })
}
