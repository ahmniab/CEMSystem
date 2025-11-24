# CEM Cinema Management System 🎬

A modern cinema seat booking and ticket management system built with F# and Avalonia UI. The system provides a complete workflow from customer seat booking to staff ticket validation at cinema entrance.

## Features 🎯

### 🎪 Cinema Booking System

- **Visual Seat Selection**: Interactive 2D grid representing cinema seats (20x11 layout = 220 total seats)
- **Real-time Availability**: Color-coded seat status (🟢 Available, 🔴 Booked, 🔵 Selected)
- **Customer Management**: Book seats with customer name and automatic timestamps
- **Data Persistence**: All bookings saved to `cinema_bookings.json`
- **Statistics**: Live display of available/total seats
- **Responsive UI**: Immediate visual feedback for all booking operations

### 🎫 Simple Ticketing System

- **Automatic Ticket Generation**: Each booking creates a simple ticket with unique ID
- **Ticket IDs**: Hash-based unique identifiers (e.g., TKT-A1B2C3D4)
- **HTML Ticket Export**: Professional, printable HTML tickets
- **Ticket Database**: All tickets stored in `tickets.json`

### 👨‍💼 Staff Validation Interface

- **Dual-View Application**: Separate interfaces for customer booking and staff validation
- **Ticket Verification**: Validate tickets by ID lookup in database
- **Entry Management**: Redeem valid tickets to allow cinema entry
- **Automatic Seat Clearing**: Seats become available after ticket redemption
- **One-time Use**: Tickets become invalid after redemption

## Technical Architecture 🏗️

### Project Structure

```
CEMSystem/
├── Data/
│   ├── BD.fs                    # Core cinema booking service
│   └── TicketModels.fs          # Ticket data structures
├── Services/
│   ├── TicketService.fs         # Ticket CRUD operations
│   └── HtmlTicketGenerator.fs   # HTML ticket rendering
├── Components/
│   ├── CinemaView.fs           # Main booking interface
│   └── StaffValidationView.fs  # Staff ticket validation interface
├── Program.fs                   # Application entry point & navigation
├── CEMSystem.fsproj            # Project configuration
├── cinema_bookings.json        # Seat booking data (auto-generated)
├── tickets.json                # Ticket database (auto-generated)
├── Tickets/                    # Generated HTML tickets folder
│   └── ticket_TKT-*.html      # Individual ticket HTML files
└── README.md                   # This documentation
```

### Technologies Used

- **Language**: F# (Functional Programming)
- **UI Framework**: Avalonia UI with FuncUI for reactive components
- **Data Format**: JSON with System.Text.Json
- **Platform**: Cross-platform (.NET 10.0)
- **Architecture**: Modular functional design with separation of concerns

### Data Models

#### Seat Structure

```fsharp
type Seat = {
    Row: int                    // Seat row (1-11)
    Column: int                 // Seat column (1-20)
    Status: SeatStatus          // Available or Booked
    BookedBy: string option     // Customer name (if booked)
    BookingTime: DateTime option // When the booking was made
}
```

#### Cinema Hall

```fsharp
type CinemaHall = {
    Width: int      // 20 seats
    Height: int     // 11 rows
    Seats: Seat[,]  // 2D array of seats
}
```

#### Ticket Information

```fsharp
type TicketInfo = {
    CustomerName: string
    SeatRow: int
    SeatColumn: int
    BookingDate: DateTime
    TicketId: string  // Simple hash-based ID
}
```

## Getting Started 🚀

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- Any IDE that supports F# (VS Code, Visual Studio, JetBrains Rider)

### Development Setup

1. **Clone the repository:**

```bash
git clone https://github.com/ahmniab/CEMSystem.git
cd CEMSystem
```

2. **Restore dependencies:**

```bash
dotnet restore
```

3. **Build the project:**

```bash
dotnet build
```

4. **Run the application:**

```bash
dotnet run
```

The application will start with a dark theme UI showing both booking and staff validation interfaces.

### Development Commands

```bash
# Clean build artifacts
dotnet clean

# Build in release mode
dotnet build --configuration Release

# Run with specific verbosity
dotnet run --verbosity normal

# Watch for changes during development
dotnet watch run
```

## Usage Guide 📖

### Application Interface

The application has two main views accessible via navigation tabs:

#### 🎬 Cinema Booking (Customer Interface)

1. **View Cinema Hall**: Interactive grid showing all 220 seats (20x11)
2. **Select a Seat**: Click any available (green) seat to select it (turns blue)
3. **Enter Customer Details**: Type customer name in the input field
4. **Book Seat**: Click "Book Seat" to confirm reservation
5. **Ticket Generation**: System automatically creates ticket with unique ID
6. **HTML Export**: Professional ticket is saved as HTML file in `Tickets/` folder (e.g., `Tickets/ticket_TKT-A1B2C3.html`)

#### 🎫 Staff Validation (Cinema Entrance)

1. **Enter Ticket ID**: Input the ticket ID from customer's ticket
2. **Validate Ticket**: Click "Validate" to check ticket authenticity
3. **View Details**: System displays customer name, seat info, and booking date
4. **Allow Entry**: Click "Allow Entry" to redeem ticket and grant access
5. **Seat Release**: Redeemed tickets automatically clear the cinema seat

### Booking Examples

**Booking a Seat:**

1. Click seat "R05-C10" (Row 5, Column 10)
2. Enter "John Doe" as customer name
3. Click "Book Seat"
4. Seat turns red and shows as booked
5. Ticket file `Tickets/ticket_TKT-123ABC.html` is created

**Staff Validation:**

1. Customer arrives with ticket ID: `TKT-123ABC`
2. Staff enters ID and clicks "Validate"
3. System shows: "✅ VALID TICKET - John Doe, Row 5, Column 10"
4. Staff clicks "Allow Entry" to redeem ticket
5. Seat becomes available for new bookings

## Data Storage 💾

### Files Created

- `cinema_bookings.json`: Cinema seat reservations and status
- `tickets.json`: Ticket database with redemption tracking
- `Tickets/ticket_TKT-*.html`: Individual printable HTML tickets (organized in Tickets folder)

### JSON Structure Examples

**Cinema Bookings (`cinema_bookings.json`):**

```json
{
  "Width": 20,
  "Height": 11,
  "Seats": [
    {
      "Row": 1,
      "Column": 1,
      "Status": 0,
      "BookedBy": null,
      "BookingTime": null
    },
    {
      "Row": 1,
      "Column": 2,
      "Status": 1,
      "BookedBy": "John Doe",
      "BookingTime": "2024-11-24T15:30:00.123Z"
    }
  ]
}
```

_Where `Status: 0` = Available, `Status: 1` = Booked_

**Tickets Database (`tickets.json`):**

```json
[
  {
    "TicketId": "TKT-A1B2C3",
    "CustomerName": "John Doe",
    "SeatRow": 5,
    "SeatColumn": 10,
    "BookingDate": "2024-11-24T15:30:00.123Z",
    "IsRedeemed": false
  }
]
```

## Key Features ✨

### Simple & Reliable

- **No Complex Authentication**: Straightforward ticket validation by ID lookup
- **One-time Use**: Tickets become invalid after redemption to prevent reuse
- **Automatic Data Persistence**: All data automatically saved to JSON files
- **Error Handling**: Comprehensive error messages and validation

### Development Features

- **Type Safety**: F# provides compile-time safety for all booking operations
- **Immutable Data**: Functional programming principles ensure data integrity
- **Responsive UI**: Real-time updates and visual feedback for all operations
- **Cross-platform**: Runs on Windows, macOS, and Linux
- **Modular Architecture**: Clean separation of concerns with functional design

## Dependencies 📦

The project uses these NuGet packages:

```xml
<PackageReference Include="Avalonia.Desktop" Version="11.1.0" />
<PackageReference Include="Avalonia.FuncUI" Version="1.5.1" />
<PackageReference Include="Avalonia.Themes.Fluent" Version="11.1.0" />
```

## Future Enhancements 💡

Potential improvements for the system:

- **Multiple Cinema Halls**: Support for different cinema rooms and showtimes
- **Seat Pricing Tiers**: Variable pricing by seat location (front, middle, back)
- **Time-based Bookings**: Multiple showtimes per day with time slot management
- **Customer Profiles**: Customer contact information and booking history
- **Payment Integration**: Payment processing and receipt generation
- **Barcode/QR Codes**: Visual ticket codes for faster validation
- **Group Bookings**: Reserve multiple seats in a single transaction
- **Mobile App**: Companion mobile application for customer bookings
- **Web Interface**: Browser-based booking system
- **Analytics Dashboard**: Reporting and analytics for cinema management
- **Email Notifications**: Booking confirmation and reminder emails

## Contributing 🤝

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/new-feature`
3. Make your changes and test thoroughly
4. Commit your changes: `git commit -am 'Add new feature'`
5. Push to the branch: `git push origin feature/new-feature`
6. Submit a pull request

## License 📄

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

---

**Built with ❤️ using F# and Avalonia UI**
