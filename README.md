# Cinema Seat Booking System (CEMSystem)

A modern cinema seat booking application built with F# and Avalonia UI that manages seat reservations in a 20x11 cinema hall.

## Features

### Cinema Hall Layout

- **Dimensions**: 20 seats wide × 11 rows high (220 total seats)
- **2D Array Representation**: Seats are stored and managed in a 2D array structure
- **Visual Grid**: Interactive visual representation of the cinema hall with a screen at the top

### Seat Management

- **Seat States**:
  - 🟢 Available (Light Green)
  - 🔴 Booked (Red)
  - 🔵 Selected (Light Blue)
- **Seat Identification**: Each seat is labeled with Row-Column format (e.g., "1-5", "11-20")
- **Click to Select**: Click any seat to view its status or select it for booking

### Booking Operations

- **Book Seat**: Reserve an available seat for a specific customer
- **Cancel Booking**: Release a previously booked seat back to available status
- **Customer Information**: Store customer name and booking timestamp
- **Real-time Updates**: Immediate visual feedback for all booking operations

### Data Persistence

- **JSON Storage**: All booking data is saved in `cinema_bookings.json`
- **Auto-save**: Bookings are automatically saved after each operation
- **Auto-load**: Previous bookings are restored when the application starts
- **Backup-friendly**: Human-readable JSON format for easy backup and transfer

## Technical Architecture

### Technologies Used

- **Language**: F# (Functional Programming)
- **UI Framework**: Avalonia UI with FuncUI
- **Data Format**: JSON with System.Text.Json
- **Platform**: Cross-platform (.NET)

### Project Structure

```
CEMSystem/
├── Data/
│   └── BD.fs                 # Data layer with booking logic
├── Components/
│   └── CinemaView.fs        # UI components for cinema visualization
├── Program.fs               # Main application entry point
├── CEMSystem.fsproj        # Project configuration
└── cinema_bookings.json    # Seat booking data (created at runtime)
```

### Data Models

#### Seat Structure

```fsharp
type Seat = {
    Row: int                    // Seat row (1-11)
    Column: int                // Seat column (1-20)
    Status: SeatStatus         // Available or Booked
    BookedBy: string option    // Customer name (if booked)
    BookingTime: DateTime option // When the booking was made
}
```

#### Cinema Hall

```fsharp
type CinemaHall = {
    Width: int          // 20 seats
    Height: int         // 11 rows
    Seats: Seat[,]      // 2D array of seats
}
```

## Usage Instructions

### Running the Application

```bash
# Navigate to project directory
cd /path/to/CEMSystem

# Build and run
dotnet run
```

### Using the Interface

1. **View Cinema Hall**: The main screen shows all 220 seats arranged in 11 rows of 20 seats each
2. **Select a Seat**: Click any seat to select it (available seats turn light blue when selected)
3. **Enter Customer Name**: Type the customer's name in the text box on the right panel
4. **Book Seat**: Click "Book Seat" to confirm the reservation
5. **Cancel Booking**: Select a booked seat and click "Cancel Booking" to release it
6. **View Statistics**: Check available/total seat counts in the right panel
7. **Refresh Data**: Use the "Refresh" button to reload data from the JSON file

### Booking Examples

**Booking a Seat:**

1. Click seat "5-10" (Row 5, Column 10)
2. Enter "John Doe" as customer name
3. Click "Book Seat"
4. Seat turns red and shows as booked

**Canceling a Booking:**

1. Click any red (booked) seat
2. Click "Cancel Booking"
3. Seat turns green and becomes available again

## Data Storage Format

The `cinema_bookings.json` file stores all seat data in this format:

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

Where `Status: 0` = Available, `Status: 1` = Booked

## Development Features

- **Type Safety**: F# provides compile-time safety for all booking operations
- **Immutable Data**: Functional programming principles ensure data integrity
- **Error Handling**: Comprehensive error handling with detailed user feedback
- **Responsive UI**: Real-time updates and visual feedback for all operations
- **Cross-platform**: Runs on Windows, macOS, and Linux

## Future Enhancements

Potential improvements for the system:

- Multiple cinema halls support
- Seat pricing tiers
- Booking time limits
- Customer contact information
- Booking confirmation emails
- Seat selection preferences (aisle, center, etc.)
- Group booking functionality
- Payment integration

---

**Built with ❤️ using F# and Avalonia UI**
