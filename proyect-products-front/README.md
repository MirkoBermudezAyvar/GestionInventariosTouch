# Sistema de Gestión de Inventarios - Frontend

Frontend desarrollado en **React 19** con Vite para el Sistema de Gestión de Inventarios de Touch Consulting.

## 🚀 Tecnologías Utilizadas

- **React 19** - Biblioteca UI
- **Vite 6** - Build tool y dev server
- **React Router DOM 7** - Enrutamiento
- **Zustand 5** - State management
- **Axios** - Cliente HTTP
- **React Hook Form** - Manejo de formularios
- **Tailwind CSS 3** - Estilos
- **Lucide React** - Iconos
- **React Hot Toast** - Notificaciones

## 📋 Requisitos Previos

- Node.js 18 o superior
- npm o yarn
- Backend ejecutándose en `http://localhost:5000`

## 🛠️ Instalación

1. **Clonar e instalar dependencias:**

```bash
cd proyect-products-front
npm install
```

2. **Configurar variables de entorno (opcional):**

El frontend está configurado para conectarse al backend en `http://localhost:5000/api`. 
Si necesitas cambiar esto, modifica el archivo `src/services/api.js`.

3. **Iniciar en modo desarrollo:**

```bash
npm run dev
```

La aplicación estará disponible en `http://localhost:5173`

## 📁 Estructura del Proyecto

```
src/
├── components/          # Componentes reutilizables
│   ├── Layout.jsx       # Layout principal con sidebar
│   └── ui.jsx           # Componentes UI (Button, Input, Modal, etc.)
├── pages/               # Páginas de la aplicación
│   ├── Login.jsx        # Inicio de sesión
│   ├── Register.jsx     # Registro de usuarios
│   ├── Products.jsx     # Listado de productos
│   ├── ProductForm.jsx  # Crear/Editar producto
│   ├── Categories.jsx   # Gestión de categorías
│   ├── Notifications.jsx # Centro de notificaciones
│   └── Reports.jsx      # Generación de reportes PDF
├── services/            # Servicios de API
│   ├── api.js           # Configuración de Axios
│   └── index.js         # Servicios (auth, products, etc.)
├── store/               # Estado global
│   └── authStore.js     # Store de autenticación
├── App.jsx              # Componente principal con rutas
├── main.jsx             # Entry point
└── index.css            # Estilos globales + Tailwind
```

## 🔐 Autenticación

El sistema utiliza JWT para autenticación:

- Los tokens se almacenan en localStorage mediante Zustand persist
- Se implementa refresh token automático al expirar el access token
- Las rutas protegidas redirigen a `/login` si no hay sesión

## 👥 Roles de Usuario

### Administrador
- Acceso completo al sistema
- CRUD de productos y categorías
- Generación de reportes PDF
- Ver y gestionar notificaciones

### Empleado
- Ver listado de productos
- Reportar productos con stock bajo
- Ver notificaciones

## 📱 Características

- ✅ Diseño responsivo (mobile-first)
- ✅ Búsqueda y filtrado de productos
- ✅ Paginación
- ✅ Notificaciones de stock bajo
- ✅ Descarga de reportes en PDF
- ✅ Validación de formularios
- ✅ Manejo de errores con toast notifications
- ✅ Refresh automático de tokens

## 🧪 Scripts Disponibles

```bash
npm run dev       # Inicia servidor de desarrollo
npm run build     # Genera build de producción
npm run preview   # Preview del build de producción
npm run lint      # Ejecuta ESLint
```

## 🔧 Configuración de Proxy

El servidor de desarrollo (Vite) está configurado con un proxy para evitar problemas de CORS:

```javascript
// vite.config.js
proxy: {
  '/api': {
    target: 'http://localhost:5000',
    changeOrigin: true
  }
}
```

## 📝 Variables de Entorno

Para producción, crea un archivo `.env`:

```env
VITE_API_URL=https://tu-api.com/api
```

Y modifica `src/services/api.js` para usar:

```javascript
baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api'
```

## 🐳 Docker

Puedes crear un Dockerfile para el frontend:

```dockerfile
FROM node:18-alpine as build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

## 📄 Licencia

Proyecto desarrollado para Touch Consulting.
