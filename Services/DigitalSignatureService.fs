namespace CEMSystem.Services

open System
open System.Text
open System.Security.Cryptography
open System.Text.Json
open CEMSystem.Data

module DigitalSignatureService =

    // Secret key for signing (in production, this should be stored securely)
    let private secretKey = "CEMSystem_Secret_Key_2024_Cinema_Booking_System"

    // Create HMAC-SHA256 signature
    let private createSignature (data: string) =
        use hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey))
        let hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data))
        Convert.ToBase64String(hash)

    // Create JWT-like token structure
    let createTicketToken (payload: TicketPayload) =
        try
            // Create header
            let header = {| alg = "HS256"; typ = "TKT" |} // TKT for Ticket
            let headerJson = JsonSerializer.Serialize(header)
            let headerBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(headerJson))

            // Create payload
            let payloadJson = JsonSerializer.Serialize(payload)
            let payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadJson))

            // Create signature
            let dataToSign = $"{headerBase64}.{payloadBase64}"
            let signature = createSignature dataToSign
            let signatureBase64 = signature.Replace("+", "-").Replace("/", "_").Replace("=", "")

            // Combine all parts
            let token = $"{headerBase64}.{payloadBase64}.{signatureBase64}"
            Result.Ok token
        with ex ->
            Result.Error $"Failed to create ticket token: {ex.Message}"

    // Verify and decode ticket token
    let verifyTicketToken (token: string) =
        try
            let parts = token.Split('.')

            if parts.Length <> 3 then
                Result.Error "Invalid token format"
            else
                let headerBase64 = parts.[0]
                let payloadBase64 = parts.[1]
                let receivedSignature = parts.[2].Replace("-", "+").Replace("_", "/")

                // Add padding if needed
                let paddedSignature =
                    let padding = 4 - (receivedSignature.Length % 4)

                    if padding < 4 then
                        receivedSignature + String('=', padding)
                    else
                        receivedSignature

                // Verify signature
                let dataToSign = $"{headerBase64}.{payloadBase64}"
                let expectedSignature = createSignature dataToSign

                if expectedSignature = paddedSignature then
                    // Decode payload
                    let payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(payloadBase64))
                    let payload = JsonSerializer.Deserialize<TicketPayload>(payloadJson)
                    Result.Ok payload
                else
                    Result.Error "Invalid signature"
        with ex ->
            Result.Error $"Failed to verify token: {ex.Message}"

    // Generate unique ticket ID based on ticket data
    let generateTicketId (customerName: string) (seatInfo: string) (bookingTime: DateTime) =
        let formattedTime = bookingTime.ToString("yyyy-MM-dd-HH-mm-ss")
        let data = $"{customerName}:{seatInfo}:{formattedTime}"
        use sha = SHA256.Create()
        let hash = sha.ComputeHash(Encoding.UTF8.GetBytes(data))
        let hashString = BitConverter.ToString(hash).Replace("-", "").ToLower()
        $"TKT-{hashString.[..15]}" // Take first 16 characters for readability
