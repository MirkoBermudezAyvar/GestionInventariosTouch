import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { useAuthStore } from '../store/authStore'
import { productService, categoryService } from '../services'
import { Button, Card, Badge, Spinner, EmptyState, Modal } from '../components/ui'
import { Plus, Search, Edit, Trash2, AlertTriangle, Package } from 'lucide-react'
import toast from 'react-hot-toast'

// Helper para verificar si es admin
const checkIsAdmin = (role) => {
  if (typeof role === 'string') {
    return role === 'Administrator' || role === 'administrator'
  }
  return role === 2
}

export default function Products() {
  const { user } = useAuthStore()
  const isAdmin = checkIsAdmin(user?.role)
  
  const [products, setProducts] = useState([])
  const [categories, setCategories] = useState([])
  const [loading, setLoading] = useState(true)
  const [pagination, setPagination] = useState({ page: 1, pageSize: 10, totalPages: 1 })
  const [filters, setFilters] = useState({ searchTerm: '', categoryId: '' })
  const [deleteModal, setDeleteModal] = useState({ open: false, product: null })

  useEffect(() => {
    loadCategories()
  }, [])

  useEffect(() => {
    loadProducts()
  }, [pagination.page, filters])

  const loadCategories = async () => {
    try {
      const { data } = await categoryService.getAll()
      if (data.isSuccess) setCategories(data.data)
    } catch {}
  }

  const loadProducts = async () => {
    setLoading(true)
    try {
      const { data } = await productService.getAll({
        page: pagination.page,
        pageSize: pagination.pageSize,
        searchTerm: filters.searchTerm || undefined,
        categoryId: filters.categoryId || undefined
      })
      if (data.isSuccess) {
        setProducts(data.data.items)
        setPagination(p => ({ ...p, totalPages: data.data.totalPages }))
      }
    } catch (err) {
      toast.error('Error al cargar productos')
    } finally {
      setLoading(false)
    }
  }

  const handleDelete = async () => {
    try {
      const { data } = await productService.delete(deleteModal.product.id)
      if (data.isSuccess) {
        toast.success('Producto eliminado')
        loadProducts()
      } else {
        toast.error(data.message)
      }
    } catch (err) {
      toast.error('Error al eliminar')
    } finally {
      setDeleteModal({ open: false, product: null })
    }
  }

  const handleReportLowStock = async (product) => {
    try {
      const { data } = await productService.reportLowStock(product.id)
      if (data.isSuccess) {
        toast.success('Stock bajo reportado al administrador')
      } else {
        toast.error(data.message)
      }
    } catch {
      toast.error('Error al reportar')
    }
  }

  const getCategoryName = (categoryId) => {
    const cat = categories.find(c => c.id === categoryId)
    return cat?.name || '-'
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <h1 className="text-2xl font-bold">Productos</h1>
        {isAdmin && (
          <Link to="/products/new">
            <Button><Plus size={20} /> Nuevo Producto</Button>
          </Link>
        )}
      </div>

      <Card>
        <div className="flex flex-col sm:flex-row gap-4 mb-6">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" size={20} />
            <input
              type="text"
              placeholder="Buscar productos..."
              className="w-full pl-10 pr-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-200"
              value={filters.searchTerm}
              onChange={e => setFilters(f => ({ ...f, searchTerm: e.target.value }))}
            />
          </div>
          <select
            className="px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-200"
            value={filters.categoryId}
            onChange={e => setFilters(f => ({ ...f, categoryId: e.target.value }))}
          >
            <option value="">Todas las categorías</option>
            {categories.map(cat => (
              <option key={cat.id} value={cat.id}>{cat.name}</option>
            ))}
          </select>
        </div>

        {loading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : products.length === 0 ? (
          <EmptyState
            icon={Package}
            title="No hay productos"
            description={isAdmin ? "Comienza agregando tu primer producto" : "No hay productos disponibles"}
            action={isAdmin && <Link to="/products/new"><Button>Agregar Producto</Button></Link>}
          />
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b text-left">
                    <th className="pb-3 font-semibold">Producto</th>
                    <th className="pb-3 font-semibold">Categoría</th>
                    <th className="pb-3 font-semibold text-right">Precio</th>
                    <th className="pb-3 font-semibold text-right">Stock</th>
                    <th className="pb-3 font-semibold text-right">Acciones</th>
                  </tr>
                </thead>
                <tbody>
                  {products.map(product => (
                    <tr key={product.id} className="border-b last:border-0">
                      <td className="py-4">
                        <div>
                          <p className="font-medium">{product.name}</p>
                          {product.description && (
                            <p className="text-sm text-gray-500 truncate max-w-xs">{product.description}</p>
                          )}
                        </div>
                      </td>
                      <td className="py-4">{getCategoryName(product.categoryId)}</td>
                      <td className="py-4 text-right">S/ {product.price.toFixed(2)}</td>
                      <td className="py-4 text-right">
                        <div className="flex items-center justify-end gap-2">
                          {product.stockQuantity}
                          {product.isLowStock && (
                            <Badge variant="danger">Bajo</Badge>
                          )}
                        </div>
                      </td>
                      <td className="py-4">
                        <div className="flex items-center justify-end gap-1">
                          {/* Empleados pueden reportar stock bajo */}
                          {product.isLowStock && (
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => handleReportLowStock(product)}
                              title="Reportar stock bajo"
                            >
                              <AlertTriangle size={18} className="text-yellow-500" />
                            </Button>
                          )}
                          {/* Solo admin puede editar y eliminar */}
                          {isAdmin && (
                            <>
                              <Link to={`/products/${product.id}/edit`}>
                                <Button variant="ghost" size="sm"><Edit size={18} /></Button>
                              </Link>
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => setDeleteModal({ open: true, product })}
                              >
                                <Trash2 size={18} className="text-red-500" />
                              </Button>
                            </>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
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

      <Modal
        open={deleteModal.open}
        onClose={() => setDeleteModal({ open: false, product: null })}
        title="Eliminar Producto"
      >
        <p className="text-gray-600 mb-6">
          ¿Estás seguro de eliminar <strong>{deleteModal.product?.name}</strong>?
        </p>
        <div className="flex justify-end gap-3">
          <Button variant="secondary" onClick={() => setDeleteModal({ open: false, product: null })}>
            Cancelar
          </Button>
          <Button variant="danger" onClick={handleDelete}>
            Eliminar
          </Button>
        </div>
      </Modal>
    </div>
  )
}
