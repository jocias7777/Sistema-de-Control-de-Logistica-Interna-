# Sistema de Control de Logística Interna

Sistema web desarrollado para la gestión y control de procesos logísticos dentro de una organización.
Permite administrar despachos, seguimiento de operaciones y control de información logística de forma centralizada.

##  Tecnologías utilizadas

* ASP.NET Core (.NET 8)
* Entity Framework core 
* SQL Server
* C#
* Arquitectura en capas (Clean Architecture)
* REST API

##  Estructura del proyecto

El sistema está organizado siguiendo buenas prácticas de arquitectura de software:

* **DespachoLogistica.API**
  Contiene los controladores y endpoints de la API.

* **DespachoLogistica.Application**
  Lógica de negocio y servicios de la aplicación.

* **DespachoLogistica.Domain**
  Entidades y reglas del dominio.

* **DespachoLogistica.Infrastructure**
  Acceso a datos y configuración de la base de datos.

##  Funcionalidades principales

* Gestión de despachos
* Registro de operaciones logísticas
* Administración de datos del sistema
* API para integración con otros sistemas
* Arquitectura escalable para futuras funcionalidades

##  Instalación y ejecución

1. Clonar el repositorio

git clone https://github.com/jocias7777/Sistema-de-Control-de-Logistica-Interna-.git

2. Abrir la solución en Visual Studio

3. Configurar la cadena de conexión a SQL Server en:

appsettings.json

4. Ejecutar el proyecto desde Visual Studio

##  Autor

Desarrollado por **Jocias**

##  Licencia

Proyecto de uso educativo y demostrativo.
