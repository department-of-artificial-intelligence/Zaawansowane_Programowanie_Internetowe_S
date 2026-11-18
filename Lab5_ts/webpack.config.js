let htmlWebpackPlugin = require('html-webpack-plugin');
let path = require('path');

// ===== Prosty serwer testowy do zadań 21-27 =====
// (zastępuje konfigurację z Lab05.zip - jeżeli masz ten plik, możesz użyć jego wersji)
const authors = [
  { id: 1, firstName: "Adam", lastName: "Mickiewicz" },
  { id: 2, firstName: "Juliusz", lastName: "Słowacki" },
  { id: 3, firstName: "Bolesław", lastName: "Prus" },
  { id: 4, firstName: "Henryk", lastName: "Sienkiewicz" },
  { id: 5, firstName: "Eliza", lastName: "Orzeszkowa" },
  { id: 6, firstName: "Maria", lastName: "Konopnicka" },
  { id: 7, firstName: "Stefan", lastName: "Żeromski" },
  { id: 8, firstName: "Władysław", lastName: "Reymont" }
];

function sendJson(res, status, data) {
  res.statusCode = status;
  res.setHeader('Content-Type', 'application/json; charset=utf-8');
  res.end(JSON.stringify(data));
}

function api(req, res, next) {
  const url = new URL(req.url, 'http://localhost');
  // ?delay=1500 - sztuczne spowolnienie odpowiedzi (przydatne w zadaniu 26)
  const delay = Number(url.searchParams.get('delay')) || 0;

  if (url.pathname === '/dane.json' && req.method === 'GET') {
    setTimeout(() => sendJson(res, 200, authors), delay);
  }
  else if (url.pathname === '/proceed' && req.method === 'POST') {
    let body = '';
    req.on('data', chunk => body += chunk);
    req.on('end', () => {
      let received;
      try { received = JSON.parse(body); }
      catch { return sendJson(res, 400, { message: 'Niepoprawny JSON' }); }
      sendJson(res, 200, { message: 'Dane odebrane przez serwer', received });
    });
  }
  else if (url.pathname === '/error') {
    sendJson(res, 404, { message: 'Nie znaleziono zasobu' });
  }
  else if (url.pathname === '/error1') {
    // w ok. 70% przypadków błąd 500, w pozostałych poprawna odpowiedź
    if (Math.random() < 0.7) {
      sendJson(res, 500, { message: 'Wewnętrzny błąd serwera' });
    } else {
      sendJson(res, 200, { message: 'Udało się połączyć z serwerem' });
    }
  }
  else {
    next();
  }
}

module.exports = {
  mode: 'development',
  devtool: 'source-map',
  entry: {
    app: './src/index.ts'
  },
  output: {
    path: path.join(__dirname, './build'),
    filename: 'bundle.js'
  },
  resolve: { extensions: ['.js', '.ts'] },
  devServer: {
    static: path.join(__dirname, './build/'),
    port: 9000,
    setupMiddlewares: (middlewares, devServer) => {
      middlewares.unshift({ name: 'lab05-api', middleware: api });
      return middlewares;
    }
  },
  module: {
    rules: [
      { test: /\.ts$/, loader: 'ts-loader' }
    ]
  },
  plugins: [
    new htmlWebpackPlugin({
      hash: true,
      title: 'Lab05',
      template: './build/index.html',
      path: path.join(__dirname, './build/'),
      filename: 'index.html'
    })
  ]
};