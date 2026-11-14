# Sistema de Gestión de Cafetería Escolar

API REST para registrar productos, estudiantes y ventas diarias de una cafetería escolar. Los datos de inventario, precios y consumo se guardan en una base de datos SQL Server mediante Entity Framework Core.

## Tecnologías
- .NET 10 (ASP.NET Core Web API con Controllers)
- Entity Framework Core 10 (Code First) con SQL Server
- Swagger / OpenAPI para probar los endpoints

## Arquitectura en capas
La solución está dividida en cuatro proyectos:

```
GestionCafeteriaEscolar/
├── GestionCafeteriaEscolar.API              # Controllers, Program.cs
├── GestionCafeteriaEscolar.Application      # Core (ServiceResult), Dtos y Services
├── GestionCafeteriaEscolar.Domain           # Core (BaseEntity) y Entities
└── GestionCafeteriaEscolar.Infrastructure   # Context, Migrations, Core (GenericRepository) y Repositories
```

- **Domain**: contiene `Core/BaseEntity.cs` y las entidades (`Producto`, `Estudiante`, `Venta`, `DetalleVenta`). No depende de Entity Framework ni de ASP.NET.
- **Infrastructure**: contiene el `ApplicationDbContext`, las migraciones, el `GenericRepository<T>` y los repositorios propios `EstudianteRepository` y `DetalleVentaRepository`.
- **Application**: contiene los DTOs (organizados por recurso), `ServiceResult<T>` (resultado genérico de los servicios) y los servicios de negocio `ProductoService`, `EstudianteService`, `VentaService` y `DetalleVentaService`, con sus validaciones.
- **API**: contiene los controladores, que solo reciben la petición, llaman al servicio y devuelven el código HTTP correspondiente.

Flujo de una petición: Controller → Service → Repository → DbContext → SQL Server.

Referencias entre proyectos: la API referencia a Application; Application referencia a Domain e Infrastructure; Infrastructure referencia a Domain.

## Entidades
- **Producto**: Id, Nombre, Precio, Stock
- **Estudiante**: Id, Nombre, Matrícula, Grado
- **Venta**: Id, EstudianteId, Fecha, Total
- **DetalleVenta**: Id, VentaId, ProductoId, Cantidad, PrecioUnitario, Subtotal

Todas heredan de `BaseEntity` (propiedad `Id`).

Relaciones: un Estudiante tiene muchas Ventas, una Venta tiene muchos DetalleVenta y un Producto aparece en muchos DetalleVenta.

## Endpoints
Cada recurso tiene los mismos cinco endpoints:

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | /api/{recurso} | Lista todos los registros |
| GET | /api/{recurso}/{id} | Obtiene un registro por Id |
| POST | /api/{recurso} | Crea un registro |
| PUT | /api/{recurso}/{id} | Actualiza un registro |
| DELETE | /api/{recurso}/{id} | Elimina un registro |

Recursos: `Productos`, `Estudiantes`, `Ventas` y `DetallesVenta`.

## Reglas de negocio
- La matrícula del estudiante debe ser única.
- Una venta solo se crea si el estudiante existe.
- Un detalle solo se crea si la venta y el producto existen.
- El precio unitario se toma del producto y el subtotal es precio por cantidad.
- El total de la venta se recalcula al crear, actualizar o eliminar sus detalles.
- Al eliminar una venta se eliminan sus detalles.
- No se puede eliminar un producto o un estudiante que ya tiene ventas.
- Los datos de entrada se reciben con DTOs, que se validan con Data Annotations y de nuevo en los servicios. Respuestas: 200, 201, 204, 400 y 404.

## Base de datos
- `ApplicationDbContext` está en `GestionCafeteriaEscolar.Infrastructure/Context`.
- La cadena de conexión `DefaultConnection` está en `appsettings.json` de la API y usa SQL Server LocalDB con autenticación de Windows (no incluye contraseñas).
- Las migraciones están en `GestionCafeteriaEscolar.Infrastructure/Migrations`.
- Se incluyen datos iniciales de ejemplo (2 productos y 2 estudiantes).

## Cómo ejecutarlo
1. Clonar el repositorio y abrir `GestionCafeteriaEscolar.slnx` en Visual Studio.
2. Instalar la herramienta de EF Core si no la tienes: `dotnet tool install --global dotnet-ef`
3. Crear la base de datos desde la carpeta `GestionCafeteriaEscolar` (la que contiene el `.slnx`): `dotnet ef database update --project GestionCafeteriaEscolar.Infrastructure --startup-project GestionCafeteriaEscolar.API`
4. Ejecutar el proyecto con el perfil https y abrir `https://localhost:7171/swagger`.

Si usas otra instancia de SQL Server, cambia el valor de `DefaultConnection` en `appsettings.json`.

## Flujo de trabajo con Git
- Cada tarea parte de un issue.
- Se trabaja en una rama `feature/N-nombre` creada desde la rama de la tarea (`tarea-4`).
- Se abre un Pull Request hacia la rama de la tarea.
- Cada tarea se entrega en su propia rama: `tarea-3` y `tarea-4`.