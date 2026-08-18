public static class EmailTemplateBuilder
{
    public static string Build(
        string title,
        string greetingName,
        string message,
        string buttonText,
        string buttonUrl,
        string? note = null)
    {
        return $"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
        </head>

        <body style="
            margin:0;
            padding:0;
            background-color:#f4f6f8;
            font-family:Arial, Helvetica, sans-serif;
            color:#212529;
        ">

            <table width="100%"
                   cellpadding="0"
                   cellspacing="0"
                   border="0"
                   style="padding:40px 15px;">

                <tr>
                    <td align="center">

                        <table width="100%"
                               cellpadding="0"
                               cellspacing="0"
                               border="0"
                               style="
                                   max-width:600px;
                                   background-color:#ffffff;
                                   border:1px solid #e5e7eb;
                                   border-radius:8px;
                               ">

                            <tr>
                                <td style="
                                    padding:30px 40px 15px;
                                    text-align:center;
                                ">
                                    <h2 style="
                                        margin:0;
                                        color:#0d6efd;
                                    ">
                                        My Website
                                    </h2>
                                </td>
                            </tr>

                            <tr>
                                <td style="padding:20px 40px 40px;">

                                    <h1 style="
                                        font-size:24px;
                                        margin-bottom:20px;
                                    ">
                                        {title}
                                    </h1>

                                    <p style="
                                        font-size:16px;
                                        line-height:1.6;
                                    ">
                                        Hello {greetingName},
                                    </p>

                                    <p style="
                                        font-size:16px;
                                        line-height:1.6;
                                    ">
                                        {message}
                                    </p>

                                    <table cellpadding="0"
                                           cellspacing="0"
                                           border="0"
                                           style="margin:30px 0;">

                                        <tr>
                                            <td style="
                                                background-color:#0d6efd;
                                                border-radius:6px;
                                            ">

                                                <a href="{buttonUrl}"
                                                   style="
                                                       display:inline-block;
                                                       padding:12px 24px;
                                                       color:#ffffff;
                                                       text-decoration:none;
                                                       font-size:16px;
                                                       font-weight:bold;
                                                   ">
                                                    {buttonText}
                                                </a>

                                            </td>
                                        </tr>

                                    </table>

                                    {(string.IsNullOrWhiteSpace(note)
                                        ? string.Empty
                                        : $"""
                                           <p style="
                                               font-size:14px;
                                               color:#6c757d;
                                               line-height:1.6;
                                           ">
                                               {note}
                                           </p>
                                           """)}

                                    <hr style="
                                        border:0;
                                        border-top:1px solid #e5e7eb;
                                        margin:30px 0;
                                    ">

                                    <p style="
                                        font-size:12px;
                                        color:#6c757d;
                                    ">
                                        If the button does not work, use this link:
                                    </p>

                                    <p style="
                                        font-size:12px;
                                        word-break:break-all;
                                    ">
                                        <a href="{buttonUrl}">
                                            {buttonUrl}
                                        </a>
                                    </p>

                                </td>
                            </tr>

                            <tr>
                                <td style="
                                    padding:20px 40px;
                                    background-color:#f8f9fa;
                                    text-align:center;
                                    font-size:12px;
                                    color:#6c757d;
                                ">
                                    © {DateTime.UtcNow.Year} My Website
                                </td>
                            </tr>

                        </table>

                    </td>
                </tr>

            </table>

        </body>
        </html>
        """;
    }
}