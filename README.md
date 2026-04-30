# Se7ety Doctor Appointment Booking API

ASP.NET Core Web API targeting .NET 10 for a doctor appointment booking mobile app.

## Run

```powershell
cd A:\Se7ety\Se7ety.Api
dotnet restore
dotnet build
dotnet tool restore
dotnet tool run dotnet-ef database update --project Se7ety.Api.csproj --startup-project Se7ety.Api.csproj
dotnet run
```

API docs are available in development at:

- `https://localhost:7270/scalar`
- `http://localhost:5071/scalar`
- `https://localhost:7270/swagger`
- `http://localhost:5071/swagger`

The default SQL Server connection string uses LocalDB:

```json
"Server=(localdb)\\MSSQLLocalDB;Database=Se7etyAppointmentBookingDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

## Structure

```text
Se7ety.Api
  Controllers
  Data
  Domain
    Entities
    Enums
  DTOs
  Exceptions
  Helpers
  Mapping
  Middleware
  Options
  Repositories
  Services
  wwwroot/uploads/profiles
```

## Main Endpoints

Authentication:

- `POST /api/auth/patient/register`
- `POST /api/auth/doctor/register`
- `POST /api/auth/login`

Profiles:

- `GET /api/profile/patient/me`
- `PUT /api/profile/patient`
- `GET /api/profile/doctor/me`
- `PUT /api/profile/doctor`
- `PUT /api/profile/doctor/available-slots`

Patient home and search:

- `GET /api/home/doctors?pageNumber=1&pageSize=10`
- `GET /api/home/categories`
- `GET /api/doctors/search?query=cardio&pageNumber=1&pageSize=10`
- `GET /api/doctors/{doctorProfileId}`

Appointments:

- `POST /api/appointments/patient/book`
- `GET /api/appointments/patient`
- `POST /api/appointments/patient/{appointmentId}/cancel`
- `GET /api/appointments/doctor`
- `POST /api/appointments/doctor/{appointmentId}/accept`
- `POST /api/appointments/doctor/{appointmentId}/reject`

Ratings and settings:

- `POST /api/ratings`
- `POST /api/settings/change-password`

Use the `Authorization: Bearer {token}` header for secured endpoints.
