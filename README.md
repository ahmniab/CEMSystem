# CEM Cinema Management System 🎬# Cinema Seat Booking System (CEMSystem)

A comprehensive cinema seat booking and ticket management system built with F# and Avalonia UI.A modern cinema seat booking application built with F# and Avalonia UI that manages seat reservations in a 20x11 cinema hall.

## Features 🎯## Features

### 🎪 Cinema Booking System### Cinema Hall Layout

- **Visual Seat Selection**: Interactive 2D grid representing cinema seats (20x11 layout)

- **Real-time Availability**: Color-coded seat status (Available: Green, Booked: Red, Selected: Blue)- **Dimensions**: 20 seats wide × 11 rows high (220 total seats)

- **Customer Management**: Book seats with customer name and automatic timestamps- **2D Array Representation**: Seats are stored and managed in a 2D array structure

- **Data Persistence**: All bookings saved to `cinema_bookings.json`- **Visual Grid**: Interactive visual representation of the cinema hall with a screen at the top

- **Statistics**: Live display of available/total seats

### Seat Management

### 🎫 Digital Ticketing System

- **Automatic Ticket Generation**: Each booking creates a secure digital ticket- **Seat States**:

- **JWT-like Security**: Cryptographically signed tickets with HMAC-SHA256 - 🟢 Available (Light Green)

- **Unique Ticket IDs**: SHA256-based unique identifiers (e.g., TKT-a1b2c3d4e5f6g7h8) - 🔴 Booked (Red)

- **HTML Ticket Export**: Beautiful, printable HTML tickets with professional design - 🔵 Selected (Light Blue)

- **Digital Signatures**: Tamper-proof token validation- **Seat Identification**: Each seat is labeled with Row-Column format (e.g., "1-5", "11-20")

- **Click to Select**: Click any seat to view its status or select it for booking

### 🔐 Ticket Validation System

- **Ticket Verification**: Validate tickets by ID with cryptographic signature checking### Booking Operations

- **Entry Management**: Redeem valid tickets to allow cinema entry

- **Automatic Seat Clearing**: Seats become available after ticket redemption- **Book Seat**: Reserve an available seat for a specific customer

- **Anti-fraud Protection**: One-time use tickets prevent duplicate entries- **Cancel Booking**: Release a previously booked seat back to available status

- **Customer Information**: Store customer name and booking timestamp

## Technical Architecture 🏗️- **Real-time Updates**: Immediate visual feedback for all booking operations

### Modular Design### Data Persistence

The system is organized into multiple modules for maintainability:

- **JSON Storage**: All booking data is saved in `cinema_bookings.json`

````- **Auto-save**: Bookings are automatically saved after each operation

CEMSystem/- **Auto-load**: Previous bookings are restored when the application starts

├── Data/- **Backup-friendly**: Human-readable JSON format for easy backup and transfer

│   ├── BD.fs                    # Core cinema booking service

│   └── TicketModels.fs          # Ticket data structures## Technical Architecture

├── Services/

│   ├── DigitalSignatureService.fs # JWT-like token creation/validation### Technologies Used

│   ├── TicketService.fs         # Ticket CRUD operations

│   └── HtmlTicketGenerator.fs   # HTML ticket rendering- **Language**: F# (Functional Programming)

├── Components/- **UI Framework**: Avalonia UI with FuncUI

│   ├── CinemaView.fs           # Main booking interface- **Data Format**: JSON with System.Text.Json

│   └── TicketValidationView.fs # Ticket validation interface- **Platform**: Cross-platform (.NET)

└── Program.fs                   # Application entry point

```### Project Structure



### Digital Security Features 🛡️```

CEMSystem/

#### JWT-like Token Structure├── Data/

```│   └── BD.fs                 # Data layer with booking logic

Header.Payload.Signature├── Components/

```│   └── CinemaView.fs        # UI components for cinema visualization

├── Program.fs               # Main application entry point

**Header**: `{"alg":"HS256","typ":"TKT"}`├── CEMSystem.fsproj        # Project configuration

└── cinema_bookings.json    # Seat booking data (created at runtime)

**Payload**: Contains:```

- Customer name

- Seat information ("Row X, Seat Y")### Data Models

- Booking date and time

- No expiration date (as per requirements)#### Seat Structure



**Signature**: HMAC-SHA256 with secret key```fsharp

type Seat = {

#### Ticket ID Generation    Row: int                    // Seat row (1-11)

- SHA256 hash of: `{customer}:{seat_info}:{timestamp}`    Column: int                // Seat column (1-20)

- Format: `TKT-{first16chars}`    Status: SeatStatus         // Available or Booked

- Example: `TKT-a1b2c3d4e5f6g7h8`    BookedBy: string option    // Customer name (if booked)

    BookingTime: DateTime option // When the booking was made

## Usage Guide 📖}

````

### 1. Booking a Seat

1. Open the application and go to "🎬 Cinema Booking" tab#### Cinema Hall

2. Click on an available (green) seat

3. Enter customer name in the text field```fsharp

4. Click "Book Seat"type CinemaHall = {

5. System generates ticket automatically Width: int // 20 seats

6. HTML ticket file is saved (e.g., `ticket_TKT-a1b2c3d4e5f6g7h8.html`) Height: int // 11 rows

   Seats: Seat[,] // 2D array of seats

### 2. Validating Tickets}

1. Go to "🎫 Ticket Validation" tab```

2. Enter the ticket ID (from customer's ticket)

3. Click "🔍 Validate Ticket"## Usage Instructions

4. System shows ticket details and validity

5. If valid, click "🎉 Redeem & Enter" to allow entry### Running the Application

6. Seat is automatically cleared for new bookings

````bash

### 3. Ticket Information# Navigate to project directory

Each ticket contains:cd /path/to/CEMSystem

- **Customer Name**: Who booked the seat

- **Seat Information**: Row and column number# Build and run

- **Booking Date/Time**: When the reservation was madedotnet run

- **Ticket ID**: Unique identifier for validation```

- **Digital Signature**: Cryptographic proof of authenticity

### Using the Interface

## Data Storage 💾

1. **View Cinema Hall**: The main screen shows all 220 seats arranged in 11 rows of 20 seats each

### Files Created2. **Select a Seat**: Click any seat to select it (available seats turn light blue when selected)

- `cinema_bookings.json`: Cinema seat reservations3. **Enter Customer Name**: Type the customer's name in the text box on the right panel

- `tickets.json`: Ticket database with signatures4. **Book Seat**: Click "Book Seat" to confirm the reservation

- `ticket_TKT-*.html`: Individual HTML tickets5. **Cancel Booking**: Select a booked seat and click "Cancel Booking" to release it

6. **View Statistics**: Check available/total seat counts in the right panel

### JSON Structure7. **Refresh Data**: Use the "Refresh" button to reload data from the JSON file

```json

{### Booking Examples

  "Width": 20,

  "Height": 11,**Booking a Seat:**

  "Seats": [

    {1. Click seat "5-10" (Row 5, Column 10)

      "Row": 1,2. Enter "John Doe" as customer name

      "Column": 1,3. Click "Book Seat"

      "Status": 1,4. Seat turns red and shows as booked

      "BookedBy": "John Doe",

      "BookingTime": "2024-11-24T14:30:00"**Canceling a Booking:**

    }

  ]1. Click any red (booked) seat

}2. Click "Cancel Booking"

```3. Seat turns green and becomes available again



## Security Features 🔒## Data Storage Format



### Anti-Fraud MeasuresThe `cinema_bookings.json` file stores all seat data in this format:

- **Cryptographic Signatures**: HMAC-SHA256 prevents ticket forgery

- **One-time Use**: Tickets become invalid after redemption```json

- **Tamper Detection**: Any modification invalidates the signature{

- **Unique IDs**: SHA256-based IDs prevent duplication  "Width": 20,

  "Height": 11,

### Validation Process  "Seats": [

1. Parse ticket ID format    {

2. Lookup ticket in database      "Row": 1,

3. Verify cryptographic signature      "Column": 1,

4. Check redemption status      "Status": 0,

5. Validate seat information      "BookedBy": null,

      "BookingTime": null

## Technical Requirements 🔧    },

    {

- **.NET 10.0**: Latest .NET framework      "Row": 1,

- **F# Language**: Functional programming paradigm      "Column": 2,

- **Avalonia UI**: Cross-platform desktop framework      "Status": 1,

- **Avalonia FuncUI**: Functional reactive UI programming      "BookedBy": "John Doe",

      "BookingTime": "2024-11-24T15:30:00.123Z"

### Dependencies    }

```xml  ]

<PackageReference Include="Avalonia.Desktop" Version="11.1.0" />}

<PackageReference Include="Avalonia.FuncUI" Version="1.5.1" />```

<PackageReference Include="Avalonia.Themes.Fluent" Version="11.1.0" />

```Where `Status: 0` = Available, `Status: 1` = Booked



## Building and Running 🚀## Development Features



```bash- **Type Safety**: F# provides compile-time safety for all booking operations

# Build the project- **Immutable Data**: Functional programming principles ensure data integrity

dotnet build- **Error Handling**: Comprehensive error handling with detailed user feedback

- **Responsive UI**: Real-time updates and visual feedback for all operations

# Run the application- **Cross-platform**: Runs on Windows, macOS, and Linux

dotnet run

```## Future Enhancements



## Future Enhancements 💡Potential improvements for the system:



Potential features for future versions:- Multiple cinema halls support

- **Multiple Halls**: Support for different cinema rooms- Seat pricing tiers

- **Seat Pricing**: Variable pricing by seat location- Booking time limits

- **Time Slots**: Multiple showings per day- Customer contact information

- **Payment Integration**: Payment processing- Booking confirmation emails

- **Barcode/QR Codes**: Visual ticket codes- Seat selection preferences (aisle, center, etc.)

- **Mobile App**: Companion mobile application- Group booking functionality

- **Web Interface**: Browser-based booking- Payment integration

- **Reporting**: Analytics and reporting dashboard

---

## File Structure Overview 📁

**Built with ❤️ using F# and Avalonia UI**

````

├── CEMSystem.fsproj # Project configuration
├── cinema_bookings.json # Cinema data (auto-generated)
├── tickets.json # Ticket database (auto-generated)
├── ticket_TKT-\*.html # Generated HTML tickets
├── Data/
├── Services/
├── Components/
└── README.md # This documentation

```

## License 📄

This project is part of the CEM Cinema Management System.

---

**Made with ❤️ using F# and Avalonia UI**
```
