import { useState } from 'react'
import { reportService } from '../services'
import { Button, Card, Input } from '../components/ui'
import { FileText, Download, AlertTriangle, Package } from 'lucide-react'
import toast from 'react-hot-toast'

export default function Reports() {
  const [loading, setLoading] = useState({ lowStock: false, inventory: false })
  const [threshold, setThreshold] = useState(5)

  const downloadPdf = (blob, filename) => {
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = filename
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
  }

  const handleLowStockReport = async () => {
    setLoading(l => ({ ...l, lowStock: true }))
    try {
      const response = await reportService.getLowStockPdf(threshold)
      const filename = `reporte-stock-bajo-${new Date().toISOString().slice(0, 10)}.pdf`
      downloadPdf(response.data, filename)
      toast.success('Reporte descargado')
    } catch (err) {
      toast.error(err.response?.data?.message || 'Error al generar reporte')
    } finally {
      setLoading(l => ({ ...l, lowStock: false }))
    }
  }

  const handleInventoryReport = async () => {
    setLoading(l => ({ ...l, inventory: true }))
    try {
      const response = await reportService.getInventoryPdf()
      const filename = `reporte-inventario-${new Date().toISOString().slice(0, 10)}.pdf`
      downloadPdf(response.data, filename)
      toast.success('Reporte descargado')
    } catch (err) {
      toast.error(err.response?.data?.message || 'Error al generar reporte')
    } finally {
      setLoading(l => ({ ...l, inventory: false }))
    }
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Reportes</h1>

      <div className="grid gap-6 md:grid-cols-2">
        <Card>
          <div className="flex items-start gap-4">
            <div className="w-12 h-12 bg-yellow-100 rounded-lg flex items-center justify-center">
              <AlertTriangle className="text-yellow-600" size={24} />
            </div>
            <div className="flex-1">
              <h2 className="text-lg font-semibold mb-2">Reporte de Stock Bajo</h2>
              <p className="text-gray-600 text-sm mb-4">
                Genera un PDF con todos los productos que tienen un inventario por debajo del umbral especificado.
              </p>
              
              <div className="flex items-end gap-4">
                <div className="flex-1">
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Umbral de stock
                  </label>
                  <input
                    type="number"
                    min="1"
                    value={threshold}
                    onChange={e => setThreshold(parseInt(e.target.value) || 5)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-200"
                  />
                </div>
                <Button 
                  onClick={handleLowStockReport} 
                  loading={loading.lowStock}
                  className="whitespace-nowrap"
                >
                  <Download size={20} /> Descargar PDF
                </Button>
              </div>
            </div>
          </div>
        </Card>

        <Card>
          <div className="flex items-start gap-4">
            <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
              <Package className="text-blue-600" size={24} />
            </div>
            <div className="flex-1">
              <h2 className="text-lg font-semibold mb-2">Reporte de Inventario Completo</h2>
              <p className="text-gray-600 text-sm mb-4">
                Genera un PDF con el listado completo de todos los productos en el inventario, incluyendo precios y cantidades.
              </p>
              
              <Button 
                onClick={handleInventoryReport} 
                loading={loading.inventory}
              >
                <Download size={20} /> Descargar PDF
              </Button>
            </div>
          </div>
        </Card>
      </div>

      <Card className="bg-blue-50 border-blue-200">
        <div className="flex items-start gap-3">
          <FileText className="text-blue-600 flex-shrink-0 mt-0.5" size={20} />
          <div>
            <h3 className="font-medium text-blue-900">Información sobre reportes</h3>
            <p className="text-sm text-blue-700 mt-1">
              Los reportes se generan en formato PDF y se descargan automáticamente. 
              Incluyen la fecha de generación y el logotipo de la empresa. 
              Para productos con stock bajo, se destacan visualmente para facilitar su identificación.
            </p>
          </div>
        </div>
      </Card>
    </div>
  )
}
