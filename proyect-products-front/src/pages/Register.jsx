import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { useAuthStore } from '../store/authStore'
import { Button, Input, Card, Select } from '../components/ui'
import toast from 'react-hot-toast'

export default function Register() {
  const navigate = useNavigate()
  const registerUser = useAuthStore(s => s.register)
  const [loading, setLoading] = useState(false)
  const { register, handleSubmit, formState: { errors }, watch } = useForm()
  const password = watch('password')

  const onSubmit = async (formData) => {
    setLoading(true)
    try {
      const result = await registerUser({
        email: formData.email,
        password: formData.password,
        firstName: formData.firstName,
        lastName: formData.lastName,
        role: parseInt(formData.role) // 1 = Employee, 2 = Administrator
      })
      if (result.isSuccess) {
        toast.success('Registro exitoso!')
        navigate('/products')
      } else {
        toast.error(result.message || 'Error en el registro')
      }
    } catch (err) {
      const errorMsg = err.response?.data?.message || err.response?.data?.Message || 'Error al registrar'
      toast.error(errorMsg)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4 py-8">
      <Card className="w-full max-w-md">
        <div className="text-center mb-8">
          <h1 className="text-2xl font-bold text-gray-900">Crear Cuenta</h1>
          <p className="text-gray-500 mt-1">Sistema de Gestión de Inventarios</p>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <Input
              label="Nombre"
              {...register('firstName', { required: 'Requerido' })}
              error={errors.firstName?.message}
            />
            <Input
              label="Apellido"
              {...register('lastName', { required: 'Requerido' })}
              error={errors.lastName?.message}
            />
          </div>

          <Input
            label="Correo electrónico"
            type="email"
            {...register('email', { 
              required: 'El correo es requerido',
              pattern: { value: /^\S+@\S+$/i, message: 'Correo inválido' }
            })}
            error={errors.email?.message}
          />

          <Input
            label="Contraseña"
            type="password"
            {...register('password', { 
              required: 'La contraseña es requerida',
              minLength: { value: 6, message: 'Mínimo 6 caracteres' },
              pattern: {
                value: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/,
                message: 'Debe tener mayúscula, minúscula y número'
              }
            })}
            error={errors.password?.message}
          />
          <p className="text-xs text-gray-500 -mt-2">
            Debe contener: mayúscula, minúscula y número
          </p>

          <Input
            label="Confirmar contraseña"
            type="password"
            {...register('confirmPassword', { 
              required: 'Confirme su contraseña',
              validate: v => v === password || 'Las contraseñas no coinciden'
            })}
            error={errors.confirmPassword?.message}
          />

          <Select
            label="Rol"
            {...register('role', { required: 'Seleccione un rol' })}
            options={[
              { value: '1', label: 'Empleado' },
              { value: '2', label: 'Administrador' }
            ]}
            error={errors.role?.message}
          />

          <Button type="submit" className="w-full" loading={loading}>
            Registrarme
          </Button>
        </form>

        <p className="text-center text-sm text-gray-500 mt-6">
          ¿Ya tienes cuenta?{' '}
          <Link to="/login" className="text-blue-600 hover:underline">
            Inicia sesión
          </Link>
        </p>
      </Card>
    </div>
  )
}
