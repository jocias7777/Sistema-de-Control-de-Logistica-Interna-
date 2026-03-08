# ?? Sistema de Control de Logística Interna

Sistema web para la gestión y control de inventarios, solicitudes de despacho y operaciones logísticas internas de una empresa.

## ?? Características

### ?? Dashboard Interactivo
- KPIs en tiempo real (productos bajo mínimo, solicitudes pendientes, despachos del día)
- Gráficos dinámicos con Chart.js
- Actualización automática cada 60 segundos
- Estadísticas mensuales de solicitudes y rotación de productos

### ?? Gestión de Productos
- CRUD completo de productos
- Control de stock en múltiples bodegas
- Alertas de stock mínimo
- Búsqueda y filtrado avanzado
- Historial de movimientos (Kardex)

### ?? Sistema de Solicitudes
- Creación y gestión de solicitudes de despacho
- Flujo de aprobación (Borrador ? Pendiente ? Aprobada ? Despachada)
- Múltiples líneas por solicitud
- Asignación por bodega
- Historial completo de cambios de estado

### ?? Kardex
- Registro detallado de movimientos de inventario
- Tipos: Entrada, Salida, Ajuste, Traslado
- Consulta por producto, bodega y rango de fechas
- Trazabilidad completa de operaciones

### ?? Seguridad
- Autenticación JWT
- Control de acceso basado en roles (Admin, Aprobador, Bodeguero, Usuario)
- Contraseñas encriptadas con BCrypt
- Middleware de manejo de excepciones

## ??? Tecnologías

### Backend
- **.NET 8** - Framework principal
- **ASP.NET Core Web API** - REST API
- **SQL Server** - Base de datos
- **Dapper** - Micro ORM
- **JWT Bearer** - Autenticación
- **BCrypt.Net** - Encriptación de contraseñas
- **Swagger** - Documentación de API

### Frontend
- **HTML5 / CSS3** - Estructura y estilos
- **JavaScript (Vanilla)** - Lógica del cliente
- **Chart.js** - Gráficos interactivos
- **Bootstrap Icons** - Iconografía
- **Fetch API** - Comunicación con backend

## ?? Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server/sql-server-downloads) o SQL Server Express
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [Visual Studio Code](https://code.visualstudio.com/)
- [SQL Server Management Studio (SSMS)](https://aka.ms/ssmsfullsetup) (opcional)

## ?? Instalación

### 1. Clonar el repositorio
```bash
git clone https://github.com/jocias7777/Sistema-de-Control-de-Logistica-Interna-.git
cd Sistema-de-Control-de-Logistica-Interna-
```

### 2. Configurar la base de datos

#### Opción A: Crear la base de datos manualmente
Ejecuta el siguiente script SQL en tu servidor:

```sql
CREATE DATABASE DespachoLogisticaDB;
GO

USE DespachoLogisticaDB;
GO

-- Crear las tablas necesarias según tu esquema
-- (Aquí irían los scripts de creación de tablas)
```

#### Opción B: Usar el script de inicialización
Si tienes un script SQL de inicialización, ejecútalo:
```bash
sqlcmd -S NOMBRE_SERVIDOR -d DespachoLogisticaDB -i script.sql
```

### 3. Configurar la cadena de conexión

Edita el archivo `DespachoLogistica.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=TU_SERVIDOR;Database=DespachoLogisticaDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "TU_CLAVE_SECRETA_MINIMO_32_CARACTERES",
    "Issuer": "DespachoAPI",
    "ExpiresHours": 8
  }
}
```

### 4. Restaurar paquetes NuGet
```bash
cd DespachoLogistica.API
dotnet restore
```

### 5. Ejecutar el proyecto
```bash
dotnet run
```

O desde Visual Studio:
- Presiona **Ctrl + Shift + B** para compilar
- Presiona **F5** para ejecutar en modo debug

### 6. Acceder a la aplicación

Abre tu navegador en:
```
https://localhost:7185/index.html
```

## ?? Estructura del Proyecto

```
DespachoLogistica/
??? DespachoLogistica.API/
?   ??? Controllers/           # Controladores de la API
?   ?   ??? AuthController.cs
?   ?   ??? DashboardController.cs
?   ?   ??? ProductosController.cs
?   ?   ??? SolicitudesController.cs
?   ?   ??? BodegasController.cs
?   ??? Services/              # Lógica de negocio
?   ?   ??? DashboardService.cs
?   ?   ??? ProductoService.cs
?   ?   ??? SolicitudService.cs
?   ??? Models/                # Modelos y DTOs
?   ?   ??? DTOs/
?   ?   ??? Common/
?   ??? Data/                  # Contexto de base de datos
?   ?   ??? DatabaseContext.cs
?   ??? Helpers/               # Utilidades
?   ?   ??? JwtHelper.cs
?   ??? Middleware/            # Middleware personalizado
?   ?   ??? ExceptionMiddleware.cs
?   ??? wwwroot/               # Archivos estáticos
?   ?   ??? css/
?   ?   ?   ??? app.css
?   ?   ??? js/
?   ?   ?   ??? utils.js
?   ?   ??? dashboard.html
?   ?   ??? productos.html
?   ?   ??? solicitudes.html
?   ?   ??? kardex.html
?   ?   ??? index.html
?   ??? appsettings.json
?   ??? Program.cs
??? DespachoLogistica.sln
```

## ?? Usuarios por Defecto

El sistema incluye usuarios de prueba con diferentes roles:

| Usuario | Contraseña | Rol |
|---------|------------|-----|
| admin | admin123 | Administrador |
| aprobador | aprobador123 | Aprobador |
| bodeguero | bodeguero123 | Bodeguero |
| usuario | usuario123 | Usuario |

> ?? **Importante**: Cambiar estas credenciales en producción

## ?? Roles y Permisos

- **Administrador**: Acceso total al sistema
- **Aprobador**: Puede aprobar/rechazar solicitudes
- **Bodeguero**: Gestiona inventarios y despachos
- **Usuario**: Crea solicitudes y consulta información

## ?? API Endpoints

### Autenticación
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/register` - Registrar usuario

### Dashboard
- `GET /api/dashboard/kpis` - Obtener KPIs principales
- `GET /api/dashboard/solicitudes-por-estado` - Estadísticas de solicitudes
- `GET /api/dashboard/rotacion` - Top productos más despachados

### Productos
- `GET /api/productos` - Listar productos
- `GET /api/productos/{id}` - Obtener producto
- `POST /api/productos` - Crear producto
- `PUT /api/productos/{id}` - Actualizar producto
- `DELETE /api/productos/{id}` - Eliminar producto

### Solicitudes
- `GET /api/solicitudes` - Listar solicitudes
- `GET /api/solicitudes/{id}` - Obtener solicitud
- `POST /api/solicitudes` - Crear solicitud
- `PUT /api/solicitudes/{id}/estado` - Cambiar estado
- `POST /api/solicitudes/{id}/despachar` - Despachar solicitud

### Bodegas
- `GET /api/bodegas` - Listar bodegas

## ?? Configuración Adicional

### Configurar CORS (si es necesario)
En `Program.cs`, ajusta las políticas de CORS según tus necesidades:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### Variables de Entorno
Para producción, usa variables de entorno en lugar de `appsettings.json`:

```bash
export ConnectionStrings__Default="Server=...;Database=...;"
export Jwt__Key="TuClaveSecreta"
```

## ?? Debugging

### Ver logs en consola
Los logs se muestran en la consola de Visual Studio o en el terminal.

### Swagger UI
Accede a la documentación interactiva de la API:
```
https://localhost:7185/swagger
```

## ?? Despliegue

### IIS
1. Publicar el proyecto: `dotnet publish -c Release`
2. Copiar la carpeta `publish` al servidor IIS
3. Configurar un Application Pool con .NET CLR Version: "No Managed Code"
4. Configurar las variables de entorno

### Docker (opcional)
Crear un `Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DespachoLogistica.API/DespachoLogistica.API.csproj", "DespachoLogistica.API/"]
RUN dotnet restore "DespachoLogistica.API/DespachoLogistica.API.csproj"
COPY . .
WORKDIR "/src/DespachoLogistica.API"
RUN dotnet build "DespachoLogistica.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DespachoLogistica.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DespachoLogistica.API.dll"]
```

## ?? Contribuir

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/NuevaCaracteristica`)
3. Commit tus cambios (`git commit -m 'Agregar nueva característica'`)
4. Push a la rama (`git push origin feature/NuevaCaracteristica`)
5. Abre un Pull Request

## ?? Licencia

Este proyecto es de código abierto y está disponible bajo la [Licencia MIT](LICENSE).

## ????? Autor

**Jocias Avila**
- GitHub: [@jocias7777](https://github.com/jocias7777)

## ?? Soporte

Si encuentras algún problema o tienes sugerencias:
- Abre un [Issue](https://github.com/jocias7777/Sistema-de-Control-de-Logistica-Interna-/issues)
- Contacta al equipo de desarrollo

## ?? Roadmap

- [ ] Reportes en PDF/Excel
- [ ] Notificaciones por email
- [ ] App móvil con Xamarin/MAUI
- [ ] Integración con código de barras
- [ ] Dashboard de BI con Power BI
- [ ] API de integración con otros sistemas

---

? Si este proyecto te fue útil, ¡dale una estrella en GitHub!
