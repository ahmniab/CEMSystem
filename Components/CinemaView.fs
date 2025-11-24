namespace CEMSystem.Components

open System
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.Media
open CEMSystem.Data

module CinemaView =

    let view () =
        Component(fun ctx ->
            let cinema =
                ctx.useState (
                    match CinemaService.loadCinemaData () with
                    | Result.Ok c -> c
                    | Result.Error _ -> CinemaService.createCinemaHall 20 11
                )

            let selectedSeat = ctx.useState (None: (int * int) option)
            let customerName = ctx.useState ""
            let statusMessage = ctx.useState "Cinema loaded - Click a seat to select it"

            let seatButton row col seat isSelected =
                let (bgColor, fgColor) =
                    match seat.Status with
                    | SeatStatus.Available ->
                        if isSelected then
                            (Brushes.LightBlue, Brushes.Black)
                        else
                            (Brushes.LightGreen, Brushes.Black)
                    | SeatStatus.Booked -> (Brushes.Red, Brushes.White)
                    | _ -> (Brushes.Gray, Brushes.White)

                Button.create
                    [ Button.content $"{row}-{col}"
                      Button.width 35.0
                      Button.height 25.0
                      Button.margin (1.0, 1.0)
                      Button.background bgColor
                      Button.foreground fgColor
                      Button.fontSize 8.0
                      Button.onClick (fun _ ->
                          selectedSeat.Set(Some(row, col))

                          match seat.Status with
                          | SeatStatus.Available -> statusMessage.Set $"Selected seat {row}-{col} (Available)"
                          | SeatStatus.Booked ->
                              let bookedBy = seat.BookedBy |> Option.defaultValue "Unknown"
                              statusMessage.Set $"Selected seat {row}-{col} (Booked by {bookedBy})"
                          | _ -> statusMessage.Set "Invalid seat") ]

            let onBookSeat () =
                match selectedSeat.Current with
                | Some(row, col) when not (String.IsNullOrWhiteSpace(customerName.Current)) ->
                    let request =
                        { Row = row
                          Column = col
                          CustomerName = customerName.Current.Trim() }

                    match CinemaService.bookSeat cinema.Current request with
                    | SuccessWithTicket(msg, ticketInfo) ->
                        // Generate HTML ticket
                        match CEMSystem.Services.TicketService.getTicketInfo ticketInfo.TicketId with
                        | Some(_, false) ->
                            // Get the token for the ticket
                            match CEMSystem.Services.TicketService.loadTickets () with
                            | Result.Ok tickets ->
                                match tickets |> List.tryFind (fun t -> t.TicketId = ticketInfo.TicketId) with
                                | Some ticket ->
                                    match CEMSystem.Services.HtmlTicketGenerator.saveTicketAsHtml ticketInfo with
                                    | Result.Ok filename ->
                                        statusMessage.Set
                                            $"{msg}\n🎫 Ticket created: {filename}\n📋 Ticket ID: {ticketInfo.TicketId}"
                                    | Result.Error htmlError ->
                                        statusMessage.Set
                                            $"{msg}\n⚠️ Ticket created but HTML generation failed: {htmlError}\n📋 Ticket ID: {ticketInfo.TicketId}"
                                | None -> statusMessage.Set $"{msg}\n📋 Ticket ID: {ticketInfo.TicketId}"
                            | Result.Error _ -> statusMessage.Set $"{msg}\n📋 Ticket ID: {ticketInfo.TicketId}"
                        | _ -> statusMessage.Set $"{msg}\n📋 Ticket ID: {ticketInfo.TicketId}"

                        selectedSeat.Set None
                        customerName.Set ""
                        // Reload cinema data
                        match CinemaService.loadCinemaData () with
                        | Result.Ok c -> cinema.Set c
                        | Result.Error _ -> ()
                    | Success msg ->
                        statusMessage.Set msg
                        selectedSeat.Set None
                        customerName.Set ""
                        // Reload cinema data
                        match CinemaService.loadCinemaData () with
                        | Result.Ok c -> cinema.Set c
                        | Result.Error _ -> ()
                    | SeatAlreadyBooked -> statusMessage.Set "Seat is already booked"
                    | InvalidSeat -> statusMessage.Set "Invalid seat selection"
                    | Error msg -> statusMessage.Set $"Error: {msg}"
                | None -> statusMessage.Set "Please select a seat first"
                | Some _ -> statusMessage.Set "Please enter customer name"

            DockPanel.create
                [ DockPanel.children
                      [
                        // Status bar at bottom
                        Border.create
                            [ Border.dock Dock.Bottom
                              Border.background Brushes.LightGray
                              Border.padding (10.0, 5.0)
                              Border.child (
                                  TextBlock.create
                                      [ TextBlock.text statusMessage.Current
                                        TextBlock.fontSize 12.0
                                        TextBlock.foreground Brushes.Black ]
                              ) ]

                        // Control panel on the right
                        StackPanel.create
                            [ StackPanel.dock Dock.Right
                              StackPanel.orientation Orientation.Vertical
                              StackPanel.spacing 10.0
                              StackPanel.margin (20.0, 20.0, 20.0, 0.0)
                              StackPanel.width 200.0
                              StackPanel.children
                                  [ TextBlock.create
                                        [ TextBlock.text "Customer Name:"
                                          TextBlock.fontSize 12.0
                                          TextBlock.fontWeight FontWeight.Bold ]
                                    TextBox.create
                                        [ TextBox.text customerName.Current
                                          TextBox.watermark "Enter customer name"
                                          TextBox.onTextChanged customerName.Set ]

                                    // Selected seat info
                                    (match selectedSeat.Current with
                                     | Some(row, col) ->
                                         TextBlock.create
                                             [ TextBlock.text $"Selected: Row {row}, Seat {col}"
                                               TextBlock.fontSize 12.0
                                               TextBlock.foreground Brushes.Blue
                                               TextBlock.fontWeight FontWeight.Bold ]
                                     | None ->
                                         TextBlock.create
                                             [ TextBlock.text "No seat selected"
                                               TextBlock.fontSize 12.0
                                               TextBlock.foreground Brushes.Gray ])

                                    Button.create
                                        [ Button.content "Book Seat"
                                          Button.background Brushes.Green
                                          Button.foreground Brushes.White
                                          Button.isEnabled (
                                              selectedSeat.Current.IsSome
                                              && not (String.IsNullOrWhiteSpace(customerName.Current))
                                          )
                                          Button.onClick (fun _ -> onBookSeat ())
                                          Button.margin (0.0, 10.0, 0.0, 0.0) ]

                                    Button.create
                                        [ Button.content "Clear Selection"
                                          Button.background Brushes.Gray
                                          Button.foreground Brushes.White
                                          Button.isEnabled selectedSeat.Current.IsSome
                                          Button.onClick (fun _ ->
                                              selectedSeat.Set None
                                              customerName.Set ""
                                              statusMessage.Set "Selection cleared") ]

                                    // Statistics
                                    StackPanel.create
                                        [ StackPanel.orientation Orientation.Vertical
                                          StackPanel.spacing 5.0
                                          StackPanel.margin (0.0, 20.0, 0.0, 0.0)
                                          StackPanel.children
                                              [ TextBlock.create
                                                    [ TextBlock.text "Cinema Statistics:"
                                                      TextBlock.fontSize 12.0
                                                      TextBlock.fontWeight FontWeight.Bold ]
                                                TextBlock.create
                                                    [ TextBlock.text
                                                          $"Available: {CinemaService.getAvailableSeatsCount cinema.Current}"
                                                      TextBlock.fontSize 10.0 ]
                                                TextBlock.create
                                                    [ TextBlock.text
                                                          $"Total: {CinemaService.getTotalSeatsCount cinema.Current}"
                                                      TextBlock.fontSize 10.0 ] ] ]

                                    // Legend
                                    StackPanel.create
                                        [ StackPanel.orientation Orientation.Vertical
                                          StackPanel.spacing 5.0
                                          StackPanel.margin (0.0, 20.0, 0.0, 0.0)
                                          StackPanel.children
                                              [ TextBlock.create
                                                    [ TextBlock.text "Legend:"
                                                      TextBlock.fontSize 12.0
                                                      TextBlock.fontWeight FontWeight.Bold ]
                                                TextBlock.create
                                                    [ TextBlock.text "🟢 Available   🔴 Booked   🔵 Selected"
                                                      TextBlock.fontSize 10.0
                                                      TextBlock.textWrapping TextWrapping.Wrap ] ] ] ] ]

                        // Main cinema seating area
                        ScrollViewer.create
                            [ ScrollViewer.padding 20.0
                              ScrollViewer.content (
                                  StackPanel.create
                                      [ StackPanel.orientation Orientation.Vertical
                                        StackPanel.spacing 10.0
                                        StackPanel.horizontalAlignment HorizontalAlignment.Center
                                        StackPanel.children
                                            [
                                              // Screen
                                              Border.create
                                                  [ Border.background Brushes.DarkGray
                                                    Border.height 30.0
                                                    Border.width 600.0
                                                    Border.cornerRadius 5.0
                                                    Border.child (
                                                        TextBlock.create
                                                            [ TextBlock.text "SCREEN"
                                                              TextBlock.foreground Brushes.White
                                                              TextBlock.horizontalAlignment HorizontalAlignment.Center
                                                              TextBlock.verticalAlignment VerticalAlignment.Center
                                                              TextBlock.fontSize 14.0
                                                              TextBlock.fontWeight FontWeight.Bold ]
                                                    ) ]

                                              // Seats grid
                                              StackPanel.create
                                                  [ StackPanel.orientation Orientation.Vertical
                                                    StackPanel.spacing 3.0
                                                    StackPanel.children
                                                        [ for row in 1 .. cinema.Current.Height do
                                                              yield
                                                                  StackPanel.create
                                                                      [ StackPanel.orientation Orientation.Horizontal
                                                                        StackPanel.spacing 3.0
                                                                        StackPanel.horizontalAlignment
                                                                            HorizontalAlignment.Center
                                                                        StackPanel.children
                                                                            [
                                                                              // Row label
                                                                              yield
                                                                                  TextBlock.create
                                                                                      [ TextBlock.text $"R{row:D2}"
                                                                                        TextBlock.width 35.0
                                                                                        TextBlock.verticalAlignment
                                                                                            VerticalAlignment.Center
                                                                                        TextBlock.fontSize 10.0
                                                                                        TextBlock.textAlignment
                                                                                            TextAlignment.Center
                                                                                        TextBlock.fontWeight
                                                                                            FontWeight.Bold ]

                                                                              // Seats in this row
                                                                              for col in 1 .. cinema.Current.Width do
                                                                                  let seat =
                                                                                      cinema.Current.Seats.[row - 1,
                                                                                                            col - 1]

                                                                                  let isSelected =
                                                                                      match selectedSeat.Current with
                                                                                      | Some(selRow, selCol) ->
                                                                                          selRow = row && selCol = col
                                                                                      | None -> false

                                                                                  yield
                                                                                      seatButton
                                                                                          row
                                                                                          col
                                                                                          seat
                                                                                          isSelected ] ] ] ] ] ]
                              ) ] ] ])
