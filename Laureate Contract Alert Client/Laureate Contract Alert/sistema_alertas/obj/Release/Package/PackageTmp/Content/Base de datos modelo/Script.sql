
--
-- Base de datos: `sistema_alertas`
--


-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `area_contratos`
--

use sistema_alertas;

CREATE TABLE area_contratos (
  Area_ContratoId int NOT NULL IDENTITY(1,1),
  Nombre_Area varchar(25) NOT NULL,

  CONSTRAINT PK_Area_ContratosId PRIMARY KEY(Area_ContratoId),
);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE contratos (
  ContratoId int NOT NULL IDENTITY(1,1),
  Nombre_Contrato varchar(25) NOT NULL,
  Numero_Contrato varchar(15) NOT NULL,
  Numero_Servicio_Contrato varchar(15) NOT NULL,
  Fecha_Inicio varchar(10) NOT NULL,
  Fecha_Finalizacion varchar(10) NOT NULL,
  Costo_Total decimal(5,0) NOT NULL,
  Costo_Por_Cada_Pago decimal(5,0) NOT NULL,
  Fecha_Renovacion_Pago varchar(10) NOT NULL,
  Cantidad_Licencias int NOT NULL,
  Garantia varchar(25) NOT NULL,
  Fecha_Vencimiento_Garantia varchar(10) NOT NULL,
  Fecha_Creacion varchar(10) NOT NULL,
  Descripcion varchar(100) DEFAULT NULL,
  Direccion_Archivo varchar(100) DEFAULT NULL,
  Proveedor_ManufactureroId int NOT NULL,
  Proveedor_DistribuidorId int NOT NULL,
  Usuario_AprobadorId int NOT NULL,
  Usuario_AdministradorId int NOT NULL,
  Tipo_ContratoId int NOT NULL,
  Metodo_PagoId int NOT NULL,
  Unidad_NegocioId int NOT NULL,
  Tipo_PagoId int NOT NULL,
  Orden_CompraId int NOT NULL,
  TarjetaId int DEFAULT NULL,
  Area_ContratoId int NOT NULL,
  ProductoId int NOT NULL,
  RecordatorioId int NOT NULL,
  EstadoId int NOT NULL,

  CONSTRAINT PK_ContratoId PRIMARY KEY(ContratoId),
  CONSTRAINT UQ_Numero_Contrato UNIQUE(Numero_Contrato)

);

-- --------------------------------------------------------
--
-- Estructura de tabla para la tabla `estados`
--

CREATE TABLE estados (
  EstadoId int NOT NULL IDENTITY(1,1),
  Descripcion varchar(25) NOT NULL,

  CONSTRAINT PK_EstadosId PRIMARY KEY(EstadoId)
);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `metodo_pagos`
--

CREATE TABLE metodo_pagos (
  Metodo_PagoId int NOT NULL IDENTITY(1,1),
  Nombre_Metodo_Pago varchar(25) NOT NULL,

  CONSTRAINT PK_Metodo_PagoId PRIMARY KEY(Metodo_PagoId)
);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `orden_compras`
--

CREATE TABLE orden_compras (
  Orden_CompraId int NOT NULL IDENTITY(1,1) ,
  Fecha_Ultima_Orden_Compra varchar(10) NOT NULL,
  Ultimo_Numero_Factura varchar(15) NOT NULL,
  Codigo_Presupuestario2016 varchar(15) NOT NULL,
  Monto_Previo varchar(25) NOT NULL,
  Direccion_Archivo varchar(100) DEFAULT NULL,

  CONSTRAINT PK_Orden_CompraId PRIMARY KEY(Orden_CompraId)
) ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `perfiles`
--

CREATE TABLE perfiles (
  PerfilId int NOT NULL IDENTITY(1,1),
  Nombre_Perfil varchar(25) NOT NULL,
  Descripcion varchar(100) DEFAULT NULL,

  CONSTRAINT PK_PerfilId PRIMARY KEY(PerfilId)
) ;

--
-- Volcado de datos para la tabla `perfiles`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `permisos`
--

CREATE TABLE permisos (
  PermisoId int NOT NULL IDENTITY(1,1),
  Nombre_Permiso varchar(25) NOT NULL,
  Descripcion varchar(100) DEFAULT NULL,

  CONSTRAINT PK_PermisoId PRIMARY KEY(PermisoId)
) ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `productos`
--

CREATE TABLE productos (
  ProductoId int NOT NULL IDENTITY(1,1),
  Nombre_Producto varchar(25) NOT NULL,
  Precio decimal(5,5) NOT NULL,
  Descripcion varchar(100) DEFAULT NULL,
  Proveedor_ManufactureroId int NOT NULL,
  Proveedor_DistribuidorId int NOT NULL,

  CONSTRAINT PK_ProductoId PRIMARY KEY(ProductoId )
)  ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `proveedores`
--

CREATE TABLE proveedores (
  ProveedorId int NOT NULL IDENTITY(1,1),
  Nombre_Proveedor varchar(25) NOT NULL,
  Correo_Proveedor varchar(50) NOT NULL,
  Nombre_Contacto varchar(25) DEFAULT NULL,
  Telefono_Contacto varchar(10) DEFAULT NULL,
  Ubicacion varchar(50) DEFAULT NULL,
  Descripcion varchar(100) DEFAULT NULL,
  Tipo_ProveedorId int NOT NULL,

  CONSTRAINT PK_ProveedorId PRIMARY KEY(ProveedorId)
);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `recordatorios`
--

CREATE TABLE recordatorios (
  RecordatorioId int NOT NULL IDENTITY(1,1),
  Descripcion varchar(25) NOT NULL,

  CONSTRAINT PK_RecordatorioId PRIMARY KEY(RecordatorioId)
) ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tarjetas`
--

CREATE TABLE tarjetas(
  TarjetaId int NOT NULL IDENTITY(1,1),
  Ultimos_Digitos_Tarjeta varchar(4) NOT NULL,
  Fecha_Vencimiento_Tarjeta varchar(10) NOT NULL,
  Nombre_Persona_Asignada_A_Tarjeta varchar(25) NOT NULL,
  Tipo_TarjetaId int NOT NULL,

  CONSTRAINT PK_TarjetaId PRIMARY KEY(TarjetaId)

) ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipo_contratos`
--

CREATE TABLE tipo_contratos (
  Tipo_ContratoId int NOT NULL IDENTITY(1,1),
  Nombre_Tipo_Contrato varchar(25) NOT NULL,
  Descripcion varchar(100) DEFAULT NULL,

  CONSTRAINT PK_Tipo_ContratoId PRIMARY KEY(Tipo_ContratoId)
);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipo_pagos`
--

CREATE TABLE tipo_pagos (
  Tipo_PagoId int NOT NULL IDENTITY(1,1),
  Nombre_Tipo_Pago varchar(25) NOT NULL,

  CONSTRAINT PK_Tipo_PagoId PRIMARY KEY(Tipo_PagoId)
);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipo_proveedores`
--

CREATE TABLE tipo_proveedores (
  Tipo_ProveedorId int NOT NULL IDENTITY(1,1),
  Descripcion varchar(25) NOT NULL,

  CONSTRAINT PK_Tipo_ProveedorId PRIMARY KEY(Tipo_ProveedorId)
) ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipo_tarjetas`
--

CREATE TABLE tipo_tarjetas (
  Tipo_TarjetaId int NOT NULL IDENTITY(1,1),
  Nombre_Tarjeta varchar(25) NOT NULL,
  Descripcion varchar(100) DEFAULT NULL,

  CONSTRAINT PK_Tipo_TarjetaId PRIMARY KEY(Tipo_TarjetaId)
) ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `unidad_negocios`
--

CREATE TABLE unidad_negocios (
  Unidad_NegocioId int NOT NULL IDENTITY(1,1),
  Numero_Unidad_Negocio varchar(15) NOT NULL,
  Nombre_Unidad_Negocio varchar(15) NOT NULL,
  Nombre_Previo_Unidad_Negocio varchar(15) NOT NULL,

  CONSTRAINT PK_Unidad_NegocioId PRIMARY KEY(Unidad_NegocioId)
) ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE usuarios (
  UsuarioId int NOT NULL IDENTITY(1,1),
  Nombre varchar(25) NOT NULL,
  Apellido varchar(25) NOT NULL,
  email varchar(50) NOT NULL,
  pass varchar(30) NOT NULL,
  Telefono varchar(10) NOT NULL,
  Codigo_Usuario varchar(15) NOT NULL,
  username varchar(30) NOT NULL,
  Fecha_Creacion varchar(10) NOT NULL,
  Direccion_Fotografia varchar(100) DEFAULT NULL,
  PerfilId int NOT NULL,

  CONSTRAINT PK_UsuarioId PRIMARY KEY(UsuarioId),
  CONSTRAINT UQ_Codigo_Usuario UNIQUE(Codigo_Usuario)

) ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios_perfiles_permisos`
--

CREATE TABLE usuarios_perfiles_permisos (
  Usuario_Perfil_PermisoId int NOT NULL IDENTITY(1,1),
  UsuarioId int NOT NULL,
  PerfilId int NOT NULL,
  PermisoId int NOT NULL,
  Descripcion varchar(100) DEFAULT NULL,

  CONSTRAINT PK_Usuario_Perfil_PermisosId PRIMARY KEY(Usuario_Perfil_PermisoId)
);

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE contratos
  ADD CONSTRAINT FK_Proveedores_Manufacturero_PK_ProveedorId FOREIGN KEY (Proveedor_ManufactureroId) REFERENCES proveedores (ProveedorId) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE contratos
  ADD CONSTRAINT FK_Proveedores_Distribuidor_PK_ProveedorId FOREIGN KEY (Proveedor_DistribuidorId) REFERENCES proveedores (ProveedorId) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE contratos
  ADD CONSTRAINT FK_Tipo_Contratos_PK_Tipo_ContratoId FOREIGN KEY (Tipo_ContratoId) REFERENCES tipo_contratos (Tipo_ContratoId) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE contratos
  ADD CONSTRAINT FK_Usuarios_Aprobador_PK_UsuarioId FOREIGN KEY (Usuario_AprobadorId) REFERENCES usuarios (UsuarioId) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE contratos
  ADD CONSTRAINT FK_Usuarios_Administrador_UsuarioId FOREIGN KEY (Usuario_AdministradorId) REFERENCES usuarios (UsuarioId) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE contratos
  ADD CONSTRAINT FK_Metodo_Pagos_PK_Metodo_PagoId FOREIGN KEY (Metodo_PagoId) REFERENCES metodo_pagos (Metodo_PagoId) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE contratos
  ADD CONSTRAINT FK_Unidad_Negocios_PK_Unidad_NegocioId FOREIGN KEY (Unidad_NegocioId) REFERENCES unidad_negocios (Unidad_NegocioId) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE contratos
  ADD CONSTRAINT FK_Tipo_Pagos_PK_Tipo_PagoId FOREIGN KEY (Tipo_PagoId) REFERENCES tipo_pagos (Tipo_PagoId) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE contratos
  ADD CONSTRAINT FK_Orden_Compras_PK_Orden_CompraId FOREIGN KEY (Orden_CompraId) REFERENCES orden_compras (Orden_CompraId) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE contratos
  ADD CONSTRAINT FK_Tarjetas_Pk_TarjetaId FOREIGN KEY (TarjetaId) REFERENCES tarjetas (TarjetaId) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE contratos
  ADD CONSTRAINT FK_Area_Contratos_PK_Area_Contrato FOREIGN KEY (Area_ContratoId) REFERENCES area_contratos (Area_ContratoId) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE contratos
  ADD CONSTRAINT FK_Productos_ProductoId FOREIGN KEY (ProductoId) REFERENCES productos (ProductoId) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE contratos
  ADD CONSTRAINT FK_Recordatorios_RecordatorioId FOREIGN KEY (RecordatorioId) REFERENCES recordatorios (RecordatorioId) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE contratos
  ADD CONSTRAINT FK_Estados_EstadoId FOREIGN KEY (EstadoId) REFERENCES estados (EstadoId) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `productos`
--
ALTER TABLE productos
  ADD CONSTRAINT FK_Proveedor_Manufactureros_PK_ProductoId FOREIGN KEY (Proveedor_ManufactureroId) REFERENCES proveedores (ProveedorId) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE productos
  ADD CONSTRAINT FK_Proveedor_Distribuidores_PK_ProductoId FOREIGN KEY (Proveedor_DistribuidorId) REFERENCES proveedores (ProveedorId) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Filtros para la tabla `proveedores`
--
ALTER TABLE proveedores
  ADD CONSTRAINT FK_Tipo_Proveedores_PK_ProveedorId FOREIGN KEY (Tipo_ProveedorId) REFERENCES tipo_proveedores (Tipo_ProveedorId) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `tarjetas`
--
ALTER TABLE tarjetas
  ADD CONSTRAINT FK_Tipo_Tarjetas_Tipo_TarjetaId FOREIGN KEY (Tipo_TarjetaId) REFERENCES tipo_tarjetas (Tipo_TarjetaId) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `usuarios`
--
ALTER TABLE usuarios
  ADD CONSTRAINT FK_Perfiles_PK_Perfil FOREIGN KEY (PerfilId) REFERENCES perfiles (PerfilId) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `usuarios_perfiles_permisos`
--
ALTER TABLE usuarios_perfiles_permisos
  ADD CONSTRAINT FK_Usuarios_PK_UsuarioId FOREIGN KEY (UsuarioId) REFERENCES usuarios (UsuarioId) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE usuarios_perfiles_permisos
  ADD CONSTRAINT FK_Perfiles_PK_PerfilId FOREIGN KEY (PerfilId) REFERENCES perfiles (PerfilId) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE usuarios_perfiles_permisos
  ADD CONSTRAINT FK_Permisos_PK_PermisoId FOREIGN KEY (PermisoId) REFERENCES permisos (PermisoId) ON DELETE CASCADE ON UPDATE CASCADE;
