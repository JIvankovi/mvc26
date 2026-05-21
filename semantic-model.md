# Semanticki DB model

## Pregled entiteta/tablica

1. Device
- Svrha: osnovni zapis o mjernom uredaju.
- Kljucna svojstva: Id, Name, Manufacturer, SerialNumber, PurchaseDate, MeasurementType.
- Veze:
  - 1:N prema Calibration (Device.CalibrationHistory)
  - 1:N prema DeviceLocation (Device.LocationHistory)

2. Calibration
- Svrha: zapis o kalibraciji uredaja.
- Kljucna svojstva: Id, CalibrationDateTime, CalibrationStandard, MeasuredDeviation, PassedCalibration, NextCalibrationDue, Notes.
- Vanjski kljucevi: DeviceId (opcionalno), TechnicianId (opcionalno).
- Veze:
  - N:1 prema Device
  - N:1 prema Technician

3. Technician
- Svrha: osoba koja provodi kalibracije.
- Kljucna svojstva: Id, Name, Email, PhoneNumber, Certification, YearsOfExperience.
- Veze:
  - 1:N prema Calibration (Technician.Calibrations)

4. Laboratory
- Svrha: lokacija/laboratorij gdje je uredaj rasporeden.
- Kljucna svojstva: Id, Name, Location, BuildingCode, RoomNumber, ResponsiblePerson.
- Veze:
  - 1:N prema DeviceLocation (Laboratory.DeviceLocations)

5. DeviceLocation
- Svrha: poveznica uredaj-laboratorij kroz vrijeme (trenutna i povijesna lokacija).
- Kljucna svojstva: Id, DeviceId, LaboratoryId, AssignedDate, RemovedDate, IsCurrentLocation, AssignmentReason.
- Veze:
  - N:1 prema Device
  - N:1 prema Laboratory

## Relacijski sazetak

- Device 1---N Calibration
- Technician 1---N Calibration
- Device 1---N DeviceLocation
- Laboratory 1---N DeviceLocation

## Napomene o EF modelu

- Model koristi nullable reference i nullable FK gdje su podaci opcionalni.
- Navigacijska svojstva za kolekcije su virtual ICollection radi EF-prijateljskog modela.
- Veze i brisanja su dodatno definirani u ApplicationDbContext.OnModelCreating.
