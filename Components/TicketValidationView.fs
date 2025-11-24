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
                    let validationState = ValidationHelpers.validateTicketId ticketId.Current
                    let formattedResult = ValidationHelpers.formatValidationResult validationState
                    validationResult.Set formattedResult
                    validatedTicket.Set validationState.TicketInfo
                    canRedeem.Set validationState.IsValid
                finally
                    isValidating.Set false

            let redeemTicket () =
                match validatedTicket.Current with
                | Some ticketInfo ->
                    match ValidationHelpers.redeemTicketWithSeatClearing ticketInfo.TicketId with
                    | Result.Ok message ->
                        validationResult.Set message
                        canRedeem.Set false
                        validatedTicket.Set None
                        ticketId.Set ""
                    | Result.Error errorMessage -> validationResult.Set errorMessage
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
                                [ TextBlock.create
                                      [ TextBlock.text "🎫 Ticket Validation System"
                                        TextBlock.fontSize 28.0
                                        TextBlock.fontWeight FontWeight.Bold
                                        TextBlock.foreground Brushes.DarkBlue
                                        TextBlock.horizontalAlignment HorizontalAlignment.Center ]

                                  StackPanel.create
                                      [ StackPanel.orientation Orientation.Vertical
                                        StackPanel.spacing 10.0
                                        StackPanel.children
                                            [ TextBlock.create
                                                  [ TextBlock.text "Enter Ticket ID:"; TextBlock.fontSize 16.0 ]

                                              TextBox.create
                                                  [ TextBox.text ticketId.Current
                                                    TextBox.onTextChanged ticketId.Set
                                                    TextBox.watermark "e.g., TKT-a1b2c3d4e5f6g7h8"
                                                    TextBox.fontSize 14.0
                                                    TextBox.isEnabled (not isValidating.Current) ] ] ]

                                  StackPanel.create
                                      [ StackPanel.orientation Orientation.Horizontal
                                        StackPanel.spacing 15.0
                                        StackPanel.horizontalAlignment HorizontalAlignment.Center
                                        StackPanel.children
                                            [ Button.create
                                                  [ Button.content "🔍 Validate Ticket"
                                                    Button.background Brushes.DodgerBlue
                                                    Button.foreground Brushes.White
                                                    Button.isEnabled (
                                                        not (String.IsNullOrWhiteSpace(ticketId.Current))
                                                        && not isValidating.Current
                                                    )
                                                    Button.onClick (fun _ -> validateTicket ()) ]

                                              Button.create
                                                  [ Button.content "🎉 Redeem & Enter"
                                                    Button.background Brushes.Green
                                                    Button.foreground Brushes.White
                                                    Button.isEnabled (canRedeem.Current && not isValidating.Current)
                                                    Button.onClick (fun _ -> redeemTicket ()) ]

                                              Button.create
                                                  [ Button.content "🔄 Clear"
                                                    Button.background Brushes.Gray
                                                    Button.foreground Brushes.White
                                                    Button.isEnabled (not isValidating.Current)
                                                    Button.onClick (fun _ -> clearForm ()) ] ] ]

                                  if not (String.IsNullOrWhiteSpace(validationResult.Current)) then
                                      Border.create
                                          [ Border.background Brushes.LightGray
                                            Border.padding 20.0
                                            Border.child (
                                                TextBlock.create
                                                    [ TextBlock.text validationResult.Current
                                                      TextBlock.fontSize 12.0
                                                      TextBlock.textWrapping TextWrapping.Wrap ]
                                            ) ] ] ]
                  ) ])
