import { useState, useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { categoryService } from '../services'
import { Button, Input, Card, Spinner, EmptyState, Modal } from '../components/ui'
import { Plus, Edit, Trash2, Tags } from 'lucide-react'
import toast from 'react-hot-toast'

export default function Categories() {
  const [categories, setCategories] = useState([])
  const [loading, setLoading] = useState(true)
  const [modal, setModal] = useState({ open: false, category: null })
  const [deleteModal, setDeleteModal] = useState({ open: false, category: null })
  const [saving, setSaving] = useState(false)

  const { register, handleSubmit, formState: { errors }, reset } = useForm()

  useEffect(() => {
    loadCategories()
  }, [])

  const loadCategories = async () => {
    setLoading(true)
    try {
      const { data } = await categoryService.getAll()
      if (data.isSuccess) setCategories(data.data)
    } catch {
      toast.error('Error al cargar categorías')
    } finally {
      setLoading(false)
    }
  }

  const openModal = (category = null) => {
    reset(category ? { name: category.name, description: category.description || '' } : { name: '', description: '' })
    setModal({ open: true, category })
  }

  const onSubmit = async (formData) => {
    setSaving(true)
    try {
      let result
      if (modal.category) {
        result = await categoryService.update(modal.category.id, { id: modal.category.id, ...formData })
      } else {
        result = await categoryService.create(formData)
      }

      if (result.data.isSuccess) {
        toast.success(modal.category ? 'Categoría actualizada' : 'Categoría creada')
        loadCategories()
        setModal({ open: false, category: null })
      } else {
        toast.error(result.data.message)
      }
    } catch (err) {
      toast.error(err.response?.data?.message || 'Error al guardar')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async () => {
    try {
      const { data } = await categoryService.delete(deleteModal.category.id)
      if (data.isSuccess) {
        toast.success('Categoría eliminada')
        loadCategories()
      } else {
        toast.error(data.message)
      }
    } catch (err) {
      toast.error(err.response?.data?.message || 'Error al eliminar')
    } finally {
      setDeleteModal({ open: false, category: null })
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">Categorías</h1>
        <Button onClick={() => openModal()}>
          <Plus size={20} /> Nueva Categoría
        </Button>
      </div>

      <Card>
        {loading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : categories.length === 0 ? (
          <EmptyState
            icon={Tags}
            title="No hay categorías"
            description="Crea tu primera categoría"
            action={<Button onClick={() => openModal()}>Crear Categoría</Button>}
          />
        ) : (
          <div className="divide-y">
            {categories.map(category => (
              <div key={category.id} className="py-4 flex items-center justify-between">
                <div>
                  <p className="font-medium">{category.name}</p>
                  {category.description && (
                    <p className="text-sm text-gray-500">{category.description}</p>
                  )}
                </div>
                <div className="flex gap-1">
                  <Button variant="ghost" size="sm" onClick={() => openModal(category)}>
                    <Edit size={18} />
                  </Button>
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => setDeleteModal({ open: true, category })}
                  >
                    <Trash2 size={18} className="text-red-500" />
                  </Button>
                </div>
              </div>
            ))}
          </div>
        )}
      </Card>

      <Modal
        open={modal.open}
        onClose={() => setModal({ open: false, category: null })}
        title={modal.category ? 'Editar Categoría' : 'Nueva Categoría'}
      >
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <Input
            label="Nombre *"
            {...register('name', { required: 'El nombre es requerido' })}
            error={errors.name?.message}
          />
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Descripción</label>
            <textarea
              rows={3}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-200"
              {...register('description')}
            />
          </div>
          <div className="flex justify-end gap-3">
            <Button type="button" variant="secondary" onClick={() => setModal({ open: false, category: null })}>
              Cancelar
            </Button>
            <Button type="submit" loading={saving}>
              {modal.category ? 'Actualizar' : 'Crear'}
            </Button>
          </div>
        </form>
      </Modal>

      <Modal
        open={deleteModal.open}
        onClose={() => setDeleteModal({ open: false, category: null })}
        title="Eliminar Categoría"
      >
        <p className="text-gray-600 mb-6">
          ¿Estás seguro de eliminar <strong>{deleteModal.category?.name}</strong>?
        </p>
        <div className="flex justify-end gap-3">
          <Button variant="secondary" onClick={() => setDeleteModal({ open: false, category: null })}>
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
