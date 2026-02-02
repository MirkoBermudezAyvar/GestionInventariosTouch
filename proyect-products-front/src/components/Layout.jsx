import { useState, useEffect } from 'react'
import { Outlet, NavLink, useNavigate } from 'react-router-dom'
import { useAuthStore } from '../store/authStore'
import { notificationService } from '../services'
import { Package, Tags, Bell, FileText, LogOut, Menu, X } from 'lucide-react'
import clsx from 'clsx'

// Helper para verificar si es admin (maneja número o string)
const checkIsAdmin = (role) => {
  if (typeof role === 'string') {
    return role === 'Administrator' || role === 'administrator'
  }
  return role === 2 // Administrator = 2 en el enum del backend
}

export default function Layout() {
  const { user, logout } = useAuthStore()
  const navigate = useNavigate()
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const [unreadCount, setUnreadCount] = useState(0)
  const isAdmin = checkIsAdmin(user?.role)

  useEffect(() => {
    loadUnreadCount()
    const interval = setInterval(loadUnreadCount, 30000)
    return () => clearInterval(interval)
  }, [])

  const loadUnreadCount = async () => {
    try {
      const { data } = await notificationService.getUnreadCount()
      if (data.isSuccess) setUnreadCount(data.data)
    } catch {}
  }

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  const navItems = [
    { to: '/products', label: 'Productos', icon: Package },
    isAdmin && { to: '/categories', label: 'Categorías', icon: Tags },
    { to: '/notifications', label: 'Notificaciones', icon: Bell, badge: unreadCount },
    isAdmin && { to: '/reports', label: 'Reportes', icon: FileText }
  ].filter(Boolean)

  return (
    <div className="min-h-screen flex">
      {/* Sidebar mobile overlay */}
      {sidebarOpen && (
        <div className="fixed inset-0 bg-black/50 z-40 lg:hidden" onClick={() => setSidebarOpen(false)} />
      )}

      {/* Sidebar */}
      <aside className={clsx(
        "fixed lg:static inset-y-0 left-0 z-50 w-64 bg-white border-r transform transition-transform lg:transform-none",
        sidebarOpen ? "translate-x-0" : "-translate-x-full"
      )}>
        <div className="h-16 flex items-center justify-between px-4 border-b">
          <h1 className="text-xl font-bold text-blue-600">Inventario</h1>
          <button className="lg:hidden" onClick={() => setSidebarOpen(false)}>
            <X size={24} />
          </button>
        </div>

        <nav className="p-4 space-y-1">
          {navItems.map(({ to, label, icon: Icon, badge }) => (
            <NavLink
              key={to}
              to={to}
              onClick={() => setSidebarOpen(false)}
              className={({ isActive }) => clsx(
                "flex items-center gap-3 px-3 py-2 rounded-lg transition-colors",
                isActive ? "bg-blue-50 text-blue-600" : "text-gray-600 hover:bg-gray-50"
              )}
            >
              <Icon size={20} />
              <span>{label}</span>
              {badge > 0 && (
                <span className="ml-auto bg-red-500 text-white text-xs px-2 py-0.5 rounded-full">
                  {badge}
                </span>
              )}
            </NavLink>
          ))}
        </nav>

        <div className="absolute bottom-0 left-0 right-0 p-4 border-t">
          <div className="flex items-center gap-3 mb-3">
            <div className="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center text-blue-600 font-semibold">
              {user?.firstName?.[0]}{user?.lastName?.[0]}
            </div>
            <div className="flex-1 min-w-0">
              <p className="font-medium truncate">{user?.firstName} {user?.lastName}</p>
              <p className="text-sm text-gray-500">{isAdmin ? 'Administrador' : 'Empleado'}</p>
            </div>
          </div>
          <button
            onClick={handleLogout}
            className="w-full flex items-center justify-center gap-2 px-3 py-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
          >
            <LogOut size={20} />
            <span>Cerrar sesión</span>
          </button>
        </div>
      </aside>

      {/* Main content */}
      <div className="flex-1 flex flex-col min-w-0">
        <header className="h-16 bg-white border-b flex items-center px-4 lg:px-6">
          <button className="lg:hidden mr-4" onClick={() => setSidebarOpen(true)}>
            <Menu size={24} />
          </button>
          <h2 className="text-lg font-semibold">Sistema de Gestión de Inventarios</h2>
        </header>

        <main className="flex-1 p-4 lg:p-6 overflow-auto">
          <Outlet context={{ refreshNotifications: loadUnreadCount }} />
        </main>
      </div>
    </div>
  )
}
