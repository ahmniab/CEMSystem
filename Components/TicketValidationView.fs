namespace CEMSystem.Components

open System
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.Media
open CEMSystem.Data
open CEMSystem.Services

module TicketValidationView =

    let view () =
        Component(fun ctx ->
            let ticketId = ctx.useState ""
            let validationResult = ctx.useState ""
            let isValidating = ctx.useState false
            let validatedTicket = ctx.useState (None: TicketInfo option)
            let canRedeem = ctx.useState false

            let validateTicket () =
                isValidating.Set true
                validationResult.Set "Validating..."

                try
                    let result = TicketService.validateTicket ticketId.Current

                    match result with
                    | ValidTicket ticketInfo ->
                        let resultText =
                            sprintf
                                "✅ VALID TICKET\nCustomer: %s\nSeat: Row %d, Column %d\nBooked: %s\nTicket ID: %s"
                                ticketInfo.CustomerName
                                ticketInfo.SeatRow
                                ticketInfo.SeatColumn
                                (ticketInfo.BookingDate.ToString("yyyy-MM-dd HH:mm"))
                                ticketInfo.TicketId

                        validationResult.Set resultText
                        validatedTicket.Set(Some ticketInfo)
                        canRedeem.Set true
                    | InvalidTicket reason ->
                        validationResult.Set $"❌ INVALID TICKET\nReason: {reason}"
                        validatedTicket.Set None
                        canRedeem.Set false
                    | TicketNotFound ->
                        validationResult.Set "❌ TICKET NOT FOUND\nThe ticket ID does not exist in our system."
                        validatedTicket.Set None
                        canRedeem.Set false
                    | ValidationError msg ->
                        validationResult.Set $"⚠️ VALIDATION ERROR\n{msg}"
                        validatedTicket.Set None
                        canRedeem.Set false
                finally
                    isValidating.Set false

            let redeemTicket () =
                match validatedTicket.Current with
                | Some ticketInfo ->
                    let redeemResult = TicketService.redeemTicket ticketInfo.TicketId

                    match redeemResult with
                    | TicketRedeemed ticketInfo ->
                        // Clear the booking from cinema
                        match CinemaService.loadCinemaData () with
                        | Result.Ok cinema ->
                            match CinemaService.clearBooking cinema ticketInfo.SeatRow ticketInfo.SeatColumn with
                            | Result.Ok msg ->
                                validationResult.Set
                                    $"🎉 TICKET REDEEMED SUCCESSFULLY\n{msg}\nCustomer can enter the cinema!"

                                canRedeem.Set false
                                validatedTicket.Set None
                                ticketId.Set ""
                            | Result.Error msg ->
                                validationResult.Set $"⚠️ Ticket redeemed but seat clearing failed: {msg}"
                        | Result.Error msg -> validationResult.Set $"⚠️ Ticket redeemed but cinema data error: {msg}"
                    | TicketError msg -> validationResult.Set $"❌ REDEMPTION FAILED\n{msg}"
                    | _ -> validationResult.Set "❌ Unexpected redemption result"
                | None -> validationResult.Set "❌ No valid ticket to redeem"

            let clearForm () =
                ticketId.Set ""
                validationResult.Set ""
                validatedTicket.Set None
                canRedeem.Set false

            Border.create
                [ Border.padding 30.0
                  Border.background Brushes.White
                  Border.child (
                      StackPanel.create
                          [ StackPanel.orientation Orientation.Vertical
                            StackPanel.spacing 20.0
                            StackPanel.children
                                [
                                  // Header
                                  TextBlock.create
                                      [ TextBlock.text "🎫 Ticket Validation System"
                                        TextBlock.fontSize 28.0
                                        TextBlock.fontWeight FontWeight.Bold
                                        TextBlock.foreground Brushes.DarkBlue
                                        TextBlock.horizontalAlignment HorizontalAlignment.Center
                                        TextBlock.margin (0.0, 0.0, 0.0, 20.0) ]

                                  // Input section
                                  StackPanel.create
                                      [ StackPanel.orientation Orientation.Vertical
                                        StackPanel.spacing 10.0
                                        StackPanel.children
                                            [ TextBlock.create
                                                  [ TextBlock.text "Enter Ticket ID:"
                                                    TextBlock.fontSize 16.0
                                                    TextBlock.fontWeight FontWeight.SemiBold
                                                    TextBlock.foreground Brushes.DarkGray ]

                                              TextBox.create
                                                  [ TextBox.text ticketId.Current
                                                    TextBox.onTextChanged ticketId.Set
                                                    TextBox.watermark "e.g., TKT-a1b2c3d4e5f6g7h8"
                                                    TextBox.fontSize 14.0
                                                    TextBox.padding (10.0, 8.0)
                                                    TextBox.isEnabled (not isValidating.Current) ] ] ]

                                  // Action buttons
                                  StackPanel.create
                                      [ StackPanel.orientation Orientation.Horizontal
                                        StackPanel.spacing 15.0
                                        StackPanel.horizontalAlignment HorizontalAlignment.Center
                                        StackPanel.children
                                            [ Button.create
                                                  [ Button.content "🔍 Validate Ticket"
                                                    Button.background Brushes.DodgerBlue
                                                    Button.foreground Brushes.White
                                                    Button.padding (20.0, 10.0)
                                                    Button.fontSize 14.0
                                                    Button.fontWeight FontWeight.SemiBold
                                                    Button.isEnabled (
                                                        not (String.IsNullOrWhiteSpace(ticketId.Current))
                                                        && not isValidating.Current
                                                    )
                                                    Button.onClick (fun _ -> validateTicket ()) ]

                                              Button.create
                                                  [ Button.content "🎉 Redeem & Enter"
                                                    Button.background Brushes.Green
                                                    Button.foreground Brushes.White
                                                    Button.padding (20.0, 10.0)
                                                    Button.fontSize 14.0
                                                    Button.fontWeight FontWeight.SemiBold
                                                    Button.isEnabled (canRedeem.Current && not isValidating.Current)
                                                    Button.onClick (fun _ -> redeemTicket ()) ]

                                              Button.create
                                                  [ Button.content "🔄 Clear"
                                                    Button.background Brushes.Gray
                                                    Button.foreground Brushes.White
                                                    Button.padding (20.0, 10.0)
                                                    Button.fontSize 14.0
                                                    Button.fontWeight FontWeight.SemiBold
                                                    Button.isEnabled (not isValidating.Current)
                                                    Button.onClick (fun _ -> clearForm ()) ] ] ]

                                  // Results section
                                  if not (String.IsNullOrWhiteSpace(validationResult.Current)) then
                                      Border.create
                                          [ Border.background Brushes.LightGray
                                            Border.borderBrush Brushes.DarkGray
                                            Border.borderThickness 1.0
                                            Border.cornerRadius 8.0
                                            Border.padding 20.0
                                            Border.margin (0.0, 10.0, 0.0, 0.0)
                                            Border.child (
                                                StackPanel.create
                                                    [ StackPanel.orientation Orientation.Vertical
                                                      StackPanel.spacing 5.0
                                                      StackPanel.children
                                                          [ TextBlock.create
                                                                [ TextBlock.text "Validation Result:"
                                                                  TextBlock.fontSize 14.0
                                                                  TextBlock.fontWeight FontWeight.Bold
                                                                  TextBlock.foreground Brushes.DarkBlue ]

                                                            TextBlock.create
                                                                [ TextBlock.text validationResult.Current
                                                                  TextBlock.fontSize 12.0
                                                                  TextBlock.textWrapping TextWrapping.Wrap
                                                                  TextBlock.foreground Brushes.Black
                                                                  TextBlock.fontFamily "Consolas, Monaco, monospace" ] ] ]
                                            ) ]

                                  // Instructions
                                  Border.create
                                      [ Border.background Brushes.LightBlue
                                        Border.cornerRadius 8.0
                                        Border.padding 15.0
                                        Border.margin (0.0, 20.0, 0.0, 0.0)
                                        Border.child (
                                            StackPanel.create
                                                [ StackPanel.orientation Orientation.Vertical
                                                  StackPanel.spacing 8.0
                                                  StackPanel.children
                                                      [ TextBlock.create
                                                            [ TextBlock.text "📋 Instructions:"
                                                              TextBlock.fontSize 14.0
                                                              TextBlock.fontWeight FontWeight.Bold
                                                              TextBlock.foreground Brushes.DarkBlue ]

                                                        TextBlock.create
                                                            [ TextBlock.text
                                                                  "1. Enter the ticket ID from the customer's ticket"
                                                              TextBlock.fontSize 12.0
                                                              TextBlock.foreground Brushes.DarkBlue ]

                                                        TextBlock.create
                                                            [ TextBlock.text
                                                                  "2. Click 'Validate Ticket' to check if it's valid"
                                                              TextBlock.fontSize 12.0
                                                              TextBlock.foreground Brushes.DarkBlue ]

                                                        TextBlock.create
                                                            [ TextBlock.text
                                                                  "3. If valid, click 'Redeem & Enter' to allow entry"
                                                              TextBlock.fontSize 12.0
                                                              TextBlock.foreground Brushes.DarkBlue ]

                                                        TextBlock.create
                                                            [ TextBlock.text
                                                                  "4. Seat will be automatically cleared after redemption"
                                                              TextBlock.fontSize 12.0
                                                              TextBlock.foreground Brushes.DarkBlue ] ] ]
                                        ) ] ] ]
                  ) ])
