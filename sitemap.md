# Sitemap i usmjeravanje

## Konvencionalni routing (default)

Definiran u Program.cs:
- pattern: {controller=Home}/{action=Index}/{id?}

## Custom routing (atributni) - zahtjev vjezbe

1. GET /uredaji
- Controller: DeviceController
- Akcija: Index
- View: Views/Device/Index.cshtml

2. GET /uredaji/detalji/{id}
- Controller: DeviceController
- Akcija: Details(int id)
- View: Views/Device/Details.cshtml

3. GET /laboratoriji
- Controller: LaboratoryController
- Akcija: Index
- View: Views/Laboratory/Index.cshtml

4. GET /laboratoriji/detalji/{id}
- Controller: LaboratoryController
- Akcija: Details(int id)
- View: Views/Laboratory/Details.cshtml

## Ostali dostupni URL-ovi (konvencionalni)

1. GET /
- Controller: HomeController
- Akcija: Index
- View: Views/Home/Index.cshtml

2. GET /Home/Privacy
- Controller: HomeController
- Akcija: Privacy
- View: Views/Home/Privacy.cshtml

3. GET /Calibration
- Controller: CalibrationController
- Akcija: Index
- View: Views/Calibration/Index.cshtml

4. GET /Calibration/Details/{id}
- Controller: CalibrationController
- Akcija: Details(int id)
- View: Views/Calibration/Details.cshtml

5. GET /Technician
- Controller: TechnicianController
- Akcija: Index
- View: Views/Technician/Index.cshtml

6. GET /Technician/Details/{id}
- Controller: TechnicianController
- Akcija: Details(int id)
- View: Views/Technician/Details.cshtml

7. GET /DeviceLocation
- Controller: DeviceLocationController
- Akcija: Index
- View: Views/DeviceLocation/Index.cshtml

8. GET /DeviceLocation/Details/{id}
- Controller: DeviceLocationController
- Akcija: Details(int id)
- View: Views/DeviceLocation/Details.cshtml

9. GET /Home/Error
- Controller: HomeController
- Akcija: Error
- View: Views/Shared/Error.cshtml
