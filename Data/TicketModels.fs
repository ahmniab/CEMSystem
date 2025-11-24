namespace CEMSystem.Data

open System

// Ticket data structures
[<CLIMutable>]
type TicketInfo =
    { CustomerName: string
      SeatRow: int
      SeatColumn: int
      BookingDate: DateTime
      TicketId: string }

// Ticket payload for JWT-like token
[<CLIMutable>]
type TicketPayload =
    { CustomerName: string
      SeatInfo: string // "Row X, Seat Y" format
      BookingDateTime: DateTime
    // No expiration date as per requirements
    }

// Ticket validation result
type TicketValidationResult =
    | ValidTicket of TicketInfo
    | InvalidTicket of string
    | TicketNotFound
    | ValidationError of string

// Ticket operation result
type TicketOperationResult =
    | TicketCreated of TicketInfo
    | TicketRedeemed of TicketInfo
    | TicketError of string
