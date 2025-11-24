namespace CEMSystem

open Avalonia
open Avalonia.Controls
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Layout
open Avalonia.Media
open Avalonia.Themes.Fluent
open Avalonia.FuncUI
open Avalonia.FuncUI.Hosts
open Avalonia.FuncUI.DSL
open CEMSystem.Components

module Main =
    let view () =
        Component(fun ctx ->
            let currentView = ctx.useState "booking" // "booking" or "validation"

            DockPanel.create
                [ DockPanel.children
                      [
                        // Navigation header
                        Border.create
                            [ Border.dock Dock.Top
                              Border.background (SolidColorBrush(Color.Parse("#2E3440")))
                              Border.padding (15.0, 10.0)
                              Border.child (
                                  StackPanel.create
                                      [ StackPanel.orientation Orientation.Vertical
                                        StackPanel.spacing 15.0
                                        StackPanel.children
                                            [
                                              // Title
                                              TextBlock.create
                                                  [ TextBlock.text "🎬 CEM Cinema Management System"
                                                    TextBlock.fontSize 24.0
                                                    TextBlock.fontWeight FontWeight.Bold
                                                    TextBlock.foreground Brushes.White
                                                    TextBlock.horizontalAlignment HorizontalAlignment.Center ]

                                              // Navigation buttons
                                              StackPanel.create
                                                  [ StackPanel.orientation Orientation.Horizontal
                                                    StackPanel.horizontalAlignment HorizontalAlignment.Center
                                                    StackPanel.spacing 20.0
                                                    StackPanel.children
                                                        [ Button.create
                                                              [ Button.content "🎬 Cinema Booking"
                                                                Button.onClick (fun _ -> currentView.Set("booking"))
                                                                Button.fontSize 16.0
                                                                Button.fontWeight FontWeight.SemiBold
                                                                Button.padding (20.0, 10.0)
                                                                Button.cornerRadius 8.0
                                                                Button.background (
                                                                    if currentView.Current = "booking" then
                                                                        SolidColorBrush(Color.Parse("#5E81AC"))
                                                                    else
                                                                        SolidColorBrush(Color.Parse("#4C566A"))
                                                                )
                                                                Button.foreground Brushes.White ]

                                                          Button.create
                                                              [ Button.content "🎫 Staff Validation"
                                                                Button.onClick (fun _ -> currentView.Set("validation"))
                                                                Button.fontSize 16.0
                                                                Button.fontWeight FontWeight.SemiBold
                                                                Button.padding (20.0, 10.0)
                                                                Button.cornerRadius 8.0
                                                                Button.background (
                                                                    if currentView.Current = "validation" then
                                                                        SolidColorBrush(Color.Parse("#5E81AC"))
                                                                    else
                                                                        SolidColorBrush(Color.Parse("#4C566A"))
                                                                )
                                                                Button.foreground Brushes.White ] ] ] ] ]
                              ) ]

                        // Content area
                        ContentControl.create
                            [ ContentControl.content (
                                  match currentView.Current with
                                  | "validation" -> StaffValidationView.view ()
                                  | _ -> CinemaView.view ()
                              ) ] ] ])

type MainWindow() =
    inherit HostWindow()

    do
        base.Title <- "CEM Cinema Management System - Booking & Ticketing"
        base.Content <- Main.view ()
        base.Width <- 1200.0
        base.Height <- 800.0

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add(FluentTheme())
        this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime -> desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =
    [<EntryPoint>]
    let main (args: string[]) =
        AppBuilder.Configure<App>().UsePlatformDetect().UseSkia().StartWithClassicDesktopLifetime(args)
