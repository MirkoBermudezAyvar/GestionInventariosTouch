import { useState, useEffect } from 'react'
import { useOutletContext } from 'react-router-dom'
import { notificationService } from '../services'
import { Button, Card, Spinner, EmptyState, Badge } from '../components/ui'
import { Bell, Check, CheckCheck } from 'lucide-react'
import toast from 'react-hot-toast'

export default function Notifications() {
  const { refreshNotifications } = useOutletContext()
  const [notifications, setNotifications] = useState([])
  const [loading, setLoading] = useState(true)
  const [pagination, setPagination] = useState({ page: 1, pageSize: 10, totalPages: 1 })

  useEffect(() => {
    loadNotifications()
  }, [pagination.page])

  const loadNotifications = async () => {
    setLoading(true)
    try {
      const { data } = await notificationService.getAll({
        page: pagination.page,
        pageSize: pagination.pageSize
      })
      if (data.isSuccess) {
        setNotifications(data.data.items)
        setPagination(p => ({ ...p, totalPages: data.data.totalPages }))
      }
    } catch {
      toast.error('Error al cargar notificaciones')
    } finally {
      setLoading(false)
    }
  }

  const handleMarkAsRead = async (id) => {
    try {
      const { data } = await notificationService.markAsRead(id)
      if (data.isSuccess) {
        setNotifications(prev => 
          prev.map(n => n.id === id ? { ...n, isRead: true } : n)
        )
        refreshNotifications?.()
      }
    } catch {
      toast.error('Error al marcar como leída')
    }
  }

  const handleMarkAllAsRead = async () => {
    try {
      const { data } = await notificationService.markAllAsRead()
      if (data.isSuccess) {
        setNotifications(prev => prev.map(n => ({ ...n, isRead: true })))
        refreshNotifications?.()
        toast.success('Todas marcadas como leídas')
      }
    } catch {
      toast.error('Error al marcar todas')
    }
  }

  const formatDate = (dateStr) => {
    const date = new Date(dateStr)
    return date.toLocaleDateString('es-PE', { 
      day: '2-digit', 
      month: 'short', 
      hour: '2-digit', 
      minute: '2-digit' 
    })
  }

  const unreadCount = notifications.filter(n => !n.isRead).length

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">Notificaciones</h1>
        {unreadCount > 0 && (
          <Button variant="secondary" onClick={handleMarkAllAsRead}>
            <CheckCheck size={20} /> Marcar todas como leídas
          </Button>
        )}
      </div>

      <Card>
        {loading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : notifications.length === 0 ? (
          <EmptyState
            icon={Bell}
            title="No hay notificaciones"
            description="Las notificaciones aparecerán aquí"
          />
        ) : (
          <>
            <div className="divide-y">
              {notifications.map(notification => (
                <div 
                  key={notification.id} 
                  className={`py-4 flex items-start gap-4 ${!notification.isRead ? 'bg-blue-50 -mx-6 px-6' : ''}`}
                >
                  <div className={`w-10 h-10 rounded-full flex items-center justify-center ${
                    notification.type === 'LowStock' ? 'bg-yellow-100 text-yellow-600' : 'bg-blue-100 text-blue-600'
                  }`}>
                    <Bell size={20} />
                  </div>
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2 mb-1">
                      <p className="font-medium">{notification.title}</p>
                      {!notification.isRead && <Badge variant="warning">Nueva</Badge>}
                    </div>
                    <p className="text-gray-600">{notification.message}</p>
                    <p className="text-sm text-gray-400 mt-1">{formatDate(notification.createdAt)}</p>
                  </div>
                  {!notification.isRead && (
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => handleMarkAsRead(notification.id)}
                      title="Marcar como leída"
                    >
                      <Check size={18} />
                    </Button>
                  )}
                </div>
              ))}
            </div>

            {pagination.totalPages > 1 && (
              <div className="flex justify-center gap-2 mt-6">
                <Button
                  variant="secondary"
                  size="sm"
                  disabled={pagination.page === 1}
                  onClick={() => setPagination(p => ({ ...p, page: p.page - 1 }))}
                >
                  Anterior
                </Button>
                <span className="px-4 py-2 text-sm">
                  Página {pagination.page} de {pagination.totalPages}
                </span>
                <Button
                  variant="secondary"
                  size="sm"
                  disabled={pagination.page === pagination.totalPages}
                  onClick={() => setPagination(p => ({ ...p, page: p.page + 1 }))}
                >
                  Siguiente
                </Button>
              </div>
            )}
          </>
        )}
      </Card>
    </div>
  )
}
