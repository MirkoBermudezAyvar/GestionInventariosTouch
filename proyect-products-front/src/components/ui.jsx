import { forwardRef } from 'react'
import clsx from 'clsx'

export const Button = forwardRef(({ 
  children, 
  variant = 'primary', 
  size = 'md', 
  className, 
  loading,
  ...props 
}, ref) => {
  const variants = {
    primary: 'bg-blue-600 text-white hover:bg-blue-700 disabled:bg-blue-300',
    secondary: 'bg-gray-100 text-gray-700 hover:bg-gray-200',
    danger: 'bg-red-600 text-white hover:bg-red-700',
    ghost: 'text-gray-600 hover:bg-gray-100'
  }

  const sizes = {
    sm: 'px-3 py-1.5 text-sm',
    md: 'px-4 py-2',
    lg: 'px-6 py-3 text-lg'
  }

  return (
    <button
      ref={ref}
      className={clsx(
        'inline-flex items-center justify-center gap-2 font-medium rounded-lg transition-colors disabled:cursor-not-allowed',
        variants[variant],
        sizes[size],
        className
      )}
      disabled={loading || props.disabled}
      {...props}
    >
      {loading && <span className="w-4 h-4 border-2 border-current border-t-transparent rounded-full animate-spin" />}
      {children}
    </button>
  )
})

export const Input = forwardRef(({ label, error, className, ...props }, ref) => (
  <div className={className}>
    {label && <label className="block text-sm font-medium text-gray-700 mb-1">{label}</label>}
    <input
      ref={ref}
      className={clsx(
        'w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 transition-colors',
        error ? 'border-red-300 focus:ring-red-200' : 'border-gray-300 focus:ring-blue-200 focus:border-blue-400'
      )}
      {...props}
    />
    {error && <p className="mt-1 text-sm text-red-500">{error}</p>}
  </div>
))

export const Select = forwardRef(({ label, error, options = [], className, ...props }, ref) => (
  <div className={className}>
    {label && <label className="block text-sm font-medium text-gray-700 mb-1">{label}</label>}
    <select
      ref={ref}
      className={clsx(
        'w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 transition-colors bg-white',
        error ? 'border-red-300 focus:ring-red-200' : 'border-gray-300 focus:ring-blue-200 focus:border-blue-400'
      )}
      {...props}
    >
      <option value="">Seleccionar...</option>
      {options.map(opt => (
        <option key={opt.value} value={opt.value}>{opt.label}</option>
      ))}
    </select>
    {error && <p className="mt-1 text-sm text-red-500">{error}</p>}
  </div>
))

export const Card = ({ children, className, ...props }) => (
  <div className={clsx('bg-white rounded-xl shadow-sm border p-6', className)} {...props}>
    {children}
  </div>
)

export const Modal = ({ open, onClose, title, children }) => {
  if (!open) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div className="fixed inset-0 bg-black/50" onClick={onClose} />
      <div className="relative bg-white rounded-xl shadow-xl max-w-md w-full max-h-[90vh] overflow-auto">
        <div className="flex items-center justify-between p-4 border-b">
          <h3 className="text-lg font-semibold">{title}</h3>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">✕</button>
        </div>
        <div className="p-4">{children}</div>
      </div>
    </div>
  )
}

export const Badge = ({ children, variant = 'default' }) => {
  const variants = {
    default: 'bg-gray-100 text-gray-700',
    success: 'bg-green-100 text-green-700',
    warning: 'bg-yellow-100 text-yellow-700',
    danger: 'bg-red-100 text-red-700'
  }

  return (
    <span className={clsx('px-2 py-1 text-xs font-medium rounded-full', variants[variant])}>
      {children}
    </span>
  )
}

export const Spinner = ({ size = 'md' }) => {
  const sizes = { sm: 'w-4 h-4', md: 'w-8 h-8', lg: 'w-12 h-12' }
  return (
    <div className={clsx('border-4 border-blue-200 border-t-blue-600 rounded-full animate-spin', sizes[size])} />
  )
}

export const EmptyState = ({ icon: Icon, title, description, action }) => (
  <div className="text-center py-12">
    {Icon && <Icon className="mx-auto h-12 w-12 text-gray-400" />}
    <h3 className="mt-2 text-sm font-semibold text-gray-900">{title}</h3>
    {description && <p className="mt-1 text-sm text-gray-500">{description}</p>}
    {action && <div className="mt-6">{action}</div>}
  </div>
)
