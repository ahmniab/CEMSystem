namespace CEMSystem.Validation

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open CEMSystem.Components

module ValidationMain =
    let view () = TicketValidationView.view ()

type ValidationWindow() =
    inherit HostWindow()

    do
        base.Title <- "CEM Cinema - Ticket Validation System"
        base.Content <- ValidationMain.view ()
        base.Width <- 800.0
        base.Height <- 600.0

type ValidationApp() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add(FluentTheme())
        this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <- ValidationWindow()
        | _ -> ()

module ValidationProgram =
    [<EntryPoint>]
    let main (args: string[]) =
        AppBuilder.Configure<ValidationApp>().UsePlatformDetect().UseSkia().StartWithClassicDesktopLifetime(args)
