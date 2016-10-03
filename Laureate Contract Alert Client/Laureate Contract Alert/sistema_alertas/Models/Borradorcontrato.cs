//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace sistema_alertas.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Borradorcontrato
    {
        public int ContratoId2 { get; set; }
        public string Nombre_Contrato2 { get; set; }
        public string Numero_Contrato2 { get; set; }
        public string Numero_Servicio_Contrato2 { get; set; }
        public System.DateTime Fecha_Inicio2 { get; set; }
        public System.DateTime Fecha_Finalizacion2 { get; set; }
        public Nullable<decimal> Costo_Total2 { get; set; }
        public Nullable<decimal> Costo_Por_Cada_Pago2 { get; set; }
        public System.DateTime Fecha_Renovacion_Pago2 { get; set; }
        public Nullable<int> Cantidad_Licencias2 { get; set; }
        public string Garantia2 { get; set; }
        public System.DateTime Fecha_Vencimiento_Garantia2 { get; set; }
        public Nullable<System.DateTime> Fecha_Creacion2 { get; set; }
        public string Descripcion2 { get; set; }
        public string Direccion_Archivo2 { get; set; }
        public Nullable<int> Proveedor_ManufactureroId2 { get; set; }
        public Nullable<int> Proveedor_DistribuidorId2 { get; set; }
        public Nullable<int> Usuario_AprobadorId2 { get; set; }
        public Nullable<int> Usuario_AdministradorId2 { get; set; }
        public Nullable<int> Tipo_ContratoId2 { get; set; }
        public Nullable<int> Metodo_PagoId2 { get; set; }
        public Nullable<int> Unidad_NegocioId2 { get; set; }
        public Nullable<int> Tipo_PagoId2 { get; set; }
        public Nullable<int> Orden_CompraId2 { get; set; }
        public Nullable<int> TarjetaId2 { get; set; }
        public Nullable<int> Area_ContratoId2 { get; set; }
        public Nullable<int> ProductoId2 { get; set; }
        public Nullable<int> RecordatorioId2 { get; set; }
        public Nullable<int> EstadoId2 { get; set; }
        public string Detalles_Producto2 { get; set; }
        public string Gastos2 { get; set; }
    
        public virtual area_contratos area_contratos { get; set; }
        public virtual estado estado { get; set; }
        public virtual metodo_pagos metodo_pagos { get; set; }
        public virtual orden_compras orden_compras { get; set; }
        public virtual producto producto { get; set; }
        public virtual proveedore proveedore { get; set; }
        public virtual proveedore proveedore1 { get; set; }
        public virtual recordatorio recordatorio { get; set; }
        public virtual tarjeta tarjeta { get; set; }
        public virtual tipo_contratos tipo_contratos { get; set; }
        public virtual tipo_pagos tipo_pagos { get; set; }
        public virtual unidad_negocios unidad_negocios { get; set; }
        public virtual usuario usuario { get; set; }
        public virtual usuario usuario1 { get; set; }
    }
}
