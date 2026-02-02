import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { useAuthStore } from '../store/authStore'
import { Button, Input, Card } from '../components/ui'
import toast from 'react-hot-toast'

export default function Login() {
  const navigate = useNavigate()
  const login = useAuthStore(s => s.login)
  const [loading, setLoading] = useState(false)
  const { register, handleSubmit, formState: { errors } } = useForm()

  const onSubmit = async (formData) => {
    setLoading(true)
    try {
      const result = await login(formData.email, formData.password)
      if (result.isSuccess) {
        toast.success('Bienvenido!')
        navigate('/products')
      } else {
        toast.error(result.message || 'Credenciales inválidas')
      }
    } catch (err) {
      toast.error(err.response?.data?.message || 'Error al iniciar sesión')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
      <Card className="w-full max-w-md">
        <div className="text-center mb-8">
          <h1 className="text-2xl font-bold text-gray-900">Iniciar Sesión</h1>
          <p className="text-gray-500 mt-1">Sistema de Gestión de Inventarios</p>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <Input
            label="Correo electrónico"
            type="email"
            {...register('email', { required: 'El correo es requerido' })}
            error={errors.email?.message}
          />

          <Input
            label="Contraseña"
            type="password"
            {...register('password', { required: 'La contraseña es requerida' })}
            error={errors.password?.message}
          />

          <Button type="submit" className="w-full" loading={loading}>
            Ingresar
          </Button>
        </form>

        <p className="text-center text-sm text-gray-500 mt-6">
          ¿No tienes cuenta?{' '}
          <Link to="/register" className="text-blue-600 hover:underline">
            Regístrate aquí
          </Link>
        </p>
      </Card>
    </div>
  )
}
