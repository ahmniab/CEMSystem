namespace CEMSystem.Components

open System
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.Media
open CEMSystem.Data
open CEMSystem.Services

module StaffValidationView =
    let view () =
        Component(fun ctx ->
            let ticketId = ctx.useState ""
            let validationResult = ctx.useState ""
            let isValid = ctx.useState false
            let ticketDetails = ctx.useState (None: TicketInfo option)

            let validateTicket () =
                if String.IsNullOrWhiteSpace(ticketId.Current) then
                    validationResult.Set "Please enter a ticket ID"
                    isValid.Set false
                    ticketDetails.Set None
                else
                    let cleanTicketId = ticketId.Current.Trim()

                    match TicketService.validateTicket cleanTicketId with
                    | ValidTicket info ->
                        validationResult.Set "✅ VALID TICKET"
                        isValid.Set true
                        ticketDetails.Set(Some info)
                    | InvalidTicket msg ->
                        validationResult.Set $"❌ INVALID: {msg}"
                        isValid.Set false
                        ticketDetails.Set None
                    | TicketNotFound ->
                        validationResult.Set "❌ TICKET NOT FOUND"
                        isValid.Set false
                        ticketDetails.Set None
                    | ValidationError msg ->
                        validationResult.Set $"❌ ERROR: {msg}"
                        isValid.Set false
                        ticketDetails.Set None

            let redeemTicket () =
                let cleanTicketId = ticketId.Current.Trim()

                match TicketService.redeemTicket cleanTicketId with
                | TicketRedeemed info ->
                    validationResult.Set "🎬 TICKET REDEEMED"
                    isValid.Set false
                    ticketDetails.Set None
                    ticketId.Set ""
                | TicketError msg -> validationResult.Set $"❌ ERROR: {msg}"
                | _ -> validationResult.Set "❌ FAILED"

            StackPanel.create
                [ StackPanel.orientation Orientation.Vertical
                  StackPanel.spacing 20.0
                  StackPanel.margin 30.0
                  StackPanel.children
                      [ TextBlock.create
                            [ TextBlock.text "🎫 Staff Validation"
                              TextBlock.fontSize 24.0
                              TextBlock.fontWeight FontWeight.Bold
                              TextBlock.foreground Brushes.White
                              TextBlock.horizontalAlignment HorizontalAlignment.Center ]

                        TextBox.create
                            [ TextBox.text ticketId.Current
                              TextBox.onTextChanged ticketId.Set
                              TextBox.watermark "Enter ticket ID"
                              TextBox.fontSize 16.0
                              TextBox.height 40.0 ]

                        StackPanel.create
                            [ StackPanel.orientation Orientation.Horizontal
                              StackPanel.spacing 10.0
                              StackPanel.horizontalAlignment HorizontalAlignment.Center
                              StackPanel.children
                                  [ Button.create
                                        [ Button.content "Validate"
                                          Button.onClick (fun _ -> validateTicket ())
                                          Button.fontSize 14.0
                                          Button.padding (15.0, 8.0)
                                          Button.background Brushes.Blue
                                          Button.foreground Brushes.White ]

                                    Button.create
                                        [ Button.content "Allow Entry"
                                          Button.onClick (fun _ -> redeemTicket ())
                                          Button.fontSize 14.0
                                          Button.padding (15.0, 8.0)
                                          Button.background Brushes.Green
                                          Button.foreground Brushes.White
                                          Button.isEnabled isValid.Current ]

                                    Button.create
                                        [ Button.content "Clear"
                                          Button.onClick (fun _ ->
                                              ticketId.Set ""
                                              validationResult.Set ""
                                              isValid.Set false
                                              ticketDetails.Set None)
                                          Button.fontSize 14.0
                                          Button.padding (15.0, 8.0)
                                          Button.background Brushes.Gray
                                          Button.foreground Brushes.White ] ] ]

                        if not (String.IsNullOrWhiteSpace(validationResult.Current)) then
                            TextBlock.create
                                [ TextBlock.text validationResult.Current
                                  TextBlock.fontSize 16.0
                                  TextBlock.foreground Brushes.White
                                  TextBlock.fontWeight FontWeight.Bold
                                  TextBlock.horizontalAlignment HorizontalAlignment.Center ]

                        match ticketDetails.Current with
                        | Some info ->
                            StackPanel.create
                                [ StackPanel.orientation Orientation.Vertical
                                  StackPanel.spacing 5.0
                                  StackPanel.children
                                      [ TextBlock.create
                                            [ TextBlock.text "TICKET DETAILS"
                                              TextBlock.fontSize 16.0
                                              TextBlock.fontWeight FontWeight.Bold
                                              TextBlock.foreground Brushes.White ]
                                        TextBlock.create
                                            [ TextBlock.text $"Customer: {info.CustomerName}"
                                              TextBlock.fontSize 14.0
                                              TextBlock.foreground Brushes.White ]
                                        TextBlock.create
                                            [ TextBlock.text $"Seat: Row {info.SeatRow}, Column {info.SeatColumn}"
                                              TextBlock.fontSize 14.0
                                              TextBlock.foreground Brushes.White ] ] ]
                        | None -> StackPanel.create [ StackPanel.children [] ] ] ])
