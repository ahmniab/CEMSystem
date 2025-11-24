namespace CEMSystem.Demo

open System
open CEMSystem.Data
open CEMSystem.Services

module TicketDemo =
    
    let demonstrateTicketing () =
        printfn "🎬 CEM Cinema Ticket System Demo"
        printfn "=================================="
        
        // Create a sample booking
        let customerName = "John Doe"
        let seatRow = 5
        let seatColumn = 10
        let bookingTime = DateTime.Now
        
        printfn $"\n📋 Creating ticket for:"
        printfn $"   Customer: {customerName}"
        printfn $"   Seat: Row {seatRow}, Column {seatColumn}"
        printfn $"   Time: {bookingTime:yyyy-MM-dd HH:mm:ss}"
        
        // Generate ticket
        match TicketService.createTicket customerName seatRow seatColumn bookingTime with
        | TicketCreated ticketInfo ->
            printfn $"\n✅ Ticket Created Successfully!"
            printfn $"   🎫 Ticket ID: {ticketInfo.TicketId}"
            printfn $"   📅 Booking Date: {ticketInfo.BookingDate:yyyy-MM-dd HH:mm:ss}"
            
            // Generate HTML ticket
            match TicketService.getTicketInfo ticketInfo.TicketId with
            | Some (_, false) ->
                match TicketService.loadTickets () with
                | Result.Ok tickets ->
                    match tickets |> List.tryFind (fun t -> t.TicketId = ticketInfo.TicketId) with
                    | Some ticket ->
                        match HtmlTicketGenerator.saveTicketAsHtml ticketInfo ticket.Token with
                        | Result.Ok filename ->
                            printfn $"   📄 HTML Ticket: {filename}"
                        | Result.Error error ->
                            printfn $"   ⚠️ HTML Generation Error: {error}"
                    | None ->
                        printfn "   ⚠️ Could not find ticket in database"
                | Result.Error error ->
                    printfn $"   ⚠️ Could not load tickets: {error}"
            | Some (_, true) ->
                printfn "   ⚠️ Ticket is already redeemed"
            | None ->
                printfn "   ⚠️ Ticket not found"
            
            // Validate the ticket
            printfn $"\n🔍 Validating ticket {ticketInfo.TicketId}..."
            match TicketService.validateTicket ticketInfo.TicketId with
            | ValidTicket validatedTicketInfo ->
                printfn "   ✅ Ticket is VALID"
                printfn $"   👤 Customer: {validatedTicketInfo.CustomerName}"
                printfn $"   💺 Seat: Row {validatedTicketInfo.SeatRow}, Column {validatedTicketInfo.SeatColumn}"
                
                // Simulate ticket redemption
                printfn "\n🎉 Redeeming ticket..."
                match TicketService.redeemTicket ticketInfo.TicketId with
                | TicketRedeemed redeemedTicketInfo ->
                    printfn "   ✅ Ticket REDEEMED successfully!"
                    printfn "   🚪 Customer can enter the cinema!"
                | TicketError error ->
                    printfn $"   ❌ Redemption failed: {error}"
                | _ ->
                    printfn "   ❌ Unexpected redemption result"
                    
            | InvalidTicket reason ->
                printfn $"   ❌ Ticket is INVALID: {reason}"
            | TicketNotFound ->
                printfn "   ❌ Ticket NOT FOUND"
            | ValidationError error ->
                printfn $"   ⚠️ Validation Error: {error}"
            
            // Try to validate again (should be invalid now)
            printfn $"\n🔍 Validating ticket again after redemption..."
            match TicketService.validateTicket ticketInfo.TicketId with
            | InvalidTicket reason ->
                printfn $"   ✅ Correctly shows as INVALID: {reason}"
            | _ ->
                printfn "   ⚠️ Unexpected validation result"
                
        | TicketError error ->
            printfn $"\n❌ Ticket Creation Failed: {error}"
        | _ ->
            printfn "\n❌ Unexpected ticket creation result"
        
        printfn "\n🎬 Demo Complete!"