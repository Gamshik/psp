namespace HttpsServer;

internal static class HtmlTemplates
{
    public const string NotFoundHtml = "<h1>404 Not Found</h1><p>The requested resource was not found on this server.</p>";
    public const string BadRequestHtml = "<h1>400 Bad Request</h1>";

    public const string InputFormHtml = @"
<!DOCTYPE html>
<html lang='ru'>
<head>
    <meta charset='UTF-8'>
    <title>Онлайн-калькулятор выражений</title>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;600&display=swap');

        body {
            font-family: 'Inter', sans-serif;
            background: linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%);
            color: #1a1a1a;
            margin: 0;
            padding: 0;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }

        .container {
            background-color: #ffffffee;
            backdrop-filter: blur(8px);
            padding: 2.5rem;
            border-radius: 16px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.15);
            width: 100%;
            max-width: 480px;
            text-align: center;
        }

        h1 {
            font-weight: 600;
            font-size: 2rem;
            margin-bottom: 1.5rem;
            color: #ff6f61;
        }

        label {
            display: block;
            text-align: left;
            font-weight: 600;
            margin-bottom: 0.5rem;
            margin-top: 1rem;
        }

        input[type='text'] {
            width: 100%;
            padding: 0.75rem 1rem;
            border: 2px solid #ddd;
            border-radius: 12px;
            font-size: 1.05rem;
            transition: border 0.2s, box-shadow 0.2s;
        }

        input[type='text']:focus {
            border-color: #ff6f61;
            box-shadow: 0 0 5px rgba(255,111,97,0.4);
            outline: none;
        }

        input[type='submit'] {
            margin-top: 2rem;
            background: #ff6f61;
            color: #fff;
            border: none;
            border-radius: 12px;
            padding: 0.85rem 1.5rem;
            font-size: 1.1rem;
            cursor: pointer;
            transition: background 0.3s, transform 0.2s;
        }

        input[type='submit']:hover {
            background: #ff3b2f;
            transform: translateY(-2px);
        }

        .info {
            background-color: #e0f7fa;
            padding: 1rem;
            border-radius: 12px;
            color: #00796b;
            font-size: 0.95rem;
        }

        .example {
            font-size: 0.85rem;
            color: #555;
            margin-top: 0.3rem;
            text-align: left;
        }
    </style>
</head>
<body>
    <div class='container'>
        <h1>Калькулятор</h1>
        <div class='info'>
            Введите выражение. Поддерживаются: <strong>+, -, *, /</strong> и скобки <strong>()</strong>.
        </div>
        <form action='/' method='post'>
            <label for='expression'>Выражение:</label>
            <input type='text' id='expression' name='expression' value='(15 + 5) * 2 / 10' required>
            <div class='example'>Примеры: 5 * (2 + 3), 10 / 2 - 3 + 4, 1.5 + 2.5</div>
            <input type='submit' value='Вычислить'>
        </form>
    </div>
</body>
</html>";

    public static string GenerateResultHtml(string? errorMessage, string resultHtml)
    {
        string content = errorMessage != null
            ? $"<div class='error' style='background:#ffe0e0; padding:1rem; border-radius:12px; color:#b71c1c; font-weight:600'>{errorMessage}</div>"
            : $"<div style='background:#e8f5e9; padding:1.5rem; border-radius:12px; color:#1b5e20; font-weight:600; font-size:1.1rem'>{resultHtml}</div>";

        return $@"
<!DOCTYPE html>
<html lang='ru'>
<head>
    <meta charset='UTF-8'>
    <title>Результат вычисления</title>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;600&display=swap');
        body {{
            font-family: 'Inter', sans-serif;
            background: linear-gradient(135deg, #a1c4fd 0%, #c2e9fb 100%);
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }}
        .container {{
            background: #fff;
            padding: 2.5rem;
            border-radius: 16px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.15);
            max-width: 500px;
            width: 100%;
            text-align: center;
        }}
        h1 {{
            font-size: 2rem;
            color: #2979ff;
            margin-bottom: 1rem;
        }}
        a {{
            display: inline-block;
            margin-top: 2rem;
            text-decoration: none;
            color: #ff6f61;
            font-weight: 600;
            transition: color 0.2s;
        }}
        a:hover {{
            color: #ff3b2f;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>Результат вычисления</h1>
        {content}
        <a href='/'>← Ввести новое выражение</a>
    </div>
</body>
</html>";
    }
}
