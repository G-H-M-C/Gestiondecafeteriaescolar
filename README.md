# Sistema de Gestión de Cafetería Escolar

API REST desarrollada con ASP.NET Core Web API (.NET 10) para gestionar el inventario, las ventas y el consumo de estudiantes de una cafetería escolar.

> Proyecto de la materia Programación II. Tarea 1: planeación y primera API (listas en memoria). La base de datos se agrega en la Tarea 2.

## Entidades
- **Producto**: Id, Nombre, Precio, Stock.
- **Estudiante**: Id, Nombre, Matricula, Grado.
- **Venta**: Id, EstudianteId, Fecha, Total.
- **DetalleVenta**: Id, VentaId, ProductoId, Cantidad, PrecioUnitario, Subtotal.

## Endpoints
Cada controlador ofrece GET (lista), GET por id, POST, PUT y DELETE:

| Ruta | Descripción |
|---|---|
| `/api/Productos` | Gestión de productos |
| `/api/Estudiantes` | Gestión de estudiantes |
| `/api/Ventas` | Gestión de ventas |
| `/api/DetallesVenta` | Gestión de detalles de venta |

Códigos HTTP usados: 200, 201, 204, 400 y 404.

## Reglas principales
- Los datos de entrada y salida usan DTOs (Create, Update y Response) con validaciones.
- La matrícula del estudiante es única.
- Una venta necesita un estudiante existente; un detalle necesita una venta y un producto existentes.
- El precio unitario, el subtotal y el total de la venta los calcula el servidor.
- Los datos se guardan en listas en memoria: se pierden al reiniciar la API.

## Cómo ejecutarlo
1. Instalar el SDK de .NET 10 y Visual Studio.
2. Abrir `GestionCafeteriaEscolar/GestionCafeteriaEscolar.slnx`.
3. Ejecutar el perfil **https**.
4. Abrir Swagger en `https://localhost:7171/swagger`.

## Flujo de trabajo en Git
- Rama `main`: versión estable.
- Rama `develop`: integración.
- Una rama `feature/...` por cada issue, integrada a `develop` mediante Pull Request.
- Seguimiento en el tablero (GitHub Project) con los issues #1 a #4.