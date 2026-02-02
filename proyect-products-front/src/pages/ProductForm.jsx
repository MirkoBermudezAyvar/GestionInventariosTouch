import { useState, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { productService, categoryService } from '../services'
import { Button, Input, Select, Card, Spinner } from '../components/ui'
import toast from 'react-hot-toast'

export default function ProductForm() {
  const navigate = useNavigate()
  const { id } = useParams()
  const isEdit = Boolean(id)
  
  const [loading, setLoading] = useState(false)
  const [loadingData, setLoadingData] = useState(isEdit)
  const [categories, setCategories] = useState([])
  
  const { register, handleSubmit, formState: { errors }, reset } = useForm()

  useEffect(() => {
    loadCategories()
    if (isEdit) loadProduct()
  }, [id])

  const loadCategories = async () => {
    try {
      const { data } = await categoryService.getAll()
      if (data.isSuccess) setCategories(data.data)
    } catch {}
  }

  const loadProduct = async () => {
    try {
      const { data } = await productService.getById(id)
      if (data.isSuccess) {
        reset({
          name: data.data.name,
          description: data.data.description || '',
          price: data.data.price,
          stockQuantity: data.data.stockQuantity,
          categoryId: data.data.categoryId || '',
          sku: data.data.sku || ''
        })
      } else {
        toast.error('Producto no encontrado')
        navigate('/products')
      }
    } catch {
      toast.error('Error al cargar producto')
      navigate('/products')
    } finally {
      setLoadingData(false)
    }
  }

  const onSubmit = async (formData) => {
    setLoading(true)
    try {
      const payload = {
        ...formData,
        price: parseFloat(formData.price),
        stockQuantity: parseInt(formData.stockQuantity),
        categoryId: formData.categoryId || null
      }

      let result
      if (isEdit) {
        result = await productService.update(id, { id, ...payload })
      } else {
        result = await productService.create(payload)
      }

      if (result.data.isSuccess) {
        toast.success(isEdit ? 'Producto actualizado' : 'Producto creado')
        navigate('/products')
      } else {
        toast.error(result.data.message)
      }
    } catch (err) {
      toast.error(err.response?.data?.message || 'Error al guardar')
    } finally {
      setLoading(false)
    }
  }

  if (loadingData) {
    return (
      <div className="flex justify-center py-12">
        <Spinner />
      </div>
    )
  }

  return (
    <div className="max-w-2xl mx-auto">
      <h1 className="text-2xl font-bold mb-6">
        {isEdit ? 'Editar Producto' : 'Nuevo Producto'}
      </h1>

      <Card>
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

          <div className="grid grid-cols-2 gap-4">
            <Input
              label="Precio *"
              type="number"
              step="0.01"
              min="0"
              {...register('price', { 
                required: 'El precio es requerido',
                min: { value: 0.01, message: 'Debe ser mayor a 0' }
              })}
              error={errors.price?.message}
            />

            <Input
              label="Cantidad en Stock *"
              type="number"
              min="0"
              {...register('stockQuantity', { 
                required: 'La cantidad es requerida',
                min: { value: 0, message: 'No puede ser negativa' }
              })}
              error={errors.stockQuantity?.message}
            />
          </div>

          <Select
            label="Categoría"
            {...register('categoryId')}
            options={categories.map(c => ({ value: c.id, label: c.name }))}
          />

          <Input
            label="SKU"
            {...register('sku')}
            placeholder="Código opcional"
          />

          <div className="flex justify-end gap-3 pt-4">
            <Button type="button" variant="secondary" onClick={() => navigate('/products')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Actualizar' : 'Crear'} Producto
            </Button>
          </div>
        </form>
      </Card>
    </div>
  )
}
